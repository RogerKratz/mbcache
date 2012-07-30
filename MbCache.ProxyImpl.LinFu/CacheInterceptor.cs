using System;
using System.Linq;
using System.Reflection;
using LinFu.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
	public class CacheInterceptor : IInterceptor
	{
		[NonSerialized]
		private static readonly MethodInfo exceptionInternalPreserveStackTrace =
			typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

		private readonly CacheAdapter _cache;
		private readonly ICacheKey _cacheKey;
		private readonly ILockObjectGenerator _lockObjectGenerator;
		private readonly Type _type;
		private readonly ImplementationAndMethods _methodData;
		private readonly object _target;
		private readonly ICachingComponent _cachingComponent;

		public CacheInterceptor(CacheAdapter cache,
										ICacheKey cacheKey,
										ILockObjectGenerator lockObjectGenerator, 
										Type type,
										ImplementationAndMethods methodData,
										params object[] ctorParameters)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_lockObjectGenerator = lockObjectGenerator;
			_type = type;
			_methodData = methodData;
			_target = createTarget(ctorParameters);
			_cachingComponent = new CachingComponent(cache, cacheKey, type, methodData);
		}

		private object createTarget(object[] ctorParameters)
		{
			try
			{
				return Activator.CreateInstance(_methodData.ConcreteType, ctorParameters);
			}
			catch (MissingMethodException ex)
			{
				var ctorParamMessage = "Incorrect number of parameters to ctor for type " + _methodData.ConcreteType;
				throw new ArgumentException(ctorParamMessage, ex);
			}
		}

		public object Intercept(InvocationInfo info)
		{
			return methodMarkedForCaching(info.TargetMethod) ?
					  interceptUsingCache(info) : callOriginalMethod(info);
		}

		private bool methodMarkedForCaching(MethodInfo method)
		{
			return _methodData.Methods.Contains(method, new MethodInfoComparer());
		}

		private object interceptUsingCache(InvocationInfo info)
		{
			var method = info.TargetMethod;
			var arguments = info.Arguments;
			var key = _cacheKey.Key(_type, _cachingComponent, method, arguments);
			var getInfo = new EventInformation(key, _type, method, arguments);
			if (key == null)
			{
				return callOriginalMethod(info);
			}
			var cachedValue = _cache.Get(getInfo);
			if (cachedValue != null)
			{
				return cachedValue;
			}
			var lockObject = _lockObjectGenerator.GetFor(key);
			if (lockObject == null)
			{
				return executeAndPutInCache(info, key);
			}
			lock (lockObject)
			{
				var cachedValue2 = _cache.Get(getInfo);
				return cachedValue2 ?? executeAndPutInCache(info, key);
			}
		}

		private object executeAndPutInCache(InvocationInfo info, string key)
		{
			var putInfo = new EventInformation(key, _type, info.TargetMethod, info.Arguments);
			var retVal = callOriginalMethod(info);
			_cache.Put(putInfo, retVal);
			return retVal;
		}

		private object callOriginalMethod(InvocationInfo info)
		{
			var targetMethod = info.TargetMethod;
			if (targetMethod.DeclaringType == typeof(ICachingComponent))
			{
				if (targetMethod.ContainsGenericParameters)
					targetMethod = targetMethod.MakeGenericMethod(info.TypeArguments);
				return invokeMethod(_cachingComponent, targetMethod, info.Arguments);
			}
			return invokeMethod(_target, targetMethod, info.Arguments);
		}

		private static object invokeMethod(object target, MethodInfo method, object[] arguments)
		{
			try
			{
				return method.Invoke(target, arguments);
			}
			catch (TargetInvocationException ex)
			{
				exceptionInternalPreserveStackTrace.Invoke(ex.InnerException, new Object[] { });
				throw ex.InnerException;
			}
		}
	}
}
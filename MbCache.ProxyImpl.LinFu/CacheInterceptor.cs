using System;
using System.Linq;
using System.Reflection;
using LinFu.DynamicProxy;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
	public class CacheInterceptor : IInterceptor
	{
		[NonSerialized]
		private static readonly MethodInfo exceptionInternalPreserveStackTrace =
			typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

		private readonly CacheAdapter _cache;
		private readonly ConfigurationForType _configurationForType;
		private readonly object _target;
		private readonly ICachingComponent _cachingComponent;

		public CacheInterceptor(CacheAdapter cache, ConfigurationForType configurationForType, object target)
		{
			_cache = cache;
			_configurationForType = configurationForType;
			_target = target;
			_cachingComponent = new CachingComponent(cache, configurationForType);
		}

		public object Intercept(InvocationInfo info)
		{
			var realTargetMethod = createGenericMethodInfoIfNeeded(info.TargetMethod, info.TypeArguments);
			return methodMarkedForCaching(realTargetMethod) ?
					  interceptUsingCache(realTargetMethod, info.Arguments) : 
					  callOriginalMethod(realTargetMethod, info.Arguments);
		}

		private static MethodInfo createGenericMethodInfoIfNeeded(MethodInfo orgMethodInfo, Type[] typeArguments)
		{
			return orgMethodInfo.ContainsGenericParameters ? 
				orgMethodInfo.MakeGenericMethod(typeArguments) : 
				orgMethodInfo;
		}

		private bool methodMarkedForCaching(MethodInfo method)
		{
			return _configurationForType.CachedMethods.Contains(method, MethodInfoComparer.Instance) &&
								_configurationForType.EnabledCache;
		}

		private object interceptUsingCache(MethodInfo method, object[] arguments)
		{
			var keyAndItsDependingKeys = _configurationForType.CacheKey.GetAndPutKey(_configurationForType.ComponentType, _cachingComponent, method, arguments);
			return keyAndItsDependingKeys.Key == null ? 
				callOriginalMethod(method, arguments) : 
				_cache.GetAndPutIfNonExisting(keyAndItsDependingKeys, method, arguments, () => callOriginalMethod(method, arguments));
		}

		private object callOriginalMethod(MethodInfo method, object[] arguments)
		{
			return invokeMethod(method.DeclaringType == typeof(ICachingComponent) ? _cachingComponent : _target, method, arguments);
		}

		private static object invokeMethod(object target, MethodInfo method, object[] arguments)
		{
			try
			{
				return method.Invoke(target, arguments);
			}
			catch (TargetInvocationException ex)
			{
				exceptionInternalPreserveStackTrace.Invoke(ex.InnerException, new object[] { });
				throw ex.InnerException;
			}
		}
	}
}
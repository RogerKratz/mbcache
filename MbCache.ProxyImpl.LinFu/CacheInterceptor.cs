using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu;

public class CacheInterceptor : IInterceptor
{
	private static readonly MethodInfo exceptionInternalPreserveStackTrace =
		typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

	private readonly ConfigurationForType _configurationForType;
	private readonly object _target;
	private readonly ICachingComponent _cachingComponent;

	public CacheInterceptor(ConfigurationForType configurationForType, object target)
	{
		_configurationForType = configurationForType;
		_target = target;
		_cachingComponent = new CachingComponent(configurationForType);
	}

	public object Intercept(InvocationInfo info)
	{
		var realTargetMethod = createGenericMethodInfoIfNeeded(info.TargetMethod, info.TypeArguments);
			
		var cachedMethod = _configurationForType.CachedMethods.SingleOrDefault(x => x.SameMethodAs(realTargetMethod));
		if (_configurationForType.EnabledCache && cachedMethod != null)
		{
			return interceptUsingCache(realTargetMethod, info.Arguments, cachedMethod.ReturnValuesNotToCache);
		}

		return callOriginalMethod(realTargetMethod, info.Arguments);
	}

	private static MethodInfo createGenericMethodInfoIfNeeded(MethodInfo orgMethodInfo, Type[] typeArguments)
	{
		return orgMethodInfo.ContainsGenericParameters ? 
			orgMethodInfo.MakeGenericMethod(typeArguments) : 
			orgMethodInfo;
	}

	private object interceptUsingCache(MethodInfo methodInfo, object[] arguments, IEnumerable<object> returnValuesNotToCache)
	{
		var keyAndItsDependingKeys = _configurationForType.CacheKey.GetAndPutKey(_configurationForType.ComponentType, _cachingComponent, methodInfo, arguments);
		return keyAndItsDependingKeys.Key == null ? 
			callOriginalMethod(methodInfo, arguments) : 
			_configurationForType.Cache.GetAndPutIfNonExisting(keyAndItsDependingKeys, methodInfo, () =>
			{
				var returnValue = callOriginalMethod(methodInfo, arguments);
				var shouldBeCached = !returnValuesNotToCache.Contains(returnValue);
				return new OriginalMethodResult(returnValue, shouldBeCached);
			});
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
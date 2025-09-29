using System;
using System.Linq;
using System.Reflection;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic.Proxy;

public sealed class ProxyInterceptor(
	object target,
	ConfigurationForType configurationForType,
	ICachingComponent cachingComponent)
{
	private static readonly MethodInfo exceptionInternalPreserveStackTrace =
		typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

	public ICachingComponent Component { get; } = cachingComponent;

	public object DecorateMethodCall(MethodInfo method, object[] arguments)
	{
		if (configurationForType.EnabledCache)
		{
			var cachedMethod = configurationForType.CachedMethods.SingleOrDefault(x => x.SameMethodAs(method)); 
			if (cachedMethod != null)
			{
				var keyAndItsDependingKeys = configurationForType.CacheKey.GetAndPutKey(configurationForType.ComponentType, Component, method, arguments);
				return keyAndItsDependingKeys.Key == null ? 
					callOriginalMethod(method, arguments) : 
					configurationForType.Cache.GetAndPutIfNonExisting(keyAndItsDependingKeys, method, () =>
					{
						var returnValue = callOriginalMethod(method, arguments);
						var shouldBeCached = !cachedMethod.ReturnValuesNotToCache.Contains(returnValue);
						return new OriginalMethodResult(returnValue, shouldBeCached);
					});	
			}	
		}

		return callOriginalMethod(method, arguments);
	}

	private object callOriginalMethod(MethodInfo method, object[] arguments)
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
using System;
using System.Linq;
using System.Reflection;
using MbCache.Core;

namespace MbCache.Logic.Proxy
{
	public class ProxyInterceptor
	{
		private readonly object _target;
		private readonly ConfigurationForType _configurationForType;
		private static readonly MethodInfo exceptionInternalPreserveStackTrace =
			typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

		public ProxyInterceptor(object target, 
			ConfigurationForType configurationForType, 
			ICachingComponent cachingComponent)
		{
			_target = target;
			_configurationForType = configurationForType;
			Component = cachingComponent;
		}
		
		public ICachingComponent Component { get; }

		public object DecorateMethodCall(MethodInfo method, object[] arguments)
		{
			if (_configurationForType.CachedMethods.Contains(method, MethodInfoComparer.Instance) && _configurationForType.EnabledCache)
			{
				var keyAndItsDependingKeys = _configurationForType.CacheKey.GetAndPutKey(_configurationForType.ComponentType, Component, method, arguments);
				return keyAndItsDependingKeys.Key == null ? 
					callOriginalMethod(method, arguments) : 
					_configurationForType.Cache.GetAndPutIfNonExisting(keyAndItsDependingKeys, method, () => callOriginalMethod(method, arguments));	
			}

			return callOriginalMethod(method, arguments);
		}

		private object callOriginalMethod(MethodInfo method, object[] arguments)
		{
			try
			{
				return method.Invoke(_target, arguments);
			}
			catch (TargetInvocationException ex)
			{
				exceptionInternalPreserveStackTrace.Invoke(ex.InnerException, new object[] { });
				throw ex.InnerException;
			}
		}
	}
}
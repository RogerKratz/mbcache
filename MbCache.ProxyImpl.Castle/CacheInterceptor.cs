using System.Linq;
using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
	public class CacheInterceptor : IInterceptor
	{
		private readonly CacheAdapter _cache;
		private readonly ICacheKey _cacheKey;
		private readonly ConfigurationForType _configurationForType;

		public CacheInterceptor(CacheAdapter cache, 
										ICacheKey cacheKey, 
										ConfigurationForType configurationForType)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_configurationForType = configurationForType;
		}

		public void Intercept(IInvocation invocation)
		{
			if (!_configurationForType.EnabledCache)
			{
				invocation.Proceed();
				return;
			}

			var method = invocation.Method;
			//ugly hack
			if (method.IsGenericMethod && !_configurationForType.CachedMethods.Contains(method, MethodInfoComparer.Instance))
			{
				invocation.Proceed();
				return;
			}
			var proxy = (ICachingComponent)invocation.Proxy;
			var arguments = invocation.Arguments;

			var key = _cacheKey.Key(_configurationForType.ComponentType, proxy, method, arguments);
			if (key == null)
			{
				invocation.Proceed();
			}
			else
			{
				var eventInfo = new EventInformation(key, _configurationForType.ComponentType.ConfiguredType, invocation.Method, invocation.Arguments);
				var hasCalledOriginalMethod = false;
				var result = _cache.GetAndPutIfNonExisting(eventInfo, () =>
				{
					invocation.Proceed();
					hasCalledOriginalMethod = true;
					return invocation.ReturnValue;
				});
				if (!hasCalledOriginalMethod)
				{
					invocation.ReturnValue = result;
				}
			}
		}
	}
}
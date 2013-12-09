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
		private readonly ILockObjectGenerator _lockObjectGenerator;
		private readonly ConfigurationForType _configurationForType;

		public CacheInterceptor(CacheAdapter cache, 
										ICacheKey cacheKey, 
										ILockObjectGenerator lockObjectGenerator,
										ConfigurationForType configurationForType)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_lockObjectGenerator = lockObjectGenerator;
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
				if (tryGetValueFromCache(invocation, eventInfo))
					return;

				var lockObject = _lockObjectGenerator.GetFor(key);
				if (lockObject == null)
				{
					executeAndPutInCache(invocation, eventInfo);
				}
				else
				{
					lock (lockObject)
					{
						if (tryGetValueFromCache(invocation, eventInfo))
							return;
						executeAndPutInCache(invocation, eventInfo);
					}				
				}
			}
		}

		private void executeAndPutInCache(IInvocation invocation, EventInformation eventInfo)
		{
			invocation.Proceed();
			_cache.Put(new CachedItem(eventInfo, invocation.ReturnValue));
		}

		private bool tryGetValueFromCache(IInvocation invocation, EventInformation eventInfo)
		{
			var cachedValue = _cache.Get(eventInfo);
			if (cachedValue != null)
			{
				invocation.ReturnValue = cachedValue;
				return true;
			}
			return false;
		}
	}
}
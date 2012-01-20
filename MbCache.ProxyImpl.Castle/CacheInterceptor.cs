using System;
using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.ProxyImpl.Castle
{
	public class CacheInterceptor : IInterceptor
	{
		private readonly ICache _cache;
		private readonly IMbCacheKey _cacheKey;
		private readonly Type _type;

		public CacheInterceptor(ICache cache, IMbCacheKey cacheKey, Type type)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_type = type;
		}

		public void Intercept(IInvocation invocation)
		{
			var method = invocation.Method;
			var proxy = (ICachingComponent)invocation.Proxy;
			var arguments = invocation.Arguments;

			var key = _cacheKey.Key(_type, proxy, method, arguments);
			if (key == null)
			{
				invocation.Proceed();
			}
			else
			{
				var cachedValue = _cache.Get(key);
				if (cachedValue != null)
				{
					invocation.ReturnValue = cachedValue;
				}
				else
				{
					invocation.Proceed();
					_cache.Put(key, invocation.ReturnValue);
				}
			}
		}
	}
}
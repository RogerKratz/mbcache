using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
	[Serializable]
	public class ProxyFactory : IProxyFactory
	{
		private CacheAdapter _cache;
		private ICacheKey _cacheKey;
		private ILockObjectGenerator _lockObjectGenerator;

		public void Initialize(CacheAdapter cache, ICacheKey cacheKey, ILockObjectGenerator lockObjectGenerator)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_lockObjectGenerator = lockObjectGenerator;
		}

		public T CreateProxy<T>(ImplementationAndMethods methodData, params object[] parameters) where T : class
		{
			var proxyFactory = new global::LinFu.DynamicProxy.ProxyFactory();
			var interceptor = new CacheInterceptor(_cache, _cacheKey, _lockObjectGenerator, typeof(T), methodData, parameters);
			return proxyFactory.CreateProxy<T>(interceptor, typeof(ICachingComponent));
		}

		public bool AllowNonVirtualMember
		{
			get { return false; }
		}
	}
}
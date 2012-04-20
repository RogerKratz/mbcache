using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
	public class ProxyFactory : IProxyFactory
	{
		private ICache _cache;
		private IMbCacheKey _mbCacheKey;

		public void Initialize(ICache cache, IMbCacheKey mbCacheKey)
		{
			_cache = cache;
			_mbCacheKey = mbCacheKey;
		}

		public T CreateProxy<T>(ImplementationAndMethods methodData, params object[] parameters) where T : class
		{
			var proxyFactory = new global::LinFu.DynamicProxy.ProxyFactory();
			var interceptor = new CacheInterceptor(_cache, _mbCacheKey, typeof(T), methodData, parameters);
			return proxyFactory.CreateProxy<T>(interceptor, typeof(ICachingComponent));
		}

		public bool AllowNonVirtualMember
		{
			get { return false; }
		}
	}
}
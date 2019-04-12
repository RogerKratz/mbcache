using LinFu.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
	public class LinFuProxyFactory : IProxyFactory
	{
		private static readonly ProxyFactory proxyFactory = new ProxyFactory();

		public T CreateProxyWithTarget<T>(T uncachedComponent, ConfigurationForType configurationForType) where T : class
		{
			var interceptor = new CacheInterceptor(configurationForType, uncachedComponent);
			return proxyFactory.CreateProxy<T>(interceptor, typeof(ICachingComponent));
		}
	}
}
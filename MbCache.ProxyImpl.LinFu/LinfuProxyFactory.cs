using LinFu.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
	public class LinFuProxyFactory : IProxyFactory
	{
		private static readonly ProxyFactory proxyFactory = new();

		public T CreateProxy<T>(T target, ConfigurationForType configurationForType) where T : class
		{
			var interceptor = new CacheInterceptor(configurationForType, target);
			return proxyFactory.CreateProxy<T>(interceptor, typeof(ICachingComponent));
		}
	}
}
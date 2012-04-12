using MbCache.Configuration;

namespace MbCacheTest
{
	public static class ConfigurationData
	{
		private static readonly IProxyFactory castleProxy = new MbCache.ProxyImpl.Castle.ProxyFactory();
		private static readonly IProxyFactory linFuProxy = new MbCache.ProxyImpl.LinFu.ProxyFactory();

		//conf values
		public static readonly IProxyFactory ProxyFactory = linFuProxy;
	}
}
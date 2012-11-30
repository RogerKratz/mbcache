using NUnit.Framework;

namespace MbCacheTest
{
	[TestFixture("MbCache.ProxyImpl.Castle.CastleProxyFactory, MbCache.ProxyImpl.Castle")]
	[TestFixture("MbCache.ProxyImpl.LinFu.LinFuProxyFactory, MbCache.ProxyImpl.LinFu")]
	public abstract class FullTest : SimpleTest
	{
		protected FullTest(string proxyTypeString) : base(proxyTypeString)
		{
		}
	}
}
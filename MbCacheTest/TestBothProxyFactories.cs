using NUnit.Framework;

namespace MbCacheTest
{

	[TestFixture("MbCache.ProxyImpl.Castle.ProxyFactory, MbCache.ProxyImpl.Castle")]
	[TestFixture("MbCache.ProxyImpl.LinFu.ProxyFactory, MbCache.ProxyImpl.LinFu")]
	public abstract class TestBothProxyFactories : SimpleTest
	{
		protected TestBothProxyFactories(string proxyTypeString) : base(proxyTypeString)
		{
		}
	}
}
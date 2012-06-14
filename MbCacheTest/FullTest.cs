using NUnit.Framework;

namespace MbCacheTest
{
	[TestFixture("MbCache.ProxyImpl.Castle.ProxyFactory, MbCache.ProxyImpl.Castle")]
	[TestFixture("MbCache.ProxyImpl.LinFu.ProxyFactory, MbCache.ProxyImpl.LinFu")]
	public abstract class FullTest : SimpleTest
	{
		protected FullTest(string proxyTypeString) : base(proxyTypeString)
		{
		}
	}
}
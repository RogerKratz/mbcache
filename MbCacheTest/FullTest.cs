using System;
using MbCache.ProxyImpl.Castle;
using MbCache.ProxyImpl.LinFu;
using NUnit.Framework;

namespace MbCacheTest
{
	[TestFixture(typeof(CastleProxyFactory))]
	[TestFixture(typeof(LinFuProxyFactory))]
	public abstract class FullTest : SimpleTest
	{
		protected FullTest(Type proxyType) : base(proxyType)
		{
		}
	}
}
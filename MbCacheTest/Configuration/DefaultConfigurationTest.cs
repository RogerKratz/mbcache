using System;
using MbCache.Configuration;
using MbCacheTest.Caches;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Configuration
{
	[TestFixture("MbCache.ProxyImpl.Castle.CastleProxyFactory, MbCache.ProxyImpl.Castle")]
	[TestFixture("MbCache.ProxyImpl.LinFu.LinFuProxyFactory, MbCache.ProxyImpl.LinFu")]
	public class DefaultConfigurationTest
	{
		private readonly IProxyFactory _proxyFactory;

		public DefaultConfigurationTest(string proxyTypeString)
		{
			var proxyType = Type.GetType(proxyTypeString, true);
			_proxyFactory = (IProxyFactory)Activator.CreateInstance(proxyType);
		}

		[Test] 
		public void ShouldCache()
		{
			using (var factory = new CacheBuilder(_proxyFactory)
				.For<ObjectReturningNewGuids>()
				.CacheMethod(c => c.CachedMethod())
				.As<IObjectReturningNewGuids>()
				.BuildFactory())
			{
				var component = factory.Create<IObjectReturningNewGuids>();

				component.CachedMethod().Should().Be.EqualTo(component.CachedMethod());
			}
		}
	}
}
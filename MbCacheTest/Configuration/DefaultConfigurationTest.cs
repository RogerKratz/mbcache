using System;
using MbCache.Configuration;
using MbCacheTest.Caches;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Configuration
{
	[TestFixture("MbCache.ProxyImpl.Castle.ProxyFactory, MbCache.ProxyImpl.Castle")]
	[TestFixture("MbCache.ProxyImpl.LinFu.ProxyFactory, MbCache.ProxyImpl.LinFu")]
	public class DefaultConfigurationTest
	{
		private readonly IProxyFactory proxyFactory;

		public DefaultConfigurationTest(string proxyTypeString)
		{
			var proxyType = Type.GetType(proxyTypeString, true);
			proxyFactory = (IProxyFactory)Activator.CreateInstance(proxyType);
		}

		[Test] 
		public void ShouldCache()
		{
			Tools.ClearMemoryCache();

			var factory = new CacheBuilder(proxyFactory)
				.For<ObjectReturningNewGuids>()
					.CacheMethod(c => c.CachedMethod())
					.As<IObjectReturningNewGuids>()
				.BuildFactory();

			var component = factory.Create<IObjectReturningNewGuids>();

			component.CachedMethod().Should().Be.EqualTo(component.CachedMethod());
		}
	}
}
using MbCache.Configuration;
using MbCache.ProxyImpl.LinFu;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Configuration
{
	public class DefaultConfigurationTest
	{
		[Test] 
		public void ShouldCache()
		{
			using (var factory = new CacheBuilder()
				.SetProxyFactory(new LinFuProxyFactory())
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
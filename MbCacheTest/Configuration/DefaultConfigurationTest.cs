using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Configuration;

public class DefaultConfigurationTest
{
	[Test] 
	public void ShouldCache()
	{
		using (var factory = new CacheBuilder()
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
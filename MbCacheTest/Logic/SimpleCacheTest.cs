using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class SimpleCacheTest : TestCase
{
	private IMbCacheFactory factory;
	
	protected override void TestSetup()
	{
		CacheBuilder.For<ReturningRandomNumbers>()
			.CacheMethod(c => c.CachedNumber())
			.CacheMethod(c => c.CachedNumber2())
			.As<IReturningRandomNumbers>();

		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void VerifyCacheIsWorking()
	{
		var obj1 = factory.Create<IReturningRandomNumbers>();
		var obj2 = factory.Create<IReturningRandomNumbers>();
		
		obj1.CachedNumber().Should().Be.EqualTo(obj2.CachedNumber());
		obj1.CachedNumber2().Should().Be.EqualTo(obj2.CachedNumber2());
		obj1.CachedNumber().Should().Not.Be.EqualTo(obj2.CachedNumber2());
		obj1.NonCachedNumber().Should().Not.Be.EqualTo(obj2.NonCachedNumber());
	}
}
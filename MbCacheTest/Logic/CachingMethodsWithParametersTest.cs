using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class CachingMethodsWithParametersTest
{
	private IMbCacheFactory factory;

	[SetUp]
	public void Setup() =>
		factory = new CacheBuilder()
			.For<ObjectWithParametersOnCachedMethod>()
			.CacheMethod(c => c.CachedMethod(null))
			.As<IObjectWithParametersOnCachedMethod>()
			.BuildFactory();

	[Test]
	public void VerifySameParameterGivesCacheHit()
	{
		factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("hej")
			.Should().Be.EqualTo(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("hej"));
	}

	[Test]
	public void VerifyDifferentParameterGivesNoCacheHit()
	{
		factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("roger")
			.Should().Not.Be.EqualTo(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
	}

	[Test]
	public void NullAsParameter()
	{
		factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null)
			.Should().Not.Be.EqualTo(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
		factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null)
			.Should().Be.EqualTo(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null));
	}

	[Test]
	public void InvalidateOnTypeWorks()
	{
		var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
		var value = obj.CachedMethod("hej");
		factory.Invalidate<IObjectWithParametersOnCachedMethod>();
		
		value.Should().Not.Be.EqualTo(obj.CachedMethod("hej"));
	}

	[Test]
	public void InvalidateOnInstanceWorks()
	{
		var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
		var value = obj.CachedMethod("hej");
		factory.Invalidate(obj);
		
		value.Should().Not.Be.EqualTo(obj.CachedMethod("hej"));
	}
}
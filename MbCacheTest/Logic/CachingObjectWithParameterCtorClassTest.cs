using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class CachingObjectWithParameterCtorClassTest
{
	private IMbCacheFactory factory;

	[SetUp]
	public void Setup() =>
		factory = new CacheBuilder()
			.For<ObjectWithCtorParametersNoInterface>()
			.CacheMethod(c => c.CachedMethod())
			.AsImplemented().BuildFactory();

	[Test]
	public void CanReadProps()
	{
		var obj = factory.Create<ObjectWithCtorParametersNoInterface>(13);
		obj.Value.Should().Be.EqualTo(13);
	}

	[Test]
	public void InvalidParametersThrows()
	{
		Assert.Throws<ArgumentException>(() => factory.Create<ObjectWithCtorParametersNoInterface>());
	}

	[Test]
	public void VerifyCacheWorks()
	{
		factory.Create<ObjectWithCtorParametersNoInterface>(4).CachedMethod()
			.Should().Be.EqualTo(factory.Create<ObjectWithCtorParametersNoInterface>(4).CachedMethod());
	}

	[Test]
	public void VerifyCacheDifferentParameters()
	{
		factory.Create<ObjectWithCtorParametersNoInterface>(3).CachedMethod()
			.Should().Be.EqualTo(factory.Create<ObjectWithCtorParametersNoInterface>(4).CachedMethod());
	}

	[Test]
	public void CanWrapUncachingComponent()
	{
		var uncachingComponent = new ObjectWithCtorParametersNoInterface(1);
		var cachedComponent = factory.ToCachedComponent(uncachingComponent);

		cachedComponent.CachedMethod()
			.Should().Be.EqualTo(cachedComponent.CachedMethod());
	}
}
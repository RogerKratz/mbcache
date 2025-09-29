using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class CachingObjectWithParameterCtorInterfaceTest
{
	private IMbCacheFactory factory;
	
	[SetUp]
	public void Setup() =>
		factory = new CacheBuilder()
			.For<ObjectWithCtorParameters>()
			.CacheMethod(c => c.CachedMethod())
			.As<IObjectWithCtorParameters>()
			.BuildFactory();

	[Test]
	public void CanReadProps()
	{
		var obj = factory.Create<IObjectWithCtorParameters>(11, 12);
		obj.Value1.Should().Be.EqualTo(11);
		obj.Value2.Should().Be.EqualTo(12);
	}

	[Test]
	public void InvalidParametersThrows()
	{
		Assert.Throws<ArgumentException>(() => factory.Create<IObjectWithCtorParameters>());
	}

	[Test]
	public void VerifyCacheWorks()
	{
		factory.Create<IObjectWithCtorParameters>(4, 3).CachedMethod()
			.Should().Be.EqualTo(factory.Create<IObjectWithCtorParameters>(4, 3).CachedMethod());
	}

	[Test]
	public void VerifyCacheDifferentParameters()
	{
		factory.Create<IObjectWithCtorParameters>(4, 4).CachedMethod()
			.Should().Be.EqualTo(factory.Create<IObjectWithCtorParameters>(4, 3).CachedMethod());
	}

	[Test]
	public void CanWrapUncachingComponent()
	{
		var uncachingComponent = new ObjectWithCtorParameters(1, 1);
		var cachedComponent = factory.ToCachedComponent((IObjectWithCtorParameters)uncachingComponent);

		cachedComponent.CachedMethod()
			.Should().Be.EqualTo(cachedComponent.CachedMethod());
	}
}
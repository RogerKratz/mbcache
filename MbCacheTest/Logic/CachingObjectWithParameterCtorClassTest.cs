using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class CachingObjectWithParameterCtorClassTest : TestCase
{
	private IMbCacheFactory factory;

	public CachingObjectWithParameterCtorClassTest(Type proxyType) : base(proxyType)
	{
	}

	protected override void TestSetup()
	{
		CacheBuilder
			.For<ObjectWithCtorParametersNoInterface>()
			.CacheMethod(c => c.CachedMethod())
			.AsImplemented();

		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void CanReadProps()
	{
		var obj = factory.Create<ObjectWithCtorParametersNoInterface>(13);
		Assert.AreEqual(13, obj.Value);
	}

	[Test]
	public void InvalidParametersThrows()
	{
		Assert.Throws<ArgumentException>(() => factory.Create<ObjectWithCtorParametersNoInterface>());
	}

	[Test]
	public void VerifyCacheWorks()
	{
		Assert.AreEqual(factory.Create<ObjectWithCtorParametersNoInterface>(4).CachedMethod(),
			factory.Create<ObjectWithCtorParametersNoInterface>(4).CachedMethod());
	}

	[Test]
	public void VerifyCacheDifferentParameters()
	{
		Assert.AreEqual(factory.Create<ObjectWithCtorParametersNoInterface>(3).CachedMethod(),
			factory.Create<ObjectWithCtorParametersNoInterface>(4).CachedMethod());
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
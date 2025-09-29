using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Generics;

public class GenericParametersTest : TestCase
{
	private IMbCacheFactory factory;

	protected override void TestSetup()
	{
		factory = CacheBuilder.For<ObjectWithGenericMethodParameters>()
			.CacheMethod(m => m.CachedMethod1(1, "1"))
			.AsImplemented()
			.BuildFactory();
	}

	[Test]
	public void ShouldCache()
	{
		var component = factory.Create<ObjectWithGenericMethodParameters>();
		component.CachedMethod1(1, "1")
			.Should().Be.EqualTo(component.CachedMethod1(1, "1"));
	}

	[Test]
	public void ShouldNotCache()
	{
		var component = factory.Create<ObjectWithGenericMethodParameters>();
		component.CachedMethod1("1", "1")
			.Should().Not.Be.EqualTo(component.CachedMethod1("1", "1"));
	}
}
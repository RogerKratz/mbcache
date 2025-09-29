using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic;

public class CacheKeyTest
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
	public void ShouldThrowIfSuspiciousParameterIsUsed()
	{
		var comp = factory.Create<IObjectWithParametersOnCachedMethod>();
		Assert.Throws<ArgumentException>(() => comp.CachedMethod(this));
	}
}
using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic;

public class CacheKeyTest : TestCase
{
	private IMbCacheFactory factory;

	protected override void TestSetup()
	{
		CacheBuilder
			.For<ObjectWithParametersOnCachedMethod>()
			.CacheMethod(c => c.CachedMethod(null))
			.As<IObjectWithParametersOnCachedMethod>();
			
		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void ShouldThrowIfSuspiciousParameterIsUsed()
	{
		var comp = factory.Create<IObjectWithParametersOnCachedMethod>();
		Assert.Throws<ArgumentException>(() => comp.CachedMethod(this));
	}
}
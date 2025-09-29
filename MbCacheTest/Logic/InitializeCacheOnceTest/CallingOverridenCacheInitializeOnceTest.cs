using System;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.InitializeCacheOnceTest;

public class CallingOverridenCacheInitializeOnceTest : TestCase
{
	private CacheWithInitializeCounter overridenCache;


	protected override void TestSetup()
	{
		overridenCache = new CacheWithInitializeCounter();
		CacheBuilder
			.For<ReturningRandomNumbers>()
			.CacheMethod(c => c.CachedNumber())
			.OverrideCache(overridenCache)
			.As<IReturningRandomNumbers>()
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.OverrideCache(overridenCache)
			.As<IObjectReturningNewGuids>()
			.BuildFactory();
	}

	[Test]
	public void ShouldOnlyInitializeOnce()
	{
		overridenCache.InitializeCounter
			.Should().Be.EqualTo(1);
	}
}
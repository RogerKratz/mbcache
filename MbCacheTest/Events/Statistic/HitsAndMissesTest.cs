using System;
using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Events.Statistic;

public class HitsAndMissesTest : TestCase
{
	private IMbCacheFactory factory;
	private StatisticsEventListener eventListener;

	public HitsAndMissesTest(Type proxyType) : base(proxyType)
	{
	}

	protected override void TestSetup()
	{
		eventListener = new StatisticsEventListener();
		CacheBuilder
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.As<IObjectReturningNewGuids>()
			.AddEventListener(eventListener);

		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void VerifyCacheHits()
	{
		var comp = factory.Create<IObjectReturningNewGuids>();
		comp.CachedMethod();
		Assert.AreEqual(0, eventListener.CacheHits);
		comp.CachedMethod();
		Assert.AreEqual(1, eventListener.CacheHits);
		comp.CachedMethod2();
		Assert.AreEqual(1, eventListener.CacheHits);
	}

	[Test]
	public void VerifyCacheMisses()
	{
		var comp = factory.Create<IObjectReturningNewGuids>();
		comp.CachedMethod();
		Assert.AreEqual(1, eventListener.CacheMisses);
		comp.CachedMethod();
		Assert.AreEqual(1, eventListener.CacheMisses);
		comp.CachedMethod2();
		Assert.AreEqual(1, eventListener.CacheMisses);
	}

	[Test]
	public void VerifyClear()
	{
		var comp = factory.Create<IObjectReturningNewGuids>();
		comp.CachedMethod();
		comp.CachedMethod();
		eventListener.Clear();
		eventListener.CacheHits.Should().Be.EqualTo(0);
		eventListener.CacheMisses.Should().Be.EqualTo(0);
	}
}
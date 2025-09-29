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
		eventListener.CacheHits.Should().Be.EqualTo(0);
		comp.CachedMethod();
		eventListener.CacheHits.Should().Be.EqualTo(1);
		comp.CachedMethod2();
		eventListener.CacheHits.Should().Be.EqualTo(1);
	}

	[Test]
	public void VerifyCacheMisses()
	{
		var comp = factory.Create<IObjectReturningNewGuids>();
		comp.CachedMethod();
		eventListener.CacheMisses.Should().Be.EqualTo(1);
		comp.CachedMethod();
		eventListener.CacheMisses.Should().Be.EqualTo(1);
		comp.CachedMethod2();
		eventListener.CacheMisses.Should().Be.EqualTo(1);
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
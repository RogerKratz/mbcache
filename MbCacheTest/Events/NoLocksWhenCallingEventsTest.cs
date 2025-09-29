using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Events;

public class NoLocksWhenCallingEventsTest
{
	private IMbCacheFactory factory;

	[SetUp]
	public void Setup()
	{
		factory = new CacheBuilder()
			.For<ObjectWithParametersOnCachedMethod>()
			.CacheMethod(c => c.CachedMethod(null))
			.As<IObjectWithParametersOnCachedMethod>()
			.AddEventListener(new slowEventListener())
			.BuildFactory();
	}

	[Test]
	public void ShouldCallHitAndMissOutsideLock()
	{
		if(Environment.ProcessorCount < 4)
			Assert.Ignore("Not reliable if too few processors.");
		var instance = factory.Create<IObjectWithParametersOnCachedMethod>();
		var tasks = new List<Task>();

		var stopwatch = Stopwatch.StartNew();
		10.Times(x =>
		{
			tasks.Add(Task.Factory.StartNew(() =>
			{
				instance.CachedMethod(x);
			}));
		});
		Task.WaitAll(tasks.ToArray());
		Console.WriteLine(stopwatch.Elapsed);
		if (stopwatch.Elapsed > TimeSpan.FromMilliseconds(500))
			Assert.Fail("Seems to be calling event listener from within lock");
	}

	private class slowEventListener : IEventListener
	{
		public void OnCacheHit(CachedItem cachedItem)
		{
			Thread.Sleep(100);
		}

		public void OnCacheRemoval(CachedItem cachedItem)
		{
			throw new NotImplementedException();
		}

		public void OnCacheMiss(CachedItem cachedItem)
		{
			Thread.Sleep(100);
		}
	}
}
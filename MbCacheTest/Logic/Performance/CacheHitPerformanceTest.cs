using System;
using System.Diagnostics;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.Performance
{
	public class CacheHitPerformanceTest : TestCase
	{
		private IReturningRandomNumbers instance;

		public CacheHitPerformanceTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ReturningRandomNumbers>()
				 .CacheMethod(c => c.CachedNumber())
				 .As<IReturningRandomNumbers>();

			var factory = CacheBuilder.BuildFactory();
			instance = factory.Create<IReturningRandomNumbers>();
		}

		[Test]
		public void MeasureCacheHitPerf()
		{
			const int loops = 300000;
			var stopwatch = new Stopwatch();

			instance.CachedNumber();
			stopwatch.Start();
			for (var i = 0; i < loops; i++)
			{
				instance.CachedNumber();
			}
			stopwatch.Stop();

			Console.WriteLine(stopwatch.Elapsed);
			//On my computer 
			//~0.5s, linfu
		}
	}
}
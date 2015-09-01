using System;
using System.Diagnostics;
using log4net;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.Performance
{
	public class CacheHitPerformanceTest : FullTest
	{
		private IReturningRandomNumbers instance;

		public CacheHitPerformanceTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			LogManager.Shutdown(); 
			CacheBuilder.For<ReturningRandomNumbers>()
				 .CacheMethod(c => c.CachedNumber())
				 .As<IReturningRandomNumbers>();

			var factory = CacheBuilder.BuildFactory();
			instance = factory.Create<IReturningRandomNumbers>();
		}

		[TearDown]
		public void AfterTest()
		{
			SetupFixtureForAssembly.StartLog4Net();
		}

		[Test]
		public void MeasureCacheHitPerf()
		{
			const int loops = 300000;
			instance.CachedNumber();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (var i = 0; i < loops; i++)
			{
				instance.CachedNumber();
			}
			stopwatch.Stop();
			Console.WriteLine(stopwatch.Elapsed);
			//On my computer 
			//~0.7s, castle
			//~1.4s, linfu
		}
	}
}
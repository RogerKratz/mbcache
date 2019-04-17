using System;
using System.Diagnostics;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.Performance
{
	public class NonCachedPerformanceTest : TestCase
	{
		private ObjectWithMutableList instance;

		public NonCachedPerformanceTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectWithMutableList>()
				.CacheMethod(c => c.GetListContents())
				.AsImplemented();

			var factory = CacheBuilder.BuildFactory();
			instance = factory.Create<ObjectWithMutableList>();
		}

		[Test]
		public void MeasureCacheHitPerf()
		{
			const int loops = 500000;
			var stopwatch = Stopwatch.StartNew();

			for (var i = 0; i < loops; i++)
			{
				instance.AddToList(i.ToString());
			}

			Console.WriteLine(stopwatch.Elapsed);
			//On my computer 
			//~0.4s, castle
			//~0.7s, linfu
		}
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Performance
{
	public class CacheMissPerformanceTest : FullTest
	{
		private ObjectTakes100MsToFill instance;
		
		public CacheMissPerformanceTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectTakes100MsToFill>()
				.CacheMethod(c => c.Execute(0))
				.AsImplemented();

			var factory = CacheBuilder.BuildFactory();
			instance = factory.Create<ObjectTakes100MsToFill>();
		}
		
		[Test]
		[Ignore("Fix by using smarter/slimmer lock in InMemoryCache")]
		public void MeasureCacheMissPerf()
		{
			var tasks = new List<Task>();
			10.Times(i =>
			{
				tasks.Add(Task.Factory.StartNew(() => instance.Execute(i)));
			});
			
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			Task.WaitAll(tasks.ToArray());
			stopwatch.Stop();

			var timeTaken = stopwatch.Elapsed;
			Console.WriteLine(timeTaken);
			timeTaken.TotalMilliseconds.Should().Be.LessThan(100 * 3);
		}
	}
}
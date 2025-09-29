using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Performance;

public class CacheMissPerformanceTest
{
	private ObjectTakes100MsToFill instance;

	[SetUp]
	public void Setup()
	{
		var factory = new CacheBuilder()
			.For<ObjectTakes100MsToFill>()
			.CacheMethod(c => c.Execute(0))
			.AsImplemented()
			.BuildFactory();
		instance = factory.Create<ObjectTakes100MsToFill>();
	}
		
	[Test]
	public void MeasureCacheMissPerf()
	{
		if(Environment.ProcessorCount < 3)
			Assert.Ignore("Not enough power...");
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
		//If global lock, approx 10 * 0.1 => 1s
		timeTaken.TotalMilliseconds.Should().Be.LessThan(700);
	}
}
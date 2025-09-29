using System.Threading;
using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Concurrency;

public class SimultaneousCachePutTest
{
	[Test]
	public void ShouldNotMakeTheSameCallMoreThanOnce()
	{
		var factory = new CacheBuilder()
			.For<ObjectWithCallCounter>()
			.CacheMethod(c => c.Increment())
			.PerInstance()
			.As<IObjectWithCallCounter>()
			.BuildFactory();

		1000.Times(() =>
		{
			var instance = factory.Create<IObjectWithCallCounter>();

			var task1 = new Thread(() => instance.Increment());
			var task2 = new Thread(() => instance.Increment());
			var task3 = new Thread(() => instance.Increment());
			task1.Start();
			task2.Start();
			task3.Start();
			task1.Join();
			task2.Join();
			task3.Join();

			instance.Count.Should().Be(1);
		});
	}
}
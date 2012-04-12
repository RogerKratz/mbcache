using System.Threading.Tasks;
using MbCache.Configuration;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Concurrency
{
	[TestFixture]
	public class SimultaniousCachePutTest
	{
		[Test, Ignore("Issue 6 - not yet fixed")]
		public void ShouldNotMakeTheSameCallMoreThanOnce()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey());

			builder.For<ObjectWithCallCounter>()
				 .CacheMethod(c => c.Increment())
				 .PerInstance()
				 .As<IObjectWithCallCounter>();

			var factory = builder.BuildFactory();

			10.Times(() =>
			{
				var instance = factory.Create<IObjectWithCallCounter>();

				var task1 = Task.Factory.StartNew(() => instance.Increment());
				var task2 = Task.Factory.StartNew(() => instance.Increment());
				var task3 = Task.Factory.StartNew(() => instance.Increment());
				Task.WaitAll(task1, task2, task3);

				instance.Count.Should().Be(1);
			});
		}
	}
}
using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Performance
{
	public class MemoryUsageTest : TestCase
	{
		private ObjectWithMultipleParameters component;
		private IMbCacheFactory factory;

		public MemoryUsageTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectWithMultipleParameters>()
				.CacheMethod(x => x.Calculate(0, "", 0))
				.AsImplemented();
			factory = CacheBuilder.BuildFactory();
			component = factory.Create<ObjectWithMultipleParameters>();
		}

		[Test]
		public void MeasureMem()
		{
			const int uniqueCacheEntries = 10000;
			var memUsageAtStart = GC.GetTotalMemory(true);

			for (var i = 0; i < uniqueCacheEntries; i++)
			{
				component.Calculate(i, "", i);
			}
			var memUsage = GC.GetTotalMemory(true) - memUsageAtStart;
			var mbUsage = memUsage / 1024 / 1024;
			Console.WriteLine(mbUsage + "mb");
			mbUsage.Should().Be.LessThan(20);
		}

		[Test]
		public void MeasureMemAfterClear()
		{
			const int uniqueCacheEntries = 10000;
			var memUsageAtStart = GC.GetTotalMemory(true);

			for (var i = 0; i < uniqueCacheEntries; i++)
			{
				component.Calculate(i, "", i);
			}
			factory.Invalidate(component);
			var memUsage = GC.GetTotalMemory(true) - memUsageAtStart;
			var mbUsage = memUsage / 1024 / 1024;
			Console.WriteLine(mbUsage + "mb");
			mbUsage.Should().Be.LessThan(2);
		}
	}
}
using System;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Performance
{
	public class MemoryUsageTest : TestCase
	{
		private ObjectWithMultipleParameters component;

		public MemoryUsageTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectWithMultipleParameters>()
				.CacheMethod(x => x.Calculate(0, "", 0))
				.AsImplemented();
			component = CacheBuilder.BuildFactory().Create<ObjectWithMultipleParameters>();
		}

		[Test]
		public void ShouldLowerMemoryUsageCausedByKeyString()
		{
			const int uniqueCacheEntries = 10000;
			var memUsageAtStart = GC.GetTotalMemory(true);

			for (var i = 0; i < uniqueCacheEntries; i++)
			{
				component.Calculate(i, "", i);
			}
			var memUsage = GC.GetTotalMemory(true) - memUsageAtStart;
			var mbUsage = memUsage / 1000 / 1000;
			Console.WriteLine(mbUsage + "mb");
			mbUsage.Should().Be.LessThan(22);
		}
	}
}
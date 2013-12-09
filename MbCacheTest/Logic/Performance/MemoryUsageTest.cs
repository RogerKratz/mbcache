using System;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.Performance
{
	[TestFixture]
	public class MemoryUsageTest : FullTest
	{
		private ObjectWithParameterReturningFixedValue component;

		public MemoryUsageTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectWithParameterReturningFixedValue>("Foo")
				.CacheMethod(x => x.DoIt(0))
				.AsImplemented();
			component = CacheBuilder.BuildFactory().Create<ObjectWithParameterReturningFixedValue>();
		}

		[Test, Explicit]
		public void ShouldLowerMemoryUsageCausedByKeyString()
		{
			const int uniqueCacheEntries = 10000;
			var memUsageAtStart = GC.GetTotalMemory(true);
			using (new NoLogger())
			{
				for (var i = 0; i < uniqueCacheEntries; i++)
				{
					component.DoIt(i);
				}				
			}
			var memUsage = GC.GetTotalMemory(true) - memUsageAtStart;
			//should be no more than 19mb
			Console.WriteLine(memUsage / 1000 / 1000 + "mb");
		}
	}
}
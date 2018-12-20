using System;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Performance
{
	public class MemoryUsageWithLargeParameterTest : TestCase
	{
		private IObjectWithParametersOnCachedMethod component;

		public MemoryUsageWithLargeParameterTest(Type proxyType) : base(proxyType)
		{
		}
		
		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectWithParametersOnCachedMethod>()
				.CacheMethod(x => x.CachedMethod(null))
				.As<IObjectWithParametersOnCachedMethod>();
			component = CacheBuilder.BuildFactory().Create<IObjectWithParametersOnCachedMethod>();
		}
		
		[Test]
		public void MeasureMem()
		{
			const int uniqueCacheEntries = 10000;
			var memUsageAtStart = GC.GetTotalMemory(true);

			for (var i = 0; i < uniqueCacheEntries; i++)
			{
				component.CachedMethod(new parameter());
			}
			var memUsage = GC.GetTotalMemory(true) - memUsageAtStart;
			var mbUsage = memUsage / 1000 / 1000;
			Console.WriteLine(mbUsage + "mb");
			mbUsage.Should().Be.LessThan(21);
		}

		private class parameter
		{
			public parameter()
			{
				LargeState = new long[10000];
			}
			
			public long[] LargeState { get; }
			
			public override string ToString()
			{
				return Guid.NewGuid().ToString();
			}
		}
	}
}
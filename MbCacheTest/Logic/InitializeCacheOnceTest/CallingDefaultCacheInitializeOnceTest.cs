using System;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.InitializeCacheOnceTest
{
	public class CallingDefaultCacheInitializeOnceTest : TestCase
	{
		private CacheWithInitializeCounter defaultCache;

		public CallingDefaultCacheInitializeOnceTest(Type proxyType) : base(proxyType)
		{
		}
		
		protected override void TestSetup()
		{
			defaultCache = new CacheWithInitializeCounter();
			CacheBuilder.SetCache(defaultCache)
				.For<ReturningRandomNumbers>()
				.CacheMethod(c => c.CachedNumber())
				.As<IReturningRandomNumbers>()
				.For<ObjectReturningNewGuids>()
				.CacheMethod(c => c.CachedMethod())
				.As<IObjectReturningNewGuids>()
				.BuildFactory();
		}

		[Test]
		public void ShouldOnlyInitializeOnce()
		{
			defaultCache.InitializeCounter
				.Should().Be.EqualTo(1);
		}
	}
}
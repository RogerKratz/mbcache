using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Wrap
{
	public class InterfaceComponentTest : TestCase
	{
		private IMbCacheFactory factory;

		public InterfaceComponentTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ReturningRandomNumbers>()
				 .CacheMethod(c => c.CachedNumber())
				 .CacheMethod(c => c.CachedNumber2())
				 .As<IReturningRandomNumbers>();

			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldWrapUncachingComponent()
		{
			var uncached = new ReturningRandomNumbers();
			uncached.CachedNumber().Should().Not.Be.EqualTo(uncached.CachedNumber());

			var cached = factory.ToCachedComponent<IReturningRandomNumbers>(uncached);
			cached.CachedNumber().Should().Be.EqualTo(cached.CachedNumber());
		}
	}
}
using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.ExplicitCacheKeyForType
{
	public class SimpleCacheTest : FullTest
	{
		private IMbCacheFactory factory;

		public SimpleCacheTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ReturningRandomNumbers>("someKey")
				 .CacheMethod(c => c.CachedNumber())
				 .CacheMethod(c => c.CachedNumber2())
				 .As<IReturningRandomNumbers>();

			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void VerifyCacheIsWorking()
		{
			var obj1 = factory.Create<IReturningRandomNumbers>();
			var obj2 = factory.Create<IReturningRandomNumbers>();
			obj1.CachedNumber().Should().Be.EqualTo(obj2.CachedNumber());
			obj1.CachedNumber2().Should().Be.EqualTo(obj2.CachedNumber2());
		}
	}
}
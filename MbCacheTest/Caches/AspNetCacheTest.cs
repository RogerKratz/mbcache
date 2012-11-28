using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Caches
{
	public class AspNetCacheTest : FullTest
	{
		private IMbCacheFactory factory;

		public AspNetCacheTest(string proxyTypeString) : base(proxyTypeString) { }

		protected override ICache CreateCache()
		{
			return new InMemoryCache(1);
		}

		protected override void TestSetup()
		{
			CacheBuilder
				 .For<ObjectReturningNewGuids>()
				 .CacheMethod(c => c.CachedMethod())
				 .As<IObjectReturningNewGuids>();

			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void VerifyCache()
		{
			var value = factory.Create<IObjectReturningNewGuids>().CachedMethod();
			Assert.AreEqual(value, factory.Create<IObjectReturningNewGuids>().CachedMethod());
		}

		[Test]
		public void CanInvalidateInvalidatedEntry()
		{
			var value = factory.Create<IObjectReturningNewGuids>().CachedMethod();
			factory.Invalidate<IObjectReturningNewGuids>();
			factory.Invalidate<IObjectReturningNewGuids>();
			Assert.AreNotEqual(value, factory.Create<IObjectReturningNewGuids>().CachedMethod());
		}
	}
}
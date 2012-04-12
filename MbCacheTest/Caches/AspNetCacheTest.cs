using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Caches
{
	[TestFixture]
	public class AspNetCacheTest
	{
		private IMbCacheFactory factory;

		[SetUp]
		public void Setup()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new AspNetCache(1), new ToStringMbCacheKey());
			builder
				 .For<ObjectReturningNewGuids>()
				 .CacheMethod(c => c.CachedMethod())
				 .As<IObjectReturningNewGuids>();

			factory = builder.BuildFactory();
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

		[Test]
		public void VerifyCacheWhenLockObjectGeneratorIsUsed()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new AspNetCache(1, new DefaultLockObjectGenerator(40)), new ToStringMbCacheKey());
			builder
				 .For<ObjectReturningNewGuids>()
				 .CacheMethod(c => c.CachedMethod())
				 .As<IObjectReturningNewGuids>();

			factory = builder.BuildFactory();

			var value = factory.Create<IObjectReturningNewGuids>().CachedMethod();
			Assert.AreEqual(value, factory.Create<IObjectReturningNewGuids>().CachedMethod());
		}
	}
}
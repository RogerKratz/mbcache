using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Caches
{
	public class LockObjectGeneratorTest : FullTest
	{
		public LockObjectGeneratorTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override ILockObjectGenerator CreateLockObjectGenerator()
		{
			return new FixedNumberOfLockObjects(40);
		}

		protected override ICache CreateCache()
		{
			return new InMemoryCache(1);
		}

		[Test]
		public void VerifyCacheWhenLockObjectGeneratorIsUsed()
		{
			CacheBuilder
				 .For<ObjectReturningNewGuids>()
				 .CacheMethod(c => c.CachedMethod())
				 .As<IObjectReturningNewGuids>();

			var factory = CacheBuilder.BuildFactory();

			var value = factory.Create<IObjectReturningNewGuids>().CachedMethod();
			Assert.AreEqual(value, factory.Create<IObjectReturningNewGuids>().CachedMethod());
		}
	}
}
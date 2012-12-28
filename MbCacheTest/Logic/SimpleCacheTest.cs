using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
	public class SimpleCacheInterfaceTest : FullTest
	{
		private IMbCacheFactory factory;

		public SimpleCacheInterfaceTest(string proxyTypeString) : base(proxyTypeString) { }

		protected override void TestSetup()
		{
			CacheBuilder.For<ReturningRandomNumbers>()
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
			Assert.AreEqual(obj1.CachedNumber(), obj2.CachedNumber());
			Assert.AreEqual(obj1.CachedNumber2(), obj2.CachedNumber2());
			Assert.AreNotEqual(obj1.CachedNumber(), obj1.CachedNumber2());
			Assert.AreNotEqual(obj1.NonCachedNumber(), obj2.NonCachedNumber());
		}
	}
}
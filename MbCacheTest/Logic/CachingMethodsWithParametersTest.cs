using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
	public class CachingMethodsWithParametersTest : FullTest
	{
		private IMbCacheFactory factory;

		public CachingMethodsWithParametersTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void VerifySameParameterGivesCacheHit()
		{
			Assert.AreEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("hej"),
								 factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("hej"));
		}

		[Test]
		public void VerifyDifferentParameterGivesNoCacheHit()
		{
			Assert.AreNotEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("roger"),
								 factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
		}

		[Test]
		public void NullAsParameter()
		{
			Assert.AreNotEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null),
				 factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
			Assert.AreEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null),
				 factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null));
		}

		[Test]
		public void InvalidateOnTypeWorks()
		{
			var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
			var value = obj.CachedMethod("hej");
			factory.Invalidate<IObjectWithParametersOnCachedMethod>();
			Assert.AreNotEqual(value, obj.CachedMethod("hej"));
		}

		[Test]
		public void InvalidateOnInstanceWorks()
		{
			var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
			var value = obj.CachedMethod("hej");
			factory.Invalidate(obj);
			Assert.AreNotEqual(value, obj.CachedMethod("hej"));
		}
	}
}
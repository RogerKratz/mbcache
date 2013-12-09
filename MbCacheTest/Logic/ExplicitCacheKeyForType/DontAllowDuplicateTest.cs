using System;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.ExplicitCacheKeyForType
{
	public class DontAllowDuplicateTest : FullTest
	{
		public DontAllowDuplicateTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		[Test]
		public void ShouldThrowIfMultiple()
		{
			const string key = "aklsdjf";
			CacheBuilder.For<ObjectReturningNewGuids>(key)
			            .CacheMethod(x => x.CachedMethod())
			            .As<IObjectReturningNewGuids>();
      CacheBuilder.For<ObjectReturningNewGuidsNoInterface>(key)
                  .CacheMethod(x => x.CachedMethod())
                  .AsImplemented();
			Assert.Throws<InvalidOperationException>(() =>
			              CacheBuilder.BuildFactory());
		}
	}
}
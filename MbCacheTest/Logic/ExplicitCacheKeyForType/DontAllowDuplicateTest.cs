using System;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.ExplicitCacheKeyForType
{
	public class DontAllowDuplicateTest : TestCase
	{
		public DontAllowDuplicateTest(Type proxyType) : base(proxyType)
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
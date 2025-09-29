using System;
using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.ExplicitCacheKeyForType;

public class DontAllowDuplicateTest
{
	[Test]
	public void ShouldThrowIfMultiple()
	{
		const string key = "aklsdjf";
		var builder = new CacheBuilder()
			.For<ObjectReturningNewGuids>(key)
				.CacheMethod(x => x.CachedMethod())
				.As<IObjectReturningNewGuids>()
			.For<ObjectReturningNewGuidsNoInterface>(key)
				.CacheMethod(x => x.CachedMethod())
				.AsImplemented();
		Assert.Throws<InvalidOperationException>(() =>
			builder.BuildFactory());
	}
}
using System;
using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic;

public class CachePropertyTest
{
	[Test]
	public void ShouldThrowIfPropertyIsCached()
	{
		Assert.Throws<ArgumentException>(() =>
			new CacheBuilder().For<ObjectWithProperty>()
				.CacheMethod(c => c.Thing)
				.AsImplemented());
	}
}
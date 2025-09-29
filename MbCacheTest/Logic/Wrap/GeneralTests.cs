using System;
using MbCache.Configuration;
using NUnit.Framework;

namespace MbCacheTest.Logic.Wrap;

public class GeneralTests
{
	[Test]
	public void ShouldThrowIfNonCachedComponentIsUsed()
	{
		var factory = new CacheBuilder().BuildFactory();
		Assert.Throws<ArgumentException>(() => factory.ToCachedComponent(new object()));
	}
}
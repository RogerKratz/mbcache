using System;
using MbCache.Configuration;
using NUnit.Framework;

namespace MbCacheTest.Configuration;

public class UnknownComponentTest
{
	[Test]
	public void ShouldThrowWhenCreatingNotKnownComponent()
	{
		var factory = new CacheBuilder().BuildFactory();
		Assert.Throws<ArgumentException>(() => factory.Create<object>());
	}

	[Test]
	public void ShouldThrowWhenInvalidatingNotKnownComponent()
	{
		var factory = new CacheBuilder().BuildFactory();
		Assert.Throws<ArgumentException>(() => factory.Invalidate<object>());
	}
}
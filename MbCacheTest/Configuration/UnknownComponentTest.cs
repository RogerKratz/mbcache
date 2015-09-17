using System;
using MbCache.Configuration;
using NUnit.Framework;

namespace MbCacheTest.Configuration
{
	public class UnknownComponentTest
	{
		[Test]
		public void ShouldThrowWhenCreatingNotKnownComponent()
		{
			var builder = new CacheBuilder(new ProxyImplThatThrowsNotSupportedEx());
			var factory = builder.BuildFactory();
			Assert.Throws<ArgumentException>(() => factory.Create<object>());
		}

		[Test]
		public void ShouldThrowWhenInvalidatingNotKnownComponent()
		{
			var builder = new CacheBuilder(new ProxyImplThatThrowsNotSupportedEx());
			var factory = builder.BuildFactory();
			Assert.Throws<ArgumentException>(() => factory.Invalidate<object>());
		}
	}
}
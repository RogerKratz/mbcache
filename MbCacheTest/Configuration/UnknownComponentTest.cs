using System;
using NUnit.Framework;

namespace MbCacheTest.Configuration
{
	public class UnknownComponentTest : TestCase
	{
		[Test]
		public void ShouldThrowWhenCreatingNotKnownComponent()
		{
			var factory = CacheBuilder.BuildFactory();
			Assert.Throws<ArgumentException>(() => factory.Create<object>());
		}

		[Test]
		public void ShouldThrowWhenInvalidatingNotKnownComponent()
		{
			var factory = CacheBuilder.BuildFactory();
			Assert.Throws<ArgumentException>(() => factory.Invalidate<object>());
		}

		public UnknownComponentTest(Type proxyType) : base(proxyType)
		{
		}
	}
}
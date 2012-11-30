using System;
using MbCache.Configuration;
using NUnit.Framework;

namespace MbCacheTest.Configuration
{
	public class UnknownComponentTest
	{
		[Test]
		public void ShouldThrowArgumentException()
		{
			var builder = new CacheBuilder(new NullProxyFactory(), new ToStringCacheKey());
			var factory = builder.BuildFactory();
			Assert.Throws<ArgumentException>(() =>
			                                 factory.Create<object>());
		}
	}
}
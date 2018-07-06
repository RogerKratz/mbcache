using System;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
	public class CachePropertyTest : TestCase
	{
		public CachePropertyTest(Type proxyType) : base(proxyType)
		{
		}

		[Test]
		public void ShouldThrowIfPropertyIsCached()
		{
			Assert.Throws<ArgumentException>(() =>
			      CacheBuilder.For<ObjectWithProperty>()
			         .CacheMethod(c => c.Thing)
			         .AsImplemented());
		}
	}
}
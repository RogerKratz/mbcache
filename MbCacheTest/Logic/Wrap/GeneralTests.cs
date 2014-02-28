using System;
using MbCache.Core;
using NUnit.Framework;

namespace MbCacheTest.Logic.Wrap
{
	public class GeneralTests : FullTest
	{
		private IMbCacheFactory factory;

		public GeneralTests(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldThrowIfNonCachedComponentIsUsed()
		{
			Assert.Throws<ArgumentException>(() => factory.ToCachedComponent(new object()));
		}
	}
}
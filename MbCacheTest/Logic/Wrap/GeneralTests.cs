using System;
using MbCache.Core;
using NUnit.Framework;

namespace MbCacheTest.Logic.Wrap;

public class GeneralTests : TestCase
{
	private IMbCacheFactory factory;

	public GeneralTests(Type proxyType) : base(proxyType)
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
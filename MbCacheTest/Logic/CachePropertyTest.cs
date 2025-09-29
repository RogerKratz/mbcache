﻿using System;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic;

public class CachePropertyTest : TestCase
{
	[Test]
	public void ShouldThrowIfPropertyIsCached()
	{
		Assert.Throws<ArgumentException>(() =>
			CacheBuilder.For<ObjectWithProperty>()
				.CacheMethod(c => c.Thing)
				.AsImplemented());
	}
}
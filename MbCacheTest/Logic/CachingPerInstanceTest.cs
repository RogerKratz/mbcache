using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic;

public class CachingPerInstanceTest : TestCase
{
	private IMbCacheFactory factory;


	protected override void TestSetup()
	{
		CacheBuilder
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.PerInstance()
			.As<IObjectReturningNewGuids>();

		factory = CacheBuilder.BuildFactory();
	}


	[Test]
	public void DifferentObjectsHasTheirOwnCache()
	{
		var obj = factory.Create<IObjectReturningNewGuids>();
		var obj2 = factory.Create<IObjectReturningNewGuids>();

		Assert.AreEqual(obj.CachedMethod(), obj.CachedMethod());
		Assert.AreNotEqual(obj.CachedMethod(), obj2.CachedMethod());
	}
}
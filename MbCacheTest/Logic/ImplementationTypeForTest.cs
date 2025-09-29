using System;
using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class ImplementationTypeForTest
{
	[Test]
	public void ShouldReturnCorrectTypeForInterface()
	{
		var factory = new CacheBuilder()
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.As<IObjectReturningNewGuids>()
			.BuildFactory();
		factory.ImplementationTypeFor(typeof(IObjectReturningNewGuids))
			.Should().Be.EqualTo<ObjectReturningNewGuids>();
	}

	[Test] 
	public void ShouldReturnCorrectTypeForClass()
	{
		var factory = new CacheBuilder()
			.For<ObjectWithNonInterfaceMethod>()
			.CacheMethod(c => c.ReturnsFour())
			.AsImplemented()
			.BuildFactory();
		factory.ImplementationTypeFor(typeof(ObjectWithNonInterfaceMethod))
			.Should().Be.EqualTo<ObjectWithNonInterfaceMethod>();
	}

	[Test]
	public void ShouldThrowIfUnknownType()
	{
		var factory = new CacheBuilder()
			.For<ObjectWithNonInterfaceMethod>()
			.CacheMethod(c => c.ReturnsFour())
			.AsImplemented()
			.BuildFactory();
		Assert.Throws<ArgumentException>(() =>
			factory.ImplementationTypeFor(typeof(IObjectReturningNewGuids))
		);
	}
}
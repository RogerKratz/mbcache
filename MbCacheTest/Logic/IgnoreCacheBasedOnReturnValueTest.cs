using System;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class IgnoreCacheBasedOnReturnValueTest : TestCase
{
	[Test]
	public void ShouldNotCache()
	{
		CacheBuilder
			.For<ObjectReturningOne>()
			.CacheMethod(c => c.Return1(), returnValuesNotToCache: new[]{1})
			.AsImplemented();
		var factory = CacheBuilder.BuildFactory();
		var obj = factory.Create<ObjectReturningOne>();

		obj.Return1();
		obj.Return1();
		obj.Return1();

		obj.NumberOfExecutions.Should().Be.EqualTo(3);
	}
		
	[Test]
	public void ShouldCacheIfAnotherReturnValue()
	{
		CacheBuilder
			.For<ObjectReturningOne>()
			.CacheMethod(c => c.Return1(), returnValuesNotToCache: new[]{2})
			.AsImplemented();
		var factory = CacheBuilder.BuildFactory();
		var obj = factory.Create<ObjectReturningOne>();

		obj.Return1();
		obj.Return1();
		obj.Return1();

		obj.NumberOfExecutions.Should().Be.EqualTo(1);
	}
		
	[Test]
	public void ShouldCacheIfReturnValueIsOnAnotherMethod()
	{
		CacheBuilder
			.For<ObjectReturningOne>()
			.CacheMethod(c => c.Return1(), returnValuesNotToCache: new[]{2})
			.CacheMethod(c => c.Return2(), returnValuesNotToCache: new[]{1})
			.AsImplemented();
		var factory = CacheBuilder.BuildFactory();
		var obj = factory.Create<ObjectReturningOne>();

		obj.Return1();
		obj.Return1();
		obj.Return1();

		obj.NumberOfExecutions.Should().Be.EqualTo(1);
	}
}
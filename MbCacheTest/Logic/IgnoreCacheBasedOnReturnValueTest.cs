using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class IgnoreCacheBasedOnReturnValueTest
{
	[Test]
	public void ShouldNotCache()
	{
		var factory = new CacheBuilder()
			.For<ObjectReturningOne>()
			.CacheMethod(c => c.Return1(), returnValuesNotToCache: new[]{1})
			.AsImplemented()
			.BuildFactory();
		var obj = factory.Create<ObjectReturningOne>();

		obj.Return1();
		obj.Return1();
		obj.Return1();

		obj.NumberOfExecutions.Should().Be.EqualTo(3);
	}
		
	[Test]
	public void ShouldCacheIfAnotherReturnValue()
	{
		var factory = new CacheBuilder()
			.For<ObjectReturningOne>()
			.CacheMethod(c => c.Return1(), returnValuesNotToCache: new[]{2})
			.AsImplemented()
			.BuildFactory();
		var obj = factory.Create<ObjectReturningOne>();

		obj.Return1();
		obj.Return1();
		obj.Return1();

		obj.NumberOfExecutions.Should().Be.EqualTo(1);
	}
		
	[Test]
	public void ShouldCacheIfReturnValueIsOnAnotherMethod()
	{
		var factory = new CacheBuilder()
			.For<ObjectReturningOne>()
			.CacheMethod(c => c.Return1(), returnValuesNotToCache: new[]{2})
			.CacheMethod(c => c.Return2(), returnValuesNotToCache: new[]{1})
			.AsImplemented()
			.BuildFactory();
		var obj = factory.Create<ObjectReturningOne>();

		obj.Return1();
		obj.Return1();
		obj.Return1();

		obj.NumberOfExecutions.Should().Be.EqualTo(1);
	}
}
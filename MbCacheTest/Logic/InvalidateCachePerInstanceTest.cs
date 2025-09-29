using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class InvalidateCachePerInstanceTest : TestCase
{
	private IMbCacheFactory factory;
	private IObjectReturningNewGuids obj1;
	private IObjectReturningNewGuids obj2;

	public InvalidateCachePerInstanceTest(Type proxyType) : base(proxyType)
	{
	}

	protected override void TestSetup()
	{
		CacheBuilder
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.CacheMethod(c => c.CachedMethod2())
			.PerInstance()
			.As<IObjectReturningNewGuids>();

		CacheBuilder
			.For<ObjectWithParametersOnCachedMethod>()
			.CacheMethod(c => c.CachedMethod(null))
			.PerInstance()
			.As<IObjectWithParametersOnCachedMethod>();

		factory = CacheBuilder.BuildFactory();
		obj1 = factory.Create<IObjectReturningNewGuids>();
		obj2 = factory.Create<IObjectReturningNewGuids>();
	}

	[Test]
	public void InvalidateByInstance()
	{
		var value1 = obj1.CachedMethod();
		var value2 = obj2.CachedMethod();

		factory.Invalidate(obj1);

		value1.Should().Not.Be.EqualTo(obj1.CachedMethod());
		value2.Should().Be.EqualTo(obj2.CachedMethod());
	}

	[Test]
	public void InvalidateSpecificMethod()
	{
		var value1 = obj1.CachedMethod();
		var value2 = obj2.CachedMethod();

		factory.Invalidate(obj1, method => obj1.CachedMethod(), false);

		value1.Should().Not.Be.EqualTo(obj1.CachedMethod());
		value2.Should().Be.EqualTo(obj2.CachedMethod());
	}

	[Test]
	public void InvalidateSpecificMethodWithSpecificParameter()
	{
		var objWithParam = factory.Create<IObjectWithParametersOnCachedMethod>();
		var objWithParam2 = factory.Create<IObjectWithParametersOnCachedMethod>();
		var value1 = objWithParam.CachedMethod("roger");
		var value2 = objWithParam2.CachedMethod("roger");
		factory.Invalidate(objWithParam, method => method.CachedMethod("roger"), true);

		value1.Should().Not.Be.EqualTo(objWithParam.CachedMethod("roger"));
		value2.Should().Be.EqualTo(objWithParam2.CachedMethod("roger"));
	}

	[Test]
	public void InvalidateAllByExplicitCall()
	{
		var obj = factory.Create<IObjectReturningNewGuids>();
		var value = obj.CachedMethod();
		factory.Invalidate();
		obj.CachedMethod().Should().Not.Be.EqualTo(value);
	}

	[Test]
	public void InvalidateAllByDispose()
	{
		var obj = factory.Create<IObjectReturningNewGuids>();
		var value = obj.CachedMethod();
		factory.Dispose();
		var newFactory = CacheBuilder.BuildFactory();
		newFactory.Create<IObjectReturningNewGuids>().CachedMethod()
			.Should().Not.Be.EqualTo(value);
	}
}
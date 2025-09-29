using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class InvalidateCacheTest : TestCase
{
	private IMbCacheFactory factory;


	protected override void TestSetup()
	{
		CacheBuilder
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.CacheMethod(c => c.CachedMethod2())
			.As<IObjectReturningNewGuids>();

		CacheBuilder
			.For<ObjectWithParametersOnCachedMethod>()
			.CacheMethod(c => c.CachedMethod(null))
			.As<IObjectWithParametersOnCachedMethod>();

		factory = CacheBuilder.BuildFactory();
	}


	[Test]
	public void VerifyInvalidateByType()
	{
		var obj = factory.Create<IObjectReturningNewGuids>();
		var value1 = obj.CachedMethod();
		var value2 = obj.CachedMethod2();
		obj.CachedMethod().Should().Be.EqualTo(value1);
		obj.CachedMethod2().Should().Be.EqualTo(value2);
		value1.Should().Not.Be.EqualTo(value2);
		factory.Invalidate<IObjectReturningNewGuids>();
		obj.CachedMethod().Should().Not.Be.EqualTo(value1);
		obj.CachedMethod2().Should().Not.Be.EqualTo(value2);
	}

	[Test]
	public void InvalidatingNonCachingComponentThrows()
	{
		Assert.Throws<ArgumentException>(() => factory.Invalidate(3));
	}

	[Test]
	public void InvalidatingNonCachingComponentAndMethodThrows()
	{
		Assert.Throws<ArgumentException>(() => factory.Invalidate(3, theInt => theInt.CompareTo(44), false));
	}

	[Test]
	public void VerifyInvalidateByInstance()
	{
		var obj = factory.Create<IObjectReturningNewGuids>();
		var obj2 = factory.Create<IObjectReturningNewGuids>();
		var value1 = obj.CachedMethod();
		var value2 = obj2.CachedMethod2();

		obj.CachedMethod().Should().Be.EqualTo(value1);
		obj.CachedMethod2().Should().Be.EqualTo(value2);
		value1.Should().Not.Be.EqualTo(value2);
		factory.Invalidate(obj);
		obj.CachedMethod().Should().Not.Be.EqualTo(value1);
		obj.CachedMethod2().Should().Not.Be.EqualTo(value2);
	}


	[Test]
	public void InvalidateByCachingComponent()
	{
		var obj = factory.Create<IObjectReturningNewGuids>();
		var value = obj.CachedMethod();

		((ICachingComponent)obj).Invalidate();
		obj.CachedMethod().Should().Not.Be.EqualTo(value);
	}

	[Test]
	public void InvalidateSpecificMethod()
	{
		var obj = factory.Create<IObjectReturningNewGuids>();
		var value1 = obj.CachedMethod();
		var value2 = obj.CachedMethod2();

		obj.CachedMethod().Should().Be.EqualTo(value1);
		obj.CachedMethod2().Should().Be.EqualTo(value2);
		value1.Should().Not.Be.EqualTo(value2);
		factory.Invalidate(obj, method => obj.CachedMethod(), false);
		obj.CachedMethod().Should().Not.Be.EqualTo(value1);
		obj.CachedMethod2().Should().Be.EqualTo(value2);
	}

	[Test]
	public void InvalidateSpecificMethodWithSpecificArgumentsUsingNoArguments()
	{
		var obj = factory.Create<IObjectReturningNewGuids>();
		var value1 = obj.CachedMethod();
		var value2 = obj.CachedMethod2();
		
		obj.CachedMethod().Should().Be.EqualTo(value1);
		obj.CachedMethod2().Should().Be.EqualTo(value2);
		value1.Should().Not.Be.EqualTo(value2);
		factory.Invalidate(obj, method => obj.CachedMethod(), true);
		obj.CachedMethod().Should().Not.Be.EqualTo(value1);
		obj.CachedMethod2().Should().Be.EqualTo(value2);
	}

	[Test]
	public void InvalidateSpecificMethodWithSpecificParameter()
	{
		var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
		var value1 = obj.CachedMethod("roger");
		var value2 = obj.CachedMethod("moore");
		factory.Invalidate(obj, method => method.CachedMethod("roger"), true);

		value1.Should().Not.Be.EqualTo(obj.CachedMethod("roger"));
		value2.Should().Be.EqualTo(obj.CachedMethod("moore"));
	}

	[Test]
	public void InvalidateSpecificMethodWithSpecificParameterNoConstant()
	{
		var invalidParamObject = new ObjectImplToString(Guid.NewGuid());
		var anotherParamObject = new ObjectImplToString(Guid.NewGuid());

		var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
		var value1 = obj.CachedMethod(invalidParamObject);
		var value2 = obj.CachedMethod(anotherParamObject);
		factory.Invalidate(obj, method => method.CachedMethod(invalidParamObject), true);

		value1.Should().Not.Be.EqualTo(obj.CachedMethod(invalidParamObject));
		value2.Should().Be.EqualTo(obj.CachedMethod(anotherParamObject));
	}

	[Test]
	public void InvalidateSpecificMethodWithSpecificParameterNotUsed()
	{
		var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
		var value1 = obj.CachedMethod("roger");
		var value2 = obj.CachedMethod("moore");
		factory.Invalidate(obj, method => method.CachedMethod("roger"), false);

		value1.Should().Not.Be.EqualTo(obj.CachedMethod("roger"));
		value2.Should().Not.Be.EqualTo(obj.CachedMethod("moore"));
	}

	[Test]
	public void InvalidateSpecificMethodWithParameterBeforeUse()
	{
		const string parameter = "roger";
		var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
		factory.Invalidate(obj, method => method.CachedMethod(parameter), true);
		obj.CachedMethod(parameter).Should().Be.EqualTo(obj.CachedMethod(parameter));
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
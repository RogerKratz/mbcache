using System;
using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Configuration;

public class CacheBuilderTest
{
	[Test]
	public void OnlyDeclareTypeOnce()
	{
		var builder = new CacheBuilder()
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.As<IObjectReturningNewGuids>();
		Assert.Throws<ArgumentException>(()
			=>
			builder
				.For<ObjectReturningNewGuids>()
				.CacheMethod(c => c.CachedMethod2())
				.As<IObjectReturningNewGuids>());
	}

	[Test]
	public void ReturnTypeMustBeDeclared()
	{
		var builder = new CacheBuilder();
		builder
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod());
		Assert.Throws<InvalidOperationException>(() => builder.BuildFactory());
	}

	[Test]
	public void CreatingProxiesOfSameDeclaredTypeShouldReturnIdenticalTypes()
	{
		var factory = new CacheBuilder()
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.As<IObjectReturningNewGuids>()
			.BuildFactory();
		
		factory.Create<IObjectReturningNewGuids>().GetType()
			.Should().Be.EqualTo(factory.Create<IObjectReturningNewGuids>().GetType());
	}

	[Test]
	public void FactoryReturnsNewInterfaceInstances()
	{
		var factory = new CacheBuilder()
			.For<ObjectWithIdentifier>()
			.As<IObjectWithIdentifier>()
			.BuildFactory();
		var obj1 = factory.Create<IObjectWithIdentifier>();
		var obj2 = factory.Create<IObjectWithIdentifier>();
		obj1.Should().Not.Be.SameInstanceAs(obj2);
		obj1.Id.Should().Not.Be.EqualTo(obj2.Id);
	}

	[Test]
	public void CanConfigureMultipleComponents()
	{
		var factory = new CacheBuilder()
			.For<ObjectReturningNewGuids>()
			.CacheMethod(m => m.CachedMethod())
			.As<IObjectReturningNewGuids>()
			.For<ObjectReturningNewGuidsNoInterface>()
			.CacheMethod(m => m.CachedMethod())
			.AsImplemented()
			.BuildFactory();
		factory.Create<IObjectReturningNewGuids>().CachedMethod()
			.Should().Be.EqualTo(factory.Create<IObjectReturningNewGuids>().CachedMethod());
		factory.Create<ObjectReturningNewGuidsNoInterface>().CachedMethod()
			.Should().Be.EqualTo(factory.Create<ObjectReturningNewGuidsNoInterface>().CachedMethod());
	}
}
using System;
using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.ClassProxy;

public class NonVirtualTest
{
	[Test]
	public void ShouldWorkIfComponentIsInterface()
	{
		var factory = new CacheBuilder()
			.For<HasNonVirtualMethod>()
			.CacheMethod(m => m.Virtual())
			.As<IHasNonVirtualMethod>()
			.BuildFactory();

		var instance = factory.Create<IHasNonVirtualMethod>();
		instance.Virtual().Should().Be.EqualTo(instance.Virtual());
	}

	[Test]
	public void ShouldNotWorkIfComponentIsClass()
	{
		var fluentBuilder = new CacheBuilder()
			.For<HasNonVirtualMethod>()
			.CacheMethod(m => m.Virtual());
		Assert.Throws<InvalidOperationException>(() => fluentBuilder.AsImplemented());
			
			
		//the reason we prevent this all together is because issue #35 doesn't work, eg these asserts...
		// const int value = 17;
		// var instance = fluentBuilder.BuildFactory().Create<HasNonVirtualMethod>();
		// instance.SetInternalState(value);
		// instance.NonVirtualWithInternalState().Should().Be.EqualTo(value);
	}
}
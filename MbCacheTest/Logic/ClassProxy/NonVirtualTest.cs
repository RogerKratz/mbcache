using System;
using MbCache.ProxyImpl.Castle;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.ClassProxy
{
	public class NonVirtualTest : TestCase
	{
		public NonVirtualTest(Type proxyType) : base(proxyType)
		{
		}

		[Test]
		public void CachedMethodNeedToBeVirtual_AsImplemented()
		{
			var fluentBuilder = CacheBuilder
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.NonVirtual());
			Assert.Throws<InvalidOperationException>(() => fluentBuilder.AsImplemented());
		}
		
		[Test]
		public void CachedMethodNeedToBeVirtual_As()
		{
			var fluentBuilder = CacheBuilder
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.NonVirtual());
			Assert.Throws<InvalidOperationException>(() => fluentBuilder.As<HasNonVirtualMethod>());
		}


		[Test]
		public void TypeWithNonVirtualMethodShouldWork()
		{
			var fluentBuilder = CacheBuilder
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.Virtual())
				.AsImplemented();

			var instance = fluentBuilder.BuildFactory().Create<HasNonVirtualMethod>();

			instance.Virtual().Should().Be.EqualTo(instance.Virtual());
			instance.NonVirtual().Should().Not.Be.EqualTo(instance.NonVirtual());
		}

		[Test]
		public void NonVirtualMethodWithStateShouldWork()
		{
			if(!(ProxyFactory is CastleProxyFactory))
				Assert.Ignore("Needs to be fixed (or not allowing non virtual methods on class proxies at all?)");
			
			var fluentBuilder = CacheBuilder
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.Virtual())
				.AsImplemented();

			const int value = 17;
			var instance = fluentBuilder.BuildFactory().Create<HasNonVirtualMethod>();
			instance.SetInternalState(value);
			instance.NonVirtualWithInternalState().Should().Be.EqualTo(value);
		}
	}
}
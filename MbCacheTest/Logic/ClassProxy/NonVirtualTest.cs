using System;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.ClassProxy
{
	public class NonVirtualTest : FullTest
	{
		public NonVirtualTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		[Test]
		public void CachedMethodNeedToBeVirtual()
		{
			var fluentBuilder = CacheBuilder
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.NonVirtual());
			Assert.Throws<InvalidOperationException>(() => fluentBuilder.AsImplemented());
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
	}
}
using System;
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
		public void ShouldWorkIfComponentIsInterface()
		{
			var fluentBuilder = CacheBuilder
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.Virtual())
				.As<IHasNonVirtualMethod>();

			var instance = fluentBuilder.BuildFactory().Create<IHasNonVirtualMethod>();

			instance.Virtual().Should().Be.EqualTo(instance.Virtual());
		}

		[Test]
		public void ShouldNotWorkIfComponentIsClass()
		{
			var fluentBuilder = CacheBuilder
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
}
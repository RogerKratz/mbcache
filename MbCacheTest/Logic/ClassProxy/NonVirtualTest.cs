using System;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.ClassProxy
{
	public class NonVirtualTest : SimpleTest
	{
		[Test]
		public void CachedMethodNeedToBeVirtual()
		{
			var fluentBuilder = CacheBuilder
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.NonVirtual());
			Assert.Throws<InvalidOperationException>(() => fluentBuilder.AsImplemented());
		}

		[Test]
		public void NonVirtualNonCachedMethod()
		{
			var fluentBuilder = CacheBuilder
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.Virtual());

			if (ProxyFactory.AllowNonVirtualMember)
			{
				Assert.DoesNotThrow(() => fluentBuilder.AsImplemented());
			}
			else
			{
				Assert.Throws<InvalidOperationException>(() => fluentBuilder.AsImplemented());
			}
		}
	}
}
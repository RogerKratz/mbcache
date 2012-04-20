using System;
using MbCache.Configuration;
using MbCacheTest.CacheForTest;
using NUnit.Framework;

namespace MbCacheTest.Logic.ClassProxy
{
	[TestFixture]
	public class NonVirtualTest
	{
		[Test]
		public void CachedMethodNeedToBeVirtual()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey())
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.NonVirtual());
			Assert.Throws<InvalidOperationException>(builder.AsImplemented);
		}

		[Test]
		public void NonVirtualNonCachedMethod()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey())
				.For<HasNonVirtualMethod>()
				.CacheMethod(m => m.Virtual());

			if (ConfigurationData.ProxyFactory.AllowNonVirtualMember)
			{
				Assert.DoesNotThrow(builder.AsImplemented);
			}
			else
			{
				Assert.Throws<InvalidOperationException>(builder.AsImplemented);
			}
		}
	}

	public class HasNonVirtualMethod
	{
		public int NonVirtual()
		{
			return 0;
		}

		public virtual int Virtual()
		{
			return 0;
		}
	}
}
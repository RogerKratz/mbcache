﻿using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Generics
{
	public class GenericTypeTest : TestCase
	{
		private IMbCacheFactory factory;

		public GenericTypeTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			factory = CacheBuilder.For<ObjectOfGenericType<string>>()
				.CacheMethod(m => m.CachedMethod(1, "1"))
				.As<IObjectOfGenericType<string>>()
				.BuildFactory();
		}

		[Test]
		public void ShouldCache()
		{
			var component = factory.Create<IObjectOfGenericType<string>>();
			component.CachedMethod(1, "1")
				.Should().Be.EqualTo(component.CachedMethod(1, "1"));
		}

		[Test]
		public void ShouldNotCache()
		{
			var component = factory.Create<IObjectOfGenericType<string>>();
			component.CachedMethod(1, "1")
				.Should().Not.Be.EqualTo(component.CachedMethod(2, "1"));
		}
	}
}
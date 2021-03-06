﻿using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class CachingObjectWithParameterCtorInterfaceTest : TestCase
	{
		private IMbCacheFactory factory;

		public CachingObjectWithParameterCtorInterfaceTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder
				 .For<ObjectWithCtorParameters>()
				 .CacheMethod(c => c.CachedMethod())
				 .As<IObjectWithCtorParameters>();

			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void CanReadProps()
		{
			var obj = factory.Create<IObjectWithCtorParameters>(11, 12);
			Assert.AreEqual(11, obj.Value1);
			Assert.AreEqual(12, obj.Value2);
		}

		[Test]
		public void InvalidParametersThrows()
		{
			Assert.Throws<ArgumentException>(() => factory.Create<IObjectWithCtorParameters>());
		}

		[Test]
		public void VerifyCacheWorks()
		{
			Assert.AreEqual(factory.Create<IObjectWithCtorParameters>(4, 3).CachedMethod(),
								 factory.Create<IObjectWithCtorParameters>(4, 3).CachedMethod());
		}

		[Test]
		public void VerifyCacheDifferentParameters()
		{
			Assert.AreEqual(factory.Create<IObjectWithCtorParameters>(4, 4).CachedMethod(),
								 factory.Create<IObjectWithCtorParameters>(4, 3).CachedMethod());
		}

		[Test]
		public void CanWrapUncachingComponent()
		{
			var uncachingComponent = new ObjectWithCtorParameters(1, 1);
			var cachedComponent = factory.ToCachedComponent((IObjectWithCtorParameters)uncachingComponent);

			cachedComponent.CachedMethod()
				.Should().Be.EqualTo(cachedComponent.CachedMethod());
		}
	}
}

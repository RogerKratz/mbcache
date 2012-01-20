using System;
using System.Collections.Generic;
using System.Reflection;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	[TestFixture]
	public class MbCacheKeyReturnNullTest
	{
		[Test]
		public void ShouldNotAddToCache()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new mbCacheStub());

			builder
				 .For<ObjectReturningNewGuids>()
				 .CacheMethod(c => c.CachedMethod())
				 .As<IObjectReturningNewGuids>();

			var factory = builder.BuildFactory();

			var svc = factory.Create<IObjectReturningNewGuids>();
			svc.CachedMethod()
				.Should().Not.Be.EqualTo(svc.CachedMethod());
		}

		[Test]
		public void ShouldNotAddToCacheUsingParameters()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new mbCacheStub());

			builder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = builder.BuildFactory();

			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var obj = new object();
			svc.CachedMethod(obj)
				.Should().Not.Be.EqualTo(svc.CachedMethod(obj));
		}

		[Test]
		public void ShouldNotInvalidate()
		{
			var stub = new mbCacheStub {TheKey = "aKey"};
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), stub);

			builder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = builder.BuildFactory();

			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var parameter = new object();

			var result = svc.CachedMethod(parameter);

			stub.TheKey = null;
			factory.Invalidate<IObjectWithParametersOnCachedMethod>();
			factory.Invalidate(svc);
			factory.Invalidate(svc, method => method.CachedMethod(null), false);
			factory.Invalidate(svc, method => method.CachedMethod(parameter), true);
			stub.TheKey = "aKey";

			svc.CachedMethod(parameter).Should().Be.EqualTo(result);
		}

		private class mbCacheStub : IMbCacheKey
		{
			public string TheKey { get; set; }

			public string Key(Type type)
			{
				return TheKey;
			}

			public string Key(Type type, ICachingComponent component)
			{
				return TheKey;
			}

			public string Key(Type type, ICachingComponent component, MethodInfo method)
			{
				return TheKey;
			}

			public string Key(Type type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
			{
				return TheKey;
			}
		}
	}
}
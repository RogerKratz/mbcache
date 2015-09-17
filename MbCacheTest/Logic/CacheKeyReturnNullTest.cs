using System.Collections.Generic;
using System.Reflection;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class CacheKeyReturnNullTest : FullTest
	{
		private cacheKeyStub cacheKey;

		public CacheKeyReturnNullTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override ICacheKey CreateCacheKey()
		{
			cacheKey = new cacheKeyStub();
			return cacheKey;
		}

		[Test]
		public void ShouldNotAddToCache()
		{
			CacheBuilder
				 .For<ObjectReturningNewGuids>()
				 .CacheMethod(c => c.CachedMethod())
				 .As<IObjectReturningNewGuids>();

			var factory = CacheBuilder.BuildFactory();

			var svc = factory.Create<IObjectReturningNewGuids>();
			svc.CachedMethod()
				.Should().Not.Be.EqualTo(svc.CachedMethod());
		}

		[Test]
		public void ShouldNotAddToCacheUsingParameters()
		{
			CacheBuilder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = CacheBuilder.BuildFactory();

			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var obj = new object();
			svc.CachedMethod(obj)
				.Should().Not.Be.EqualTo(svc.CachedMethod(obj));
		}

		[Test]
		public void ShouldNotInvalidate()
		{
			cacheKey.TheKey = "aKey";
			CacheBuilder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = CacheBuilder.BuildFactory();

			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var parameter = new object();

			var result = svc.CachedMethod(parameter);

			cacheKey.TheKey = null;
			factory.Invalidate<IObjectWithParametersOnCachedMethod>();
			factory.Invalidate(svc);
			factory.Invalidate(svc, method => method.CachedMethod(null), false);
			factory.Invalidate(svc, method => method.CachedMethod(parameter), true);
			cacheKey.TheKey = "aKey";

			svc.CachedMethod(parameter).Should().Be.EqualTo(result);
		}

		private class cacheKeyStub : ICacheKey
		{
			public string TheKey { get; set; }

			public string RemoveKey(ComponentType type)
			{
				return TheKey;
			}

			public string RemoveKey(ComponentType type, ICachingComponent component)
			{
				return TheKey;
			}

			public string RemoveKey(ComponentType type, ICachingComponent component, MethodInfo method)
			{
				return TheKey;
			}

			public string RemoveKey(ComponentType type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
			{
				return TheKey;
			}

			public KeyAndItsDependingKeys GetAndPutKey(ComponentType type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
			{
				return new KeyAndItsDependingKeys(TheKey, () => new List<string>());
			}
		}
	}
}
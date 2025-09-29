using System;
using System.Reflection;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class CacheForComponentOverridingTest : TestCase
{
	private IMbCacheFactory factory;

	protected override ICache CreateCache()
	{
		return new cacheThatThrows();
	}

	protected override void TestSetup()
	{
		CacheBuilder
			.For<ReturningRandomNumbers>()
			.CacheMethod(c => c.CachedNumber())
			.OverrideCache(new InMemoryCache(TimeSpan.FromMinutes(1)))
			.As<IReturningRandomNumbers>()
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.As<IObjectReturningNewGuids>();
			

		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void ShouldUseDefault()
	{
		var instance = factory.Create<IObjectReturningNewGuids>();
		Assert.Throws<NotImplementedException>(() => { instance.CachedMethod(); });
	}

	[Test]
	public void ShouldUseOverriden()
	{
		var obj1 = factory.Create<IReturningRandomNumbers>();
		var obj2 = factory.Create<IReturningRandomNumbers>();
		obj1.CachedNumber().Should().Be.EqualTo(obj2.CachedNumber());
	}

	private class cacheThatThrows : ICache
	{
		public void Initialize(EventListenersCallback eventListenersCallback)
		{
		}

		public object GetAndPutIfNonExisting(KeyAndItsDependingKeys keyAndItsDependingKeys,
			MethodInfo cachedMethod, Func<OriginalMethodResult> originalMethod)
		{
			throw new NotImplementedException();
		}

		public void Delete(string cacheKey)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}
	}
}
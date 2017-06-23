using System;
using System.Globalization;
using System.Linq;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using Newtonsoft.Json;
using NUnit.Framework;
using Ploeh.AutoFixture;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class ToStringOrSerializedCacheKeyTest : FullTest
	{
		private static readonly IFixture auto = new Fixture();
		private EventListenerForTest eventListener;
		private IMbCacheFactory factory;

		public ToStringOrSerializedCacheKeyTest(Type proxyType) : base(proxyType)
		{
		}

		[Test]
		public void ShouldCacheWhenParameterWithSamePropertiesValue()
		{
			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var obj = auto.Create<ObjectWithComplexProperties>();
			var sameContentObj = DuplicateObj(obj);

			svc.CachedMethod(obj);

			svc.CachedMethod(sameContentObj);
			eventListener.CacheHits.Should().Not.Be.Empty();
		}

		[Test]
		public void ShouldNotCacheWhenDifferentParameterOfSameType()
		{
			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var obj = auto.Create<ObjectWithComplexProperties>();
			var differentObj = auto.Create<ObjectWithComplexProperties>();

			svc.CachedMethod(obj);

			svc.CachedMethod(differentObj);
			eventListener.CacheHits.Should().Be.Empty();
		}

		[Test]
		public void ShouldNotIssueTypeNameWarningWithThisCacheKey()
		{
			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var obj = auto.Create<ObjectWithComplexProperties>();

			svc.CachedMethod(obj);

			eventListener.Warnings.Should().Be.Empty();
		}

		[Test]
		public void ShouldUseSerializedDataWhenToStringIsTypeName()
		{
			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var obj = auto.Create<ObjectWithComplexProperties>();

			svc.CachedMethod(obj);

			svc.CachedMethod(obj);
			var cacheKey = AssertCacheHasBeenHitFor("CachedMethod");
			cacheKey.Contains(obj.DoubleProp.ToString(CultureInfo.InvariantCulture)).Should().Be.True();
			cacheKey.Contains(obj.StringProp).Should().Be.True();
			obj.GenericListProp.ForEach(x => cacheKey.Contains(x.Thing).Should().Be.True());
		}

		[Test]
		public void ShouldUseValueWhenValueTypeParameter()
		{
			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();

			svc.CachedMethod("123");

			svc.CachedMethod("123");
			var cacheKey = AssertCacheHasBeenHitFor("CachedMethod");
			Console.WriteLine(cacheKey);
			cacheKey.Contains("123").Should().Be.True();
		}

		protected override void TestSetup()
		{
			base.TestSetup();
			ConfigureEventListener();

			CacheBuilder
				.For<ObjectWithParametersOnCachedMethod>()
				.CacheMethod(c => c.CachedMethod(null))
				.CacheKey(new ToStringOrSerializedCacheKey())
				.As<IObjectWithParametersOnCachedMethod>();
			factory = CacheBuilder.BuildFactory();
		}

		private static ObjectWithComplexProperties DuplicateObj(ObjectWithComplexProperties obj)
		{
			return JsonConvert.DeserializeObject<ObjectWithComplexProperties>(JsonConvert.SerializeObject(obj));
		}

		private string AssertCacheHasBeenHitFor(string methodName)
		{
			eventListener.CacheHits.Should().Have.Count.EqualTo(1);
			var cachedItem = eventListener.CacheHits.First();
			cachedItem.EventInformation.Method.Name.Should().Be.EqualTo(methodName);
			return cachedItem.EventInformation.CacheKey;
		}

		private void ConfigureEventListener()
		{
			eventListener = new EventListenerForTest();
			CacheBuilder.AddEventListener(eventListener);
		}
	}
}
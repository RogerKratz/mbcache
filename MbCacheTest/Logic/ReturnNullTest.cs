using System;
using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class ReturnNullTest : TestCase
	{
		private IMbCacheFactory factory;
		private StatisticsEventListener eventListener;

		public ReturnNullTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			eventListener = new StatisticsEventListener();
			CacheBuilder
				.AddEventListener(eventListener)
				.For<ObjectReturningNull>()
					.CacheMethod(c => c.ReturnNullIfZero(0))
					.As<IObjectReturningNull>();
			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldCacheNullParameterAndReturnValue()
		{
			var component = factory.Create<IObjectReturningNull>();
			component.ReturnNullIfZero(0).Should().Be.Null();
			component.ReturnNullIfZero(0).Should().Be.Null();
			eventListener.CacheHits.Should().Be.EqualTo(1);
		}

		[Test]
		public void ShouldCacheNormalForNullableComponent()
		{
			var component = factory.Create<IObjectReturningNull>();
			component.ReturnNullIfZero(1).Should().Be.EqualTo(1);
			component.ReturnNullIfZero(1).Should().Be.EqualTo(1);
			eventListener.CacheHits.Should().Be.EqualTo(1);
		}
	}
}
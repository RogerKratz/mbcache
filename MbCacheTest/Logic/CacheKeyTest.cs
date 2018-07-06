using System;
using System.Linq;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class CacheKeyTest : TestCase
	{
		private IMbCacheFactory factory;
		private EventListenerForTest eventListener;

		public CacheKeyTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			eventListener = new EventListenerForTest();
			CacheBuilder.AddEventListener(eventListener);

			CacheBuilder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();
			
			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldGiveLogWarningIfSuspectedParameterIsUsed()
		{
			var comp = factory.Create<IObjectWithParametersOnCachedMethod>();
			comp.CachedMethod(this);

			eventListener.Warnings.Single().Should().Contain(GetType().ToString());
		}
	}
}
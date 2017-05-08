using System;
using System.Collections.Generic;
using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Events.Statistic
{
	public class GetTest : FullTest
	{
		private eventListenerForGet eventListener;

		public GetTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			eventListener = new eventListenerForGet();
			CacheBuilder
				.For<ObjectReturningNull>()
					.CacheMethod(c => c.ReturnNullIfZero(0))
					.As<IObjectReturningNull>()
				.AddEventListener(eventListener);

			var factory = CacheBuilder.BuildFactory();
			var comp = factory.Create<IObjectReturningNull>();
			comp.ReturnNullIfZero(0);
			comp.ReturnNullIfZero(1);
			comp.ReturnNullIfZero(0);
			comp.ReturnNullIfZero(1);
		}

		[Test]
		public void ShouldBeCalledCorrectNumberOfTimes()
		{
			eventListener.Successful.Count.Should().Be.EqualTo(4);
		}

		[Test]
		public void ShouldHaveCorrectSuccessCacheGets()
		{
			eventListener.Successful[0].Should().Be.False();
			eventListener.Successful[1].Should().Be.False();
			eventListener.Successful[2].Should().Be.True();
			eventListener.Successful[3].Should().Be.True();
		}

		[Test]
		public void ShouldHaveCorrectCachedValues()
		{
			eventListener.CachedItems[0].CachedValue.Should().Be.Null();
			eventListener.CachedItems[1].CachedValue.Should().Be.EqualTo(1);
		}

		[Test]
		public void ShouldHaveCorrectCacheKeys()
		{
			eventListener.CachedItems[0].EventInformation.CacheKey.Should().Contain("|$0");
			eventListener.CachedItems[1].EventInformation.CacheKey.Should().Contain("|$1");
			eventListener.CachedItems[2].EventInformation.CacheKey.Should().Contain("|$0");
			eventListener.CachedItems[3].EventInformation.CacheKey.Should().Contain("|$1");
		}

		[Test]
		public void ShouldHaveCorrectMethodInfo()
		{
			eventListener.CachedItems[0].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
			eventListener.CachedItems[1].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
			eventListener.CachedItems[2].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
			eventListener.CachedItems[3].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
		}

		[Test]
		public void ShouldHaveCorrectType()
		{
			eventListener.CachedItems[0].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
			eventListener.CachedItems[1].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
			eventListener.CachedItems[2].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
			eventListener.CachedItems[3].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
		}

		[Test]
		public void ShouldHaveCorrectArguments()
		{
			eventListener.CachedItems[0].EventInformation.Arguments.Should().Have.SameSequenceAs(0);
			eventListener.CachedItems[1].EventInformation.Arguments.Should().Have.SameSequenceAs(1);
			eventListener.CachedItems[2].EventInformation.Arguments.Should().Have.SameSequenceAs(0);
			eventListener.CachedItems[3].EventInformation.Arguments.Should().Have.SameSequenceAs(1);
		}

		private class eventListenerForGet : IEventListener
		{
			public readonly IList<CachedItem> CachedItems = new List<CachedItem>();
			public readonly IList<bool> Successful = new List<bool>();


			public void OnCacheHit(CachedItem cachedItem)
			{
				Successful.Add(true);
				CachedItems.Add(cachedItem);
			}

			public void OnCacheRemoval(CachedItem cachedItem)
			{
			}

			public void OnCacheMiss(CachedItem cachedItem)
			{
				Successful.Add(false);
				CachedItems.Add(cachedItem);
			}

			public void Warning(string warnMessage)
			{
				throw new NotImplementedException();
			}
		}
	}
}
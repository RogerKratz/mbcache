using System.Collections.Generic;
using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Events.Statistic
{
	public abstract class DeleteTestBase : FullTest
	{
		protected EventListenerForDelete EventListener;

		protected DeleteTestBase(string proxyTypeString) : base(proxyTypeString)
		{
		}

		[Test]
		public void ShouldBeCalledTwice()
		{
			EventListener.CachedItems.Count.Should().Be.EqualTo(2);
		}

		[Test]
		public void ShouldHaveCorrectValue()
		{
			EventListener.CachedItems[0].CachedValue.Should().Be.Null();
			EventListener.CachedItems[1].CachedValue.Should().Be.EqualTo(1);
		}

		[Test]
		public void ShouldHaveCorrectCacheKeys()
		{
			EventListener.CachedItems[0].EventInformation.CacheKey.Should().EndWith("|0");
			EventListener.CachedItems[1].EventInformation.CacheKey.Should().EndWith("|1");
		}

		[Test]
		public void ShouldHaveCorrectMethodInfo()
		{
			EventListener.CachedItems[0].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
			EventListener.CachedItems[1].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
		}

		[Test]
		public void ShouldHaveCorrectType()
		{
			EventListener.CachedItems[0].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
			EventListener.CachedItems[1].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
		}

		[Test]
		public void ShouldHaveCorrectArguments()
		{
			EventListener.CachedItems[0].EventInformation.Arguments.Should().Have.SameSequenceAs(0);
			EventListener.CachedItems[1].EventInformation.Arguments.Should().Have.SameSequenceAs(1);
		}

		protected class EventListenerForDelete : IEventListener
		{
			public readonly IList<CachedItem> CachedItems = new List<CachedItem>();

			public void OnGetUnsuccessful(EventInformation eventInformation)
			{
			}

			public void OnGetSuccessful(CachedItem cachedItem)
			{
			}

			public void OnDelete(CachedItem cachedItem)
			{
				CachedItems.Add(cachedItem);
			}

			public void OnPut(CachedItem cachedItem)
			{
			}
		}
	}
}
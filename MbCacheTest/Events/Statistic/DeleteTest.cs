using System.Collections.Generic;
using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Events.Statistic
{
	public class DeleteTest : FullTest
	{
		private eventListenerForDelete eventListener;

		public DeleteTest(string proxyTypeString)
			: base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			eventListener = new eventListenerForDelete();
			CacheBuilder
				.For<ObjectReturningNull>()
					.CacheMethod(c => c.ReturnNullIfZero(0))
					.As<IObjectReturningNull>()
				.AddEventListener(eventListener);

			var factory = CacheBuilder.BuildFactory();
			var comp = factory.Create<IObjectReturningNull>();
			comp.ReturnNullIfZero(0);
			comp.ReturnNullIfZero(1);
			factory.Invalidate(comp, c => c.ReturnNullIfZero(0), true);
			factory.Invalidate(comp, c => c.ReturnNullIfZero(1), true);
		}

		[Test]
		public void ShouldBeCalledTwice()
		{
			eventListener.CachedItems.Count.Should().Be.EqualTo(2);
		}

		[Test]
		public void ShouldHaveCorrectValue()
		{
			eventListener.CachedItems[0].CachedValue.Should().Be.Null();
			eventListener.CachedItems[1].CachedValue.Should().Be.EqualTo(1);
		}

		[Test]
		public void ShouldHaveCorrectCacheKeys()
		{
			eventListener.CachedItems[0].EventInformation.CacheKey.Should().EndWith("|0");
			eventListener.CachedItems[1].EventInformation.CacheKey.Should().EndWith("|1");
		}

		[Test]
		public void ShouldHaveCorrectMethodInfo()
		{
			eventListener.CachedItems[0].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
			eventListener.CachedItems[1].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
		}

		[Test]
		public void ShouldHaveCorrectType()
		{
			eventListener.CachedItems[0].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
			eventListener.CachedItems[1].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
		}

		[Test]
		public void ShouldHaveCorrectArguments()
		{
			eventListener.CachedItems[0].EventInformation.Arguments.Should().Have.SameSequenceAs(0);
			eventListener.CachedItems[1].EventInformation.Arguments.Should().Have.SameSequenceAs(1);
		}

		private class eventListenerForDelete : IEventListener
		{
			public readonly IList<CachedItem> CachedItems = new List<CachedItem>();

			public void OnGet(CachedItem cachedItem)
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
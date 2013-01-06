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
			eventListener.EventInformations.Count.Should().Be.EqualTo(2);
		}

		[Test]
		public void ShouldHaveCorrectCacheKeys()
		{
			eventListener.EventInformations[0].CacheKey.Should().EndWith("|0");
			eventListener.EventInformations[1].CacheKey.Should().EndWith("|1");
		}

		[Test]
		public void ShouldHaveCorrectMethodInfo()
		{
			eventListener.EventInformations[0].Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
			eventListener.EventInformations[1].Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
		}

		[Test]
		public void ShouldHaveCorrectType()
		{
			eventListener.EventInformations[0].Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
			eventListener.EventInformations[1].Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
		}

		[Test]
		public void ShouldHaveCorrectArguments()
		{
			eventListener.EventInformations[0].Arguments.Should().Have.SameSequenceAs(0);
			eventListener.EventInformations[1].Arguments.Should().Have.SameSequenceAs(1);
		}

		private class eventListenerForDelete : IEventListener
		{
			public readonly IList<EventInformation> EventInformations = new List<EventInformation>();

			public void OnGet(CachedItem cachedItem)
			{
			}

			public void OnDelete(EventInformation info)
			{
				EventInformations.Add(info);
			}

			public void OnPut(CachedItem cachedItem)
			{
			}
		}
	}
}
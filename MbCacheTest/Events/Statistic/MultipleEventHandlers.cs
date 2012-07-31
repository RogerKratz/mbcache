using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Events.Statistic
{
	public class MultipleEventHandlers : FullTest
	{
		private eventListenerForMultiple firstEventListener;
		private eventListenerForMultiple secondEventListener;
		private eventListenerForMultiple thirdEventListener;

		public MultipleEventHandlers(string proxyTypeString)
			: base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			firstEventListener = new eventListenerForMultiple();
			secondEventListener = new eventListenerForMultiple();
			thirdEventListener = new eventListenerForMultiple();
			eventListenerForMultiple.FirstCallOnDelete = true;
			eventListenerForMultiple.FirstCallOnGet = true;
			eventListenerForMultiple.FirstCallOnPut = true;

			CacheBuilder
				.For<ReturningRandomNumbers>()
					.CacheMethod(c => c.CachedNumber())
					.As<IReturningRandomNumbers>()
				.AddEventListener(firstEventListener)
				.AddEventListener(secondEventListener)
				.AddEventListener(thirdEventListener);

			var factory = CacheBuilder.BuildFactory();
			var comp = factory.Create<IReturningRandomNumbers>();
			comp.CachedNumber();
			comp.CachedNumber();
			((ICachingComponent)comp).Invalidate();
		}

		[Test]
		public void ShouldHaveCalledGet()
		{
			firstEventListener.GetWasCalled.Should().Be.True();
			secondEventListener.GetWasCalled.Should().Be.True();
			thirdEventListener.GetWasCalled.Should().Be.True();
		}

		[Test]
		public void ShouldHaveCalledPut()
		{
			firstEventListener.PutWasCalled.Should().Be.True();
			secondEventListener.PutWasCalled.Should().Be.True();
			thirdEventListener.PutWasCalled.Should().Be.True();
		}

		[Test]
		public void ShouldHaveCalledDelete()
		{
			firstEventListener.DeleteWasCalled.Should().Be.True();
			secondEventListener.DeleteWasCalled.Should().Be.True();
			thirdEventListener.DeleteWasCalled.Should().Be.True();
		}

		[Test]
		public void ShouldHaveCalledFirstEventListenerFirst()
		{
			firstEventListener.FirstDelete.Should().Be.True();
			firstEventListener.FirstGet.Should().Be.True();
			firstEventListener.FirstPut.Should().Be.True();
		}

		[Test]
		public void ShouldNotHaveCalledNonFirstEventListenersFirst()
		{
			secondEventListener.FirstDelete.Should().Be.False();
			secondEventListener.FirstGet.Should().Be.False();
			secondEventListener.FirstPut.Should().Be.False();

			thirdEventListener.FirstDelete.Should().Be.False();
			thirdEventListener.FirstGet.Should().Be.False();
			thirdEventListener.FirstPut.Should().Be.False();
		}

		private class eventListenerForMultiple : IEventListener
		{
			public static bool FirstCallOnGet;
			public static bool FirstCallOnPut;
			public static bool FirstCallOnDelete;

			public bool FirstGet;
			public bool FirstPut;
			public bool FirstDelete;

			public bool GetWasCalled;
			public bool PutWasCalled;
			public bool DeleteWasCalled;

			void IEventListener.OnGet(EventInformation info, object cachedValue)
			{
				GetWasCalled = true;
				if (FirstCallOnGet)
				{
					FirstCallOnGet = false;
					FirstGet = true;
				}
			}

			void IEventListener.OnDelete(EventInformation info)
			{
				DeleteWasCalled = true;
				if (FirstCallOnDelete)
				{
					FirstCallOnDelete = false;
					FirstDelete = true;
				}
			}

			void IEventListener.OnPut(EventInformation info, object cachedValue)
			{
				PutWasCalled = true;
				if (FirstCallOnPut)
				{
					FirstCallOnPut = false;
					FirstPut = true;
				}
			}
		}
	}
}
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
			eventListenerForMultiple.FirstCallOnGetSuccessful = true;
			eventListenerForMultiple.FirstCallOnPut = true;
			eventListenerForMultiple.FirstCallOnGetUnsuccessful = true;

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
		public void ShouldHaveCalledGetSuccessful()
		{
			firstEventListener.SuccessfulGetWasCalled.Should().Be.True();
			secondEventListener.SuccessfulGetWasCalled.Should().Be.True();
			thirdEventListener.SuccessfulGetWasCalled.Should().Be.True();
		}


		[Test]
		public void ShouldHaveCalledGetUnsuccessful()
		{
			firstEventListener.UnsuccessfulGetWasCalled.Should().Be.True();
			secondEventListener.UnsuccessfulGetWasCalled.Should().Be.True();
			thirdEventListener.UnsuccessfulGetWasCalled.Should().Be.True();
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
			firstEventListener.FirstGetSuccessful.Should().Be.True();
			firstEventListener.FirstPut.Should().Be.True();
			firstEventListener.FirstGetUnsuccessful.Should().Be.True();
		}

		[Test]
		public void ShouldNotHaveCalledNonFirstEventListenersFirst()
		{
			secondEventListener.FirstDelete.Should().Be.False();
			secondEventListener.FirstGetSuccessful.Should().Be.False();
			secondEventListener.FirstGetUnsuccessful.Should().Be.False();
			secondEventListener.FirstPut.Should().Be.False();

			thirdEventListener.FirstDelete.Should().Be.False();
			thirdEventListener.FirstGetSuccessful.Should().Be.False();
			thirdEventListener.FirstGetUnsuccessful.Should().Be.False();
			thirdEventListener.FirstPut.Should().Be.False();
		}

		private class eventListenerForMultiple : IEventListener
		{
			public static bool FirstCallOnGetSuccessful;
			public static bool FirstCallOnPut;
			public static bool FirstCallOnDelete;
			public static bool FirstCallOnGetUnsuccessful;

			public bool FirstGetSuccessful;
			public bool FirstGetUnsuccessful;
			public bool FirstPut;
			public bool FirstDelete;

			public bool SuccessfulGetWasCalled;
			public bool UnsuccessfulGetWasCalled;
			public bool PutWasCalled;
			public bool DeleteWasCalled;

			public void OnGetUnsuccessful(EventInformation eventInformation)
			{
				UnsuccessfulGetWasCalled = true;
				if (FirstCallOnGetUnsuccessful)
				{
					FirstCallOnGetUnsuccessful = false;
					FirstGetUnsuccessful = true;
				}
			}

			public void OnGetSuccessful(CachedItem cachedItem)
			{
				SuccessfulGetWasCalled = true;
				if (FirstCallOnGetSuccessful)
				{
					FirstCallOnGetSuccessful = false;
					FirstGetSuccessful = true;
				}
			}

			public void OnDelete(CachedItem cachedItem)
			{
				DeleteWasCalled = true;
				if (FirstCallOnDelete)
				{
					FirstCallOnDelete = false;
					FirstDelete = true;
				}
			}

			public void OnPut(CachedItem cachedItem)
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
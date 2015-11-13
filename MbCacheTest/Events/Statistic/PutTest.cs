using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Events.Statistic
{
	public class PutTest : FullTest
	{
		private EventListenerForTest eventListener;

		public PutTest(string proxyTypeString)
			: base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			eventListener = new EventListenerForTest();
			CacheBuilder
				.For<ObjectReturningNull>()
					.CacheMethod(c => c.ReturnNullIfZero(0))
					.As<IObjectReturningNull>()
				.AddEventListener(eventListener);

			var factory = CacheBuilder.BuildFactory();
			var comp = factory.Create<IObjectReturningNull>();
			comp.ReturnNullIfZero(0);
			comp.ReturnNullIfZero(1);
		}

		[Test]
		public void ShouldBeCalledTwice()
		{
			eventListener.CacheMisses.Count.Should().Be.EqualTo(2);
		}

		[Test]
		public void ShouldHaveCorrectCachedValues()
		{
			eventListener.CacheMisses[0].CachedValue.Should().Be.Null();
			eventListener.CacheMisses[1].CachedValue.Should().Be.EqualTo(1);
		}

		[Test]
		public void ShouldHaveCorrectCacheKeys()
		{
			eventListener.CacheMisses[0].EventInformation.CacheKey.Should().Contain("|$0");
			eventListener.CacheMisses[1].EventInformation.CacheKey.Should().Contain("|$1");
		}

		[Test]
		public void ShouldHaveCorrectMethodInfo()
		{
			eventListener.CacheMisses[0].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
			eventListener.CacheMisses[1].EventInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
		}

		[Test]
		public void ShouldHaveCorrectType()
		{
			eventListener.CacheMisses[0].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
			eventListener.CacheMisses[1].EventInformation.Type.Should().Be.EqualTo(typeof(IObjectReturningNull));
		}

		[Test]
		public void ShouldHaveCorrectArguments()
		{
			eventListener.CacheMisses[0].EventInformation.Arguments.Should().Have.SameSequenceAs(0);
			eventListener.CacheMisses[1].EventInformation.Arguments.Should().Have.SameSequenceAs(1);
		}
	}
}
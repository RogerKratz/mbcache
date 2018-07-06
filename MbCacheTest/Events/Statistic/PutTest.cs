using System;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Events.Statistic
{
	public class PutTest : TestCase
	{
		private EventListenerForTest eventListener;

		public PutTest(Type proxyType) : base(proxyType)
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
		public void ShouldHaveCorrectMethodInfo()
		{
			eventListener.CacheMisses[0].CachedMethodInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
			eventListener.CacheMisses[1].CachedMethodInformation.Method.Name.Should().Be.EqualTo("ReturnNullIfZero");
		}

		[Test]
		public void ShouldHaveCorrectType()
		{
			eventListener.CacheMisses[0].CachedMethodInformation.Method.DeclaringType.Should().Be.EqualTo(typeof(IObjectReturningNull));
			eventListener.CacheMisses[1].CachedMethodInformation.Method.DeclaringType.Should().Be.EqualTo(typeof(IObjectReturningNull));
		}

		[Test]
		public void ShouldHaveCorrectArguments()
		{
			eventListener.CacheMisses[0].CachedMethodInformation.Arguments.Should().Have.SameSequenceAs(0);
			eventListener.CacheMisses[1].CachedMethodInformation.Arguments.Should().Have.SameSequenceAs(1);
		}
	}
}
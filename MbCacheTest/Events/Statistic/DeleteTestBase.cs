using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Events.Statistic;

public abstract class DeleteTestBase
{
	private EventListenerForTest eventListener;
	
	[SetUp]
	public void Setup()
	{
		eventListener = new EventListenerForTest();
		
		var factory = new CacheBuilder()
			.For<ObjectReturningNull>()
			.CacheMethod(c => c.ReturnNullIfZero(0))
			.As<IObjectReturningNull>()
			.AddEventListener(eventListener)
			.BuildFactory();
		var comp = factory.Create<IObjectReturningNull>();
		comp.ReturnNullIfZero(0);
		comp.ReturnNullIfZero(1);
		Invalidate(factory, comp);
	}

	protected abstract void Invalidate(IMbCacheFactory factory, IObjectReturningNull comp);

	[Test]
	public void ShouldBeCalledTwice()
	{
		eventListener.CacheRemovals.Count.Should().Be.EqualTo(2);
	}

	[Test]
	public void ShouldHaveCorrectValue()
	{
		eventListener.CacheRemovals[0].CachedValue.Should().Be.Null();
		eventListener.CacheRemovals[1].CachedValue.Should().Be.EqualTo(1);
	}

	[Test]
	public void ShouldHaveCorrectMethodInfo()
	{
		eventListener.CacheRemovals[0].CachedMethod.Name.Should().Be.EqualTo("ReturnNullIfZero");
		eventListener.CacheRemovals[1].CachedMethod.Name.Should().Be.EqualTo("ReturnNullIfZero");
	}

	[Test]
	public void ShouldHaveCorrectType()
	{
		eventListener.CacheRemovals[0].CachedMethod.DeclaringType.Should().Be.EqualTo<IObjectReturningNull>();
		eventListener.CacheRemovals[1].CachedMethod.DeclaringType.Should().Be.EqualTo<IObjectReturningNull>();
	}
}
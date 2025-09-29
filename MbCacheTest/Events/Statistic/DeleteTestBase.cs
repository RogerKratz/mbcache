using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Events.Statistic;

public abstract class DeleteTestBase //: TestCase
{
	protected EventListenerForTest EventListener;
	

	[Test]
	public void ShouldBeCalledTwice()
	{
		EventListener.CacheRemovals.Count.Should().Be.EqualTo(2);
	}

	[Test]
	public void ShouldHaveCorrectValue()
	{
		EventListener.CacheRemovals[0].CachedValue.Should().Be.Null();
		EventListener.CacheRemovals[1].CachedValue.Should().Be.EqualTo(1);
	}

	[Test]
	public void ShouldHaveCorrectMethodInfo()
	{
		EventListener.CacheRemovals[0].CachedMethod.Name.Should().Be.EqualTo("ReturnNullIfZero");
		EventListener.CacheRemovals[1].CachedMethod.Name.Should().Be.EqualTo("ReturnNullIfZero");
	}

	[Test]
	public void ShouldHaveCorrectType()
	{
		EventListener.CacheRemovals[0].CachedMethod.DeclaringType.Should().Be.EqualTo(typeof(IObjectReturningNull));
		EventListener.CacheRemovals[1].CachedMethod.DeclaringType.Should().Be.EqualTo(typeof(IObjectReturningNull));
	}
}
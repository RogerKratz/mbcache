using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Events.Statistic;

public class ImplicitDeleteTest : DeleteTestBase
{
	[SetUp]
	public void Setup()
	{
		EventListener = new EventListenerForTest();
		
		var factory = new CacheBuilder()
			.For<ObjectReturningNull>()
			.CacheMethod(c => c.ReturnNullIfZero(0))
			.As<IObjectReturningNull>()
			.AddEventListener(EventListener)
			.BuildFactory();
		var comp = factory.Create<IObjectReturningNull>();
		comp.ReturnNullIfZero(0);
		comp.ReturnNullIfZero(1);
		factory.Invalidate<IObjectReturningNull>();
	}
}
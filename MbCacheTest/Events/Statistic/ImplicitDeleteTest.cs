using System;
using MbCacheTest.TestData;

namespace MbCacheTest.Events.Statistic;

public class ImplicitDeleteTest : DeleteTestBase
{
	protected override void TestSetup()
	{
		EventListener = new EventListenerForTest();
		CacheBuilder
			.For<ObjectReturningNull>()
			.CacheMethod(c => c.ReturnNullIfZero(0))
			.As<IObjectReturningNull>()
			.AddEventListener(EventListener);

		var factory = CacheBuilder.BuildFactory();
		var comp = factory.Create<IObjectReturningNull>();
		comp.ReturnNullIfZero(0);
		comp.ReturnNullIfZero(1);
		factory.Invalidate<IObjectReturningNull>();
	}
}
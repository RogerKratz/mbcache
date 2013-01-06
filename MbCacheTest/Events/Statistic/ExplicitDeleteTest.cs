using MbCacheTest.TestData;

namespace MbCacheTest.Events.Statistic
{
	public class ExplicitDeleteTest : DeleteTestBase
	{
		public ExplicitDeleteTest(string proxyTypeString)
			: base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			EventListener = new EventListenerForDelete();
			CacheBuilder
				.For<ObjectReturningNull>()
					.CacheMethod(c => c.ReturnNullIfZero(0))
					.As<IObjectReturningNull>()
				.AddEventListener(EventListener);

			var factory = CacheBuilder.BuildFactory();
			var comp = factory.Create<IObjectReturningNull>();
			comp.ReturnNullIfZero(0);
			comp.ReturnNullIfZero(1);
			factory.Invalidate(comp, c => c.ReturnNullIfZero(0), true);
			factory.Invalidate(comp, c => c.ReturnNullIfZero(1), true);
		}
	}
}
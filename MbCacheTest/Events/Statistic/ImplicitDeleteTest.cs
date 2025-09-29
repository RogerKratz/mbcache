using MbCache.Core;
using MbCacheTest.TestData;

namespace MbCacheTest.Events.Statistic;

public class ImplicitDeleteTest : DeleteTestBase
{
	protected override void Invalidate(IMbCacheFactory factory, IObjectReturningNull comp) => 
		factory.Invalidate<IObjectReturningNull>();
}
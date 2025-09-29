using MbCache.Core;
using MbCacheTest.TestData;

namespace MbCacheTest.Events.Statistic;

public class ExplicitDeleteTest : DeleteTestBase
{
	protected override void Invalidate(IMbCacheFactory factory, IObjectReturningNull comp)
	{
		factory.Invalidate(comp, c => c.ReturnNullIfZero(0), true);
		factory.Invalidate(comp, c => c.ReturnNullIfZero(1), true);
	}
}
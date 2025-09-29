using System;

namespace MbCacheTest.TestData;

public class ObjectReturningNewGuidsNoInterface
{
	public virtual Guid CachedMethod()
	{
		return Guid.NewGuid();
	}

	public virtual Guid CachedMethod2()
	{
		return Guid.NewGuid();
	}
}
using System;

namespace MbCacheTest.TestData;

public class ObjectWithGenericMethodParameters
{
	public virtual Guid CachedMethod1<T1, T2>(T1 first, T2 second)
	{
		return Guid.NewGuid();
	}
}
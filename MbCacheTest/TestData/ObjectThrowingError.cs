using System;

namespace MbCacheTest.TestData
{
	public class ObjectThrowingError
	{
		public virtual int ThrowsArgumentOutOfRangeException()
		{
			throw new ArgumentOutOfRangeException();
		}
	}
}
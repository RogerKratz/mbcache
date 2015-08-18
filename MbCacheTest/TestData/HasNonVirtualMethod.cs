using System;

namespace MbCacheTest.TestData
{
	public class HasNonVirtualMethod
	{
		public Guid NonVirtual()
		{
			return Guid.NewGuid();
		}

		public virtual Guid Virtual()
		{
			return Guid.NewGuid();
		}
	}
}
using System;

namespace MbCacheTest.TestData
{
	public class HasNonVirtualMethod : IHasNonVirtualMethod
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

	public interface IHasNonVirtualMethod
	{
		Guid Virtual();
	}
}
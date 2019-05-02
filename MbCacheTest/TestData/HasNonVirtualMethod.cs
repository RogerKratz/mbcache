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

		private int _internalState;
		public int NonVirtualWithInternalState()
		{
			return _internalState;
		}

		public virtual void SetInternalState(int value)
		{
			_internalState = value;
		}
	}
}
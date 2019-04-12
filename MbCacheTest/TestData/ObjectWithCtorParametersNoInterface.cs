using System;

namespace MbCacheTest.TestData
{
	public class ObjectWithCtorParametersNoInterface
	{
		public ObjectWithCtorParametersNoInterface(int value)
		{
			Value = value;
		}

		public virtual int Value { get; }
		
		public virtual Guid CachedMethod()
		{
			return Guid.NewGuid();
		}
	}
}
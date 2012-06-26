using System;

namespace MbCacheTest.TestData
{
	public class ObjectWithOverloadedMethod
	{
		public virtual Guid Something(int a)
		{
			return Guid.NewGuid();
		}

		public virtual Guid Something()
		{
			return Guid.NewGuid();
		}

		public virtual Guid Something(string a)
		{
			return Guid.NewGuid();
		}
	}
}
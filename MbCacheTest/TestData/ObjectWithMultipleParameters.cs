using System;

namespace MbCacheTest.TestData
{
	public class ObjectWithMultipleParameters
	{
		public virtual Guid Calculate(int a, string b, long c)
		{
			return Guid.NewGuid();
		}
	}
}
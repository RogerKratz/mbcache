namespace MbCacheTest.TestData
{
	public class HasNonVirtualMethod
	{
		public int NonVirtual()
		{
			return 0;
		}

		public virtual int Virtual()
		{
			return 0;
		}
	}
}
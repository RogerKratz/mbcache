namespace MbCacheTest.TestData
{
	public class ObjectReturningOne
	{
		public virtual int NumberOfExecutions { get; private set; }
		
		public virtual int Return1()
		{
			NumberOfExecutions++;
			return 1;
		}
		
		public virtual int Return2()
		{
			NumberOfExecutions++;
			return 2;
		}
	}
}
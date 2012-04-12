namespace MbCacheTest.TestData
{
	public class ObjectWithCallCounter : IObjectWithCallCounter
	{
		public virtual int Count { get; set; }
		public virtual object Increment()
		{
			Count++;
			return new object();
		}
	}
}
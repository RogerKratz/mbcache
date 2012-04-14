namespace MbCacheTest.TestData
{
	public interface IObjectWithCallCounter
	{
		int Count { get; }
		object Increment();
	}

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
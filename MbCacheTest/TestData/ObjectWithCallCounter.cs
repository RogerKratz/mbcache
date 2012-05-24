namespace MbCacheTest.TestData
{
	public interface IObjectWithCallCounter
	{
		int Count { get; }
		object Increment();
	}

	public class ObjectWithCallCounter : IObjectWithCallCounter
	{
		public int Count { get; set; }
		public object Increment()
		{
			Count++;
			return new object();
		}
	}
}
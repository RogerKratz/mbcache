namespace MbCacheTest.TestData
{
	public interface IObjectWithCallCounter
	{
		int Count { get; }
		object Increment(); 
	}
}
namespace MbCacheTest.TestData
{
	public class ObjectReturningNull : IObjectReturningNull
	{
		public object ReturnNullIfZero(int parameter)
		{
			return parameter == 0 ? (object) null : parameter;
		}
	}

	public interface IObjectReturningNull
	{
		object ReturnNullIfZero(int parameter);
	}
}
namespace MbCacheTest.TestData
{
	public class ObjectWithParameterReturningFixedValue
	{
		private readonly object returnValue = new object();

		public virtual object DoIt(int parameterValue)
		{
			return returnValue;
		}
	}
}
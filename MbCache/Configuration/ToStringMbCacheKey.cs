namespace MbCache.Configuration
{
	public class ToStringMbCacheKey : MbCacheKeyBase
	{
		private const string nullKey = "Null";

		protected override string ParameterValue(object parameter)
		{
			return parameter == null ? 
						nullKey : 
						parameter.ToString();
		}
	}
}
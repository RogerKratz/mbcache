using System;

namespace MbCache.Configuration
{
	[Serializable]
	public class ToStringCacheKey : CacheKeyBase
	{
		private const string nullKey = "Null";

		protected override string ParameterValue(object parameter)
		{
			return parameter?.ToString() ?? nullKey;
		}
	}
}
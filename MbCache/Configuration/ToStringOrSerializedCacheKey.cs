using System;
using Newtonsoft.Json;

namespace MbCache.Configuration
{
	[Serializable]
	public class ToStringOrSerializedCacheKey : ToStringCacheKey
	{

		protected override string ParameterValue(object parameter)
		{
			var toStringKey = base.ParameterValue(parameter);

			if (parameterIsSuspicious(parameter, toStringKey))
				return JsonConvert.SerializeObject(parameter);
			return toStringKey;
		}
	}
}
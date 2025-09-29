using MbCache.Configuration;

namespace MbCacheTest.Logic.Scope;

public class CacheKeyWithScope : CacheKeyBase
{
	protected override string ParameterValue(object parameter)
	{
		return parameter.ToString();
	}

	public static string CurrentScope { get; set; }

	protected override string Scope()
	{
		return CurrentScope;
	}
}
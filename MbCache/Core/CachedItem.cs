using System.Reflection;

namespace MbCache.Core
{
	public class CachedItem
	{
		public CachedItem(MethodInfo cachedMethod, object cachedValue)
		{
			CachedMethod = cachedMethod;
			CachedValue = cachedValue;
		}

		public MethodInfo CachedMethod { get; }
		public object CachedValue { get; }
	}
}
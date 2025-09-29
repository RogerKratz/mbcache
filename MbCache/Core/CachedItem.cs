using System.Reflection;

namespace MbCache.Core;

public class CachedItem(MethodInfo cachedMethod, object cachedValue)
{
	public MethodInfo CachedMethod { get; } = cachedMethod;
	public object CachedValue { get; } = cachedValue;
}
using MbCache.Core.Events;

namespace MbCache.Core
{
	public class CachedItem
	{
		public CachedItem(CachedMethodInformation cachedMethodInformation, object cachedValue)
		{
			CachedMethodInformation = cachedMethodInformation;
			CachedValue = cachedValue;
		}

		public CachedMethodInformation CachedMethodInformation { get; }
		public object CachedValue { get; }
	}
}
using MbCache.Core.Events;

namespace MbCache.Core
{
	public class CachedItem
	{
		public CachedItem(EventInformation eventInformation, object cachedValue)
		{
			EventInformation = eventInformation;
			CachedValue = cachedValue;
		}

		public EventInformation EventInformation { get; private set; }
		public object CachedValue { get; private set; }
	}
}
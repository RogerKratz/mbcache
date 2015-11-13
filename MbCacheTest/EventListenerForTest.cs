using System.Collections.Generic;
using MbCache.Core;
using MbCache.Core.Events;

namespace MbCacheTest
{
	public class EventListenerForTest : IEventListener
	{
		public void OnCacheHit(CachedItem cachedItem)
		{
		}

		public void OnCacheRemoval(CachedItem cachedItem)
		{
		}

		public void OnCacheMiss(CachedItem cachedItem)
		{
		}

		public IList<string> Warnings { get; } = new List<string>();
		public void Warning(string warnMessage)
		{
			Warnings.Add(warnMessage);
		}
	}
}
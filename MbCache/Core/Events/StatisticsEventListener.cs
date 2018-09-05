using System.Threading;

namespace MbCache.Core.Events
{
	public class StatisticsEventListener : IEventListener
	{
		private long _cacheHits;
		private long _cacheMisses;
		
		void IEventListener.OnCacheHit(CachedItem cachedItem)
		{
			Interlocked.Increment(ref _cacheHits);
		}

		void IEventListener.OnCacheRemoval(CachedItem cachedItem)
		{
		}

		void IEventListener.OnCacheMiss(CachedItem cachedItem)
		{
			Interlocked.Increment(ref _cacheMisses);
		}

		public long CacheHits => _cacheHits;

		public long CacheMisses => _cacheMisses;

		public void Clear()
		{
			_cacheHits = 0;
			_cacheMisses = 0;
		}
	}
}
using System.Threading;

namespace MbCache.Core.Events
{
	public class StatisticsEventListener : IEventListener
	{
		private long _cacheHits;
		private long _cacheMisses;

		void IEventListener.OnGet(EventInformation info)
		{
			Interlocked.Increment(ref _cacheHits);
		}

		void IEventListener.OnDelete(EventInformation info)
		{
		}

		void IEventListener.OnPut(EventInformation info)
		{
			Interlocked.Increment(ref _cacheMisses);
		}

		public long CacheHits
		{
			get { return _cacheHits; }
		}

		public long CacheMisses
		{
			get
			{
				return _cacheMisses;
			}
		}
	}
}
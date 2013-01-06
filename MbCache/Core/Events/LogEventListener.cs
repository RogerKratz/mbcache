using System;
using log4net;

namespace MbCache.Core.Events
{
	[Serializable]
	public class LogEventListener : IEventListener
	{
		private const string putMessage = "Adding cache value {0} for {1} (key: {2})";
		private const string deleteMessage = "Removing cache entries for {0} (key starts with: {1})";
		private const string cacheHitLogMessage = "Cache hit for {0} (key: {1})";
		private const string cacheMissLogMessage = "Cache miss for {0} (key: {1})";

		private static readonly ILog log = LogManager.GetLogger(typeof(LogEventListener));

		public static bool IsLoggingEnabled()
		{
			return log.IsDebugEnabled;
		}

		public void OnGet(CachedItem cachedItem)
		{
			log.DebugFormat(cachedItem.CachedValue == null ? cacheMissLogMessage : cacheHitLogMessage, 
							cachedItem.EventInformation.MethodName(), 
							cachedItem.EventInformation.CacheKey);
		}

		public void OnDelete(EventInformation info)
		{
			log.DebugFormat(deleteMessage, info.MethodName(), info.CacheKey);
		}

		public void OnPut(CachedItem cachedItem)
		{
			log.DebugFormat(putMessage, cachedItem.CachedValue, cachedItem.EventInformation.MethodName(), cachedItem.EventInformation.CacheKey);
		}
	}
}
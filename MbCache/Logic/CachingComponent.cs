using System;
using System.Linq;
using System.Linq.Expressions;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
	public class CachingComponent : ICachingComponent
	{
		private readonly CacheAdapter _cache;
		private readonly ICacheKey _cacheKey;
		private readonly ComponentType _componentType;

		public CachingComponent(CacheAdapter cache, ConfigurationForType configurationForType)
		{
			_cache = cache;
			_cacheKey = configurationForType.CacheKey;
			_componentType = configurationForType.ComponentType;
			UniqueId = configurationForType.CachePerInstance ? Guid.NewGuid().ToString() : "Global";
			AllowDifferentArgumentsShareSameCacheKey = configurationForType.AllowDifferentArgumentsShareSameCacheKey;
		}

		public string UniqueId { get; }
		public bool AllowDifferentArgumentsShareSameCacheKey { get; }
		
		public void Invalidate()
		{
			var cacheKey = _cacheKey.RemoveKey(_componentType, this);
			_cache.Delete(cacheKey);
		}

		public void Invalidate<T>(Expression<Func<T, object>> method, bool matchParameterValues)
		{
			string key;
			var methodInfo = ExpressionHelper.MemberName(method.Body);
			if (matchParameterValues && methodInfo.GetParameters().Any())
			{
				var arguments = ExpressionHelper.ExtractArguments(method.Body).ToArray();
				key = _cacheKey.RemoveKey(_componentType, this, methodInfo, arguments);
			}
			else
			{
				key = _cacheKey.RemoveKey(_componentType, this, methodInfo);
			}
			_cache.Delete(key);
		}
	}
}
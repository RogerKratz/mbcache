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

		public CachingComponent(CacheAdapter cache,
										ICacheKey cacheKey,
										ConfigurationForType configurationForType)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_componentType = configurationForType.ComponentType;
			UniqueId = configurationForType.CachePerInstance ? Guid.NewGuid().ToString() : "Global";
		}

		public string UniqueId { get; }

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
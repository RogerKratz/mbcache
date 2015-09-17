using System;
using System.Linq;
using System.Linq.Expressions;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Core.Events;

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
			var cacheKey = _cacheKey.Key(_componentType, this);
			var deleteArgs = new EventInformation(cacheKey, _componentType.ConfiguredType, null, null);
			_cache.Delete(deleteArgs);
		}

		public void Invalidate<T>(Expression<Func<T, object>> method, bool matchParameterValues)
		{
			string key;
			object[] arguments;
			var methodInfo = ExpressionHelper.MemberName(method.Body);
			if (matchParameterValues && methodInfo.GetParameters().Any())
			{
				arguments = ExpressionHelper.ExtractArguments(method.Body).ToArray();
				key = _cacheKey.Key(_componentType, this, methodInfo, arguments);
			}
			else
			{
				arguments = new object[0];
				key = _cacheKey.Key(_componentType, this, methodInfo);
			}
			var deleteInfo = new EventInformation(key, _componentType.ConfiguredType, methodInfo, arguments);
			_cache.Delete(deleteInfo);
		}
	}
}
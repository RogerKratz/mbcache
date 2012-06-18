using System;
using System.Linq.Expressions;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
	public class CachingComponent : ICachingComponent
	{
		private readonly ICache _cache;
		private readonly ICacheKey _cacheKey;
		private readonly Type _definedType;

		public CachingComponent(ICache cache,
										ICacheKey cacheKey,
										Type definedType,
										ImplementationAndMethods details)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_definedType = definedType;
			UniqueId = details.CachePerInstance ? Guid.NewGuid().ToString() : "Global";
		}

		public string UniqueId { get; private set; }

		public void Invalidate()
		{
			_cache.Delete(_cacheKey.Key(_definedType, this));
		}

		public void Invalidate<T>(Expression<Func<T, object>> method, bool matchParameterValues)
		{
			var methodInfo = ExpressionHelper.MemberName(method.Body);
			string key;
			if (matchParameterValues)
			{
				var arguments = ExpressionHelper.ExtractArguments(method.Body);
				key = _cacheKey.Key(_definedType, this, methodInfo, arguments);
			}
			else
			{
				key = _cacheKey.Key(_definedType, this, methodInfo);
			}
			_cache.Delete(key);
		}
	}
}
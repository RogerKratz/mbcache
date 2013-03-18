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
		private readonly Type _definedType;

		public CachingComponent(CacheAdapter cache,
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
			var cacheKey = _cacheKey.Key(_definedType, this);
			var deleteArgs = new EventInformation(cacheKey, _definedType, null, null);
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
				key = _cacheKey.Key(_definedType, this, methodInfo, arguments);
			}
			else
			{
				arguments = new object[0];
				key = _cacheKey.Key(_definedType, this, methodInfo);
			}
			var deleteInfo = new EventInformation(key, _definedType, methodInfo, arguments);
			_cache.Delete(deleteInfo);
		}
	}
}
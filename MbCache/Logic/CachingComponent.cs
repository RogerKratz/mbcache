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
		private readonly bool _allowDifferentArgumentsShareSameCacheKey;
		private readonly string suspiciousParam =
			"Cache key of type {0} equals its own type name. You should specify a value for this parameter in your ICacheKey implementation." + Environment.NewLine +
			"However, even though it's not recommended, you can override this exception by calling AllowDifferentArgumentsShareSameCacheKey when configuring your cached component.";

		public CachingComponent(CacheAdapter cache, ConfigurationForType configurationForType)
		{
			_cache = cache;
			_cacheKey = configurationForType.CacheKey;
			_componentType = configurationForType.ComponentType;
			UniqueId = configurationForType.CachePerInstance ? Guid.NewGuid().ToString() : "Global";
			_allowDifferentArgumentsShareSameCacheKey = configurationForType.AllowDifferentArgumentsShareSameCacheKey;
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
		
		public void CheckIfSuspiciousParameter(object parameter, string parameterKey)
		{
			if (_allowDifferentArgumentsShareSameCacheKey || parameter == null) 
				return;
			if (parameterKey.Equals(parameter.GetType().ToString()))
			{
				throw new ArgumentException(string.Format(suspiciousParam, parameterKey), parameterKey);
			}
		}
	}
}
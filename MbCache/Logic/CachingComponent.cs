using System;
using System.Linq;
using System.Linq.Expressions;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic;

public class CachingComponent(ConfigurationForType configurationForType) : ICachingComponent
{
	private readonly ICache _cache = configurationForType.Cache;
	private readonly ICacheKey _cacheKey = configurationForType.CacheKey;
	private readonly ComponentType _componentType = configurationForType.ComponentType;
	private readonly bool _allowDifferentArgumentsShareSameCacheKey = configurationForType.AllowDifferentArgumentsShareSameCacheKey;
	private readonly string suspiciousParam =
		"Cache key of type {0} equals its own type name. You should specify a value for this parameter in your ICacheKey implementation." + Environment.NewLine +
		"However, even though it's not recommended, you can override this exception by calling AllowDifferentArgumentsShareSameCacheKey when configuring your cached component.";

	public string UniqueId { get; } = configurationForType.CachePerInstance ? Guid.NewGuid().ToString() : "Global";

	public void Invalidate()
	{
		var cacheKey = _cacheKey.RemoveKey(_componentType, this);
		_cache.Delete(cacheKey);
	}

	public void Invalidate<T>(Expression<Func<T, object>> method, bool matchParameterValues)
	{
		var methodInfo = ExpressionHelper.MemberName(method.Body);
		if (matchParameterValues && methodInfo.GetParameters().Any())
		{
			var arguments = ExpressionHelper.ExtractArguments(method.Body).ToArray();
			var key = _cacheKey.RemoveKey(_componentType, this, methodInfo, arguments);
			if (key != null)
			{
				_cache.Delete(key);
			}
		}
		else
		{
			_cache.Delete(_cacheKey.RemoveKey(_componentType, this, methodInfo));
		}
	}
		
	public void CheckIfSuspiciousParameter(object parameter, string parameterKey)
	{
		if (_allowDifferentArgumentsShareSameCacheKey || parameter == null) 
			return;
		if (parameterKey.Equals(parameter.GetType().ToString()))
			throw new ArgumentException(string.Format(suspiciousParam, parameterKey), parameterKey);
	}
}
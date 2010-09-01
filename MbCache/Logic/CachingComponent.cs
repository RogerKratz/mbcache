using System;
using System.Linq.Expressions;

namespace MbCache.Logic
{
    public class CachingComponent : ICachingComponent
    {
        private readonly ICache _cache;
        private readonly IMbCacheKey _cacheKey;
        private readonly Type _definedType;

        public CachingComponent(ICache cache, 
                                IMbCacheKey cacheKey,
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

        public void Invalidate<T>(Expression<Func<T, object>> method)
        {
            _cache.Delete(_cacheKey.Key(_definedType, this, ExpressionHelper.MemberName(method.Body)));
        }
    }
}
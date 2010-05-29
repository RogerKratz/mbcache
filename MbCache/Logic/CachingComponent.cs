using System;
using System.Collections.Generic;

namespace MbCache.Logic
{
    public class CachingComponent : ICachingComponent
    {
        private readonly ICache _cache;
        private readonly IMbCacheKey _cacheKey;
        private readonly Type _definedType;
        private readonly ImplementationAndMethods _details;
        private IEnumerable<string> _keysForThisComponent;
        private object lockObject =new object();

        public CachingComponent(ICache cache, 
                                IMbCacheKey cacheKey,
                                Type definedType,
                                ImplementationAndMethods details)
        {
            _cache = cache;
            _cacheKey = cacheKey;
            _definedType = definedType;
            _details = details;
            UniqueId = "Global";
        }

        public string UniqueId { get; set; }

        public IEnumerable<string> KeysForThisComponent
        {
            get
            {
                if(_keysForThisComponent==null)
                {
                    lock(lockObject)
                    {
                        if(_keysForThisComponent==null)
                        {
                            var keys = new List<string>();
                            foreach (var method in _details.Methods)
                            {
                                keys.Add(string.Concat(_cacheKey.CacheKey(_definedType, method),
                                                        _cacheKey.AddForComponent(this)));
                            }
                            _keysForThisComponent = keys;                            
                        }
                    }
                }

                return _keysForThisComponent;
            }
        }

        public void Invalidate()
        {
            foreach (var key in KeysForThisComponent)
            {
                _cache.Delete(key);   
            }
        }
    }
}
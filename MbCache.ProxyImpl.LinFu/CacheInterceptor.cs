using System;
using System.Reflection;
using LinFu.DynamicProxy;
using log4net;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
    public class CacheInterceptor : IInterceptor
    {
        private static ILog log = LogManager.GetLogger(typeof(CacheInterceptor));

        private readonly ICache _cache;
        private readonly IMbCacheKey _cacheKey;
        private readonly Type _type;
        private readonly ImplementationAndMethods _methodData;
        private readonly object _target;
        private readonly ICachingComponent _cachingComponent;

        public CacheInterceptor(ICache cache, 
                                                IMbCacheKey cacheKey, 
                                                Type type,
                                                ImplementationAndMethods methodData,
                                                params object[] ctorParameters) 
        {
            _cache = cache;
            _cacheKey = cacheKey;
            _type = type;
            _methodData = methodData;
            _target = createTarget(ctorParameters);
            _cachingComponent = new CachingComponent(cache, cacheKey, type, methodData);
        }

        private object createTarget(object[] ctorParameters)
        {
            try
            {
                return Activator.CreateInstance(_methodData.ConcreteType, ctorParameters);
            }
            catch (MissingMethodException ex)
            {
                var ctorParamMessage = "Incorrect number of parameters to ctor for type " + _methodData.ConcreteType;
                throw new ArgumentException(ctorParamMessage, ex);
            }
        }

        public object Intercept(InvocationInfo info)
        {
            return methodMarkedForCaching(info.TargetMethod) ? 
                    interceptUsingCache(info) : callOriginalMethod(info);
        }

        private bool methodMarkedForCaching(MethodInfo method)
        {
            //why?
            foreach (var cachedMethod in _methodData.Methods)
            {
                if (cachedMethod.Name.Equals(method.Name))
                    return true;
            }
            return false;
        }

        private object interceptUsingCache(InvocationInfo info)
        {
            object retVal;
            var method = info.TargetMethod;
            var arguments = info.Arguments;
            var key = _cacheKey.Key(_type, method, _cachingComponent, arguments);

            log.Debug("Trying to find cache entry <" + key + ">");
            var cachedValue = _cache.Get(key);
            if (cachedValue != null)
            {
                log.Debug("Cache hit for <" + key + ">");
                retVal = cachedValue;
            }
            else
            {
                log.Debug("Cache miss for <" + key + ">");
                retVal = callOriginalMethod(info);
                log.Debug("Put in cache entry <" + key + ">");
                _cache.Put(key, retVal);
            }

            return retVal;
        }

        private object callOriginalMethod(InvocationInfo info)
        {
            if(info.TargetMethod.DeclaringType.Equals(typeof(ICachingComponent)))
            {
                return info.TargetMethod.Invoke(_cachingComponent, info.Arguments);
            }
            return info.TargetMethod.Invoke(_target, info.Arguments);
        }
    }
}
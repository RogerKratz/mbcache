using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using LinFu.DynamicProxy;
using log4net;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
    public class CacheInterceptorAndComponent : CachingComponent, IInterceptor
    {
        private static ILog log = LogManager.GetLogger(typeof(CacheInterceptorAndComponent));

        private readonly ICache _cache;
        private readonly IMbCacheKey _cacheKey;
        private readonly Type _type;
        private readonly ImplementationAndMethods _methodData;
        private readonly object _target;

        public CacheInterceptorAndComponent(ICache cache, 
                                                IMbCacheKey cacheKey, 
                                                Type type,
                                                ImplementationAndMethods methodData,
                                                params object[] ctorParameters) 
            : base(cache, cacheKey, type, methodData)
        {
            _cache = cache;
            _cacheKey = cacheKey;
            _type = type;
            _methodData = methodData;
            _target = createTarget(ctorParameters);
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
            var typeAndMethodName = "<" + _type + "." + method.Name + "()>";
            log.Debug("Entering " + typeAndMethodName);

            var arguments = info.Arguments;

            var key = string.Concat(_cacheKey.CacheKey(_type, method),
                                    _cacheKey.AddForComponent(this),
                                    _cacheKey.AddForParameterValues(_type, method, arguments));

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

            log.Debug("Leaving " + typeAndMethodName);
            return retVal;
        }

        private object callOriginalMethod(InvocationInfo info)
        {
            try
            {
                return info.TargetMethod.Invoke(_target, info.Arguments);
            }
            catch (TargetException)
            {
                if(log.IsDebugEnabled)
                {
                    var logMess = "Cannot find method " + info.TargetMethod + " on type " + info.Target + ". Trying to call CachingComponent instead.";
                    log.Debug(logMess);                    
                }
                return info.TargetMethod.Invoke(this, info.Arguments);
            }
        }
    }
}
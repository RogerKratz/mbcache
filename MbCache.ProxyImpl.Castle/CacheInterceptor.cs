using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
	public class CacheInterceptor : IInterceptor
	{
		private readonly CacheAdapter _cache;
		private readonly ICacheKey _cacheKey;
		private readonly ILockObjectGenerator _lockObjectGenerator;
		private readonly Type _type;
		private readonly IEnumerable<MethodInfo> _methods; //only needed cause IProxyGenerationHook doesn't seem to offer closed generic types

		public CacheInterceptor(CacheAdapter cache, 
										ICacheKey cacheKey, 
										ILockObjectGenerator lockObjectGenerator, 
										Type type,
										IEnumerable<MethodInfo> methods)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_lockObjectGenerator = lockObjectGenerator;
			_type = type;
			_methods = methods;
		}

		public void Intercept(IInvocation invocation)
		{
			var method = invocation.Method;
			//ugly hack
			if (method.IsGenericMethod && !_methods.Contains(method, new MethodInfoComparer()))
			{
				invocation.Proceed();
				return;
			}
			var proxy = (ICachingComponent)invocation.Proxy;
			var arguments = invocation.Arguments;

			var key = _cacheKey.Key(_type, proxy, method, arguments);
			if (key == null)
			{
				invocation.Proceed();
			}
			else
			{
				if (tryGetValueFromCache(invocation, key))
					return;

				var lockObject = _lockObjectGenerator.GetFor(key);
				if (lockObject == null)
				{
					executeAndPutInCache(invocation, key);
				}
				else
				{
					lock (lockObject)
					{
						if (tryGetValueFromCache(invocation, key))
							return;
						executeAndPutInCache(invocation, key);
					}				
				}
			}
		}

		private void executeAndPutInCache(IInvocation invocation, string key)
		{
			invocation.Proceed();
			var putInfo = new PutInfo(key, _type, invocation.Method, invocation.Arguments);
			_cache.Put(putInfo, invocation.ReturnValue);
		}

		private bool tryGetValueFromCache(IInvocation invocation, string key)
		{
			var getInfo = new GetInfo(key, _type, invocation.Method, invocation.Arguments);
			var cachedValue = _cache.Get(getInfo);
			if (cachedValue != null)
			{
				invocation.ReturnValue = cachedValue;
				return true;
			}
			return false;
		}
	}
}
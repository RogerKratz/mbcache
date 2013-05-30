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
		private readonly ImplementationAndMethods _methods;

		public CacheInterceptor(CacheAdapter cache, 
										ICacheKey cacheKey, 
										ILockObjectGenerator lockObjectGenerator, 
										Type type,
										ImplementationAndMethods methods)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_lockObjectGenerator = lockObjectGenerator;
			_type = type;
			_methods = methods;
		}

		public void Intercept(IInvocation invocation)
		{
			if (!_methods.EnabledCache)
			{
				invocation.Proceed();
				return;
			}

			var method = invocation.Method;
			//ugly hack
			if (method.IsGenericMethod && !_methods.Methods.Contains(method, new MethodInfoComparer()))
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
				var eventInfo = new EventInformation(key, _type, invocation.Method, invocation.Arguments);
				if (tryGetValueFromCache(invocation, eventInfo))
					return;

				var lockObject = _lockObjectGenerator.GetFor(key);
				if (lockObject == null)
				{
					executeAndPutInCache(invocation, eventInfo);
				}
				else
				{
					lock (lockObject)
					{
						if (tryGetValueFromCache(invocation, eventInfo))
							return;
						executeAndPutInCache(invocation, eventInfo);
					}				
				}
			}
		}

		private void executeAndPutInCache(IInvocation invocation, EventInformation eventInfo)
		{
			invocation.Proceed();
			_cache.Put(new CachedItem(eventInfo, invocation.ReturnValue));
		}

		private bool tryGetValueFromCache(IInvocation invocation, EventInformation eventInfo)
		{
			var cachedValue = _cache.Get(eventInfo);
			if (cachedValue != null)
			{
				invocation.ReturnValue = cachedValue;
				return true;
			}
			return false;
		}
	}
}
using System.Linq;
using Castle.DynamicProxy;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
	public class CacheInterceptor : IInterceptor
	{
		private readonly ConfigurationForType _configurationForType;

		public CacheInterceptor(ConfigurationForType configurationForType)
		{
			_configurationForType = configurationForType;
		}

		public void Intercept(IInvocation invocation)
		{
			if (!_configurationForType.EnabledCache)
			{
				invocation.Proceed();
				return;
			}

			var method = invocation.Method;
			//ugly hack
			if (method.IsGenericMethod && !_configurationForType.CachedMethods.Contains(method, MethodInfoComparer.Instance))
			{
				invocation.Proceed();
				return;
			}
			var proxy = (ICachingComponent)invocation.Proxy;
			var arguments = invocation.Arguments;

			var keyAndItsDependingKeys = _configurationForType.CacheKey.GetAndPutKey(_configurationForType.ComponentType, proxy, method, arguments);
			if (keyAndItsDependingKeys.Key == null)
			{
				invocation.Proceed();
			}
			else
			{
				var hasCalledOriginalMethod = false;
				var result = _configurationForType.Cache.GetAndPutIfNonExisting(keyAndItsDependingKeys, invocation.Method, () =>
					{
						invocation.Proceed();
						hasCalledOriginalMethod = true;
						return invocation.ReturnValue;
					});
				if (!hasCalledOriginalMethod)
				{
					invocation.ReturnValue = result;
				}
			}
		}
	}
}
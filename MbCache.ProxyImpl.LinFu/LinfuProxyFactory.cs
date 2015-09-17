using System;
using LinFu.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
	[Serializable]
	public class LinFuProxyFactory : IProxyFactory
	{
		private CacheAdapter _cache;
		private static readonly ProxyFactory proxyFactory = new ProxyFactory();

		public void Initialize(CacheAdapter cache)
		{
			_cache = cache;
		}

		public T CreateProxy<T>(ConfigurationForType configurationForType, params object[] parameters) where T : class
		{
			var target = createTarget(configurationForType.ComponentType.ConcreteType, parameters);
			var interceptor = new CacheInterceptor(_cache, configurationForType, target);
			return proxyFactory.CreateProxy<T>(interceptor, typeof(ICachingComponent));
		}

		public T CreateProxyWithTarget<T>(T uncachedComponent, ConfigurationForType configurationForType) where T : class
		{
			var interceptor = new CacheInterceptor(_cache, configurationForType, uncachedComponent);
			return proxyFactory.CreateProxy<T>(interceptor, typeof(ICachingComponent));
		}

		private static object createTarget(Type type, object[] ctorParameters)
		{
			try
			{
				return Activator.CreateInstance(type, ctorParameters);
			}
			catch (MissingMethodException ex)
			{
				var ctorParamMessage = "Incorrect number of parameters to ctor for type " + type;
				throw new ArgumentException(ctorParamMessage, ex);
			}
		}
	}
}
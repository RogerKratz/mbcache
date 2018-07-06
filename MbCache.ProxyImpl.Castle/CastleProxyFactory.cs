using System;
using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
	[Serializable]
	public class CastleProxyFactory : IProxyFactory
	{
		private static readonly ProxyGenerator generator = new ProxyGenerator(new DefaultProxyBuilder(new ModuleScope(false, true)));
		private CacheAdapter _cache;

		public void Initialize(CacheAdapter cache)
		{
			_cache = cache;
		}

		public T CreateProxy<T>(ConfigurationForType configurationForType, params object[] parameters) where T : class
		{
			var cacheInterceptor = new CacheInterceptor(_cache, configurationForType);
			var options = new ProxyGenerationOptions(new CacheProxyGenerationHook(configurationForType.CachedMethods));
			options.AddMixinInstance(new CachingComponent(_cache, configurationForType));
			try
			{
				if (typeof(T).IsClass)
				{
					return (T)generator.CreateClassProxy(configurationForType.ComponentType.ConcreteType, options, parameters, cacheInterceptor);
				}
				var target = Activator.CreateInstance(configurationForType.ComponentType.ConcreteType, parameters);
				return generator.CreateInterfaceProxyWithTarget((T)target, options, cacheInterceptor);
			}
			catch (MissingMethodException createInstanceEx)
			{
				throw new ArgumentException("Cannot instantiate proxy", createInstanceEx);
			}
		}

		public T CreateProxyWithTarget<T>(T uncachedComponent, ConfigurationForType configurationForType) where T : class
		{
			var cacheInterceptor = new CacheInterceptor(_cache, configurationForType);
			var options = new ProxyGenerationOptions(new CacheProxyGenerationHook(configurationForType.CachedMethods));
			options.AddMixinInstance(new CachingComponent(_cache, configurationForType));
			return typeof (T).IsClass ? 
					generator.CreateClassProxyWithTarget(uncachedComponent, options, cacheInterceptor) : 
					generator.CreateInterfaceProxyWithTarget(uncachedComponent, options, cacheInterceptor);
		}
	}
}
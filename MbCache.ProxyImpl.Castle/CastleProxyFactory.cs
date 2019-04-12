using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
	public class CastleProxyFactory : IProxyFactory
	{
		private static readonly ProxyGenerator generator = new ProxyGenerator(new DefaultProxyBuilder(new ModuleScope(false, true)));

		public T CreateProxyWithTarget<T>(T uncachedComponent, ConfigurationForType configurationForType) where T : class
		{
			var cacheInterceptor = new CacheInterceptor(configurationForType);
			var options = new ProxyGenerationOptions(new CacheProxyGenerationHook(configurationForType.CachedMethods));
			options.AddMixinInstance(new CachingComponent(configurationForType));
			return typeof (T).IsClass ? 
					generator.CreateClassProxyWithTarget(uncachedComponent, options, cacheInterceptor) : 
					generator.CreateInterfaceProxyWithTarget(uncachedComponent, options, cacheInterceptor);
		}
	}
}
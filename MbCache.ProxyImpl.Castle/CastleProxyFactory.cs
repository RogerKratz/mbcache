using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
	public class CastleProxyFactory : IProxyFactory
	{
		private static readonly ProxyGenerator generator = new ProxyGenerator(new DefaultProxyBuilder(new ModuleScope(false, true)));

		public T CreateProxy<T>(T target, ConfigurationForType configurationForType) where T : class
		{
			var cacheInterceptor = new CacheInterceptor(configurationForType);
			var options = new ProxyGenerationOptions(new CacheProxyGenerationHook(configurationForType.CachedMethods));
			options.AddMixinInstance(new CachingComponent(configurationForType));
			return typeof (T).IsClass ? 
					generator.CreateClassProxyWithTarget(target, options, cacheInterceptor) : 
					generator.CreateInterfaceProxyWithTarget(target, options, cacheInterceptor);
		}
	}
}
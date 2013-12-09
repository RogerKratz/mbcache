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
		private ICacheKey _cacheKey;
		private ILockObjectGenerator _lockObjectGenerator;

		public void Initialize(CacheAdapter cache, ICacheKey cacheKey, ILockObjectGenerator lockObjectGenerator)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_lockObjectGenerator = lockObjectGenerator;
		}

		public T CreateProxy<T>(ConfigurationForType configurationForType, params object[] parameters) where T : class
		{
			var type = typeof(T);
			var cacheInterceptor = new CacheInterceptor(_cache, _cacheKey, _lockObjectGenerator, configurationForType);
			var options = new ProxyGenerationOptions(new CacheProxyGenerationHook(configurationForType));
			options.AddMixinInstance(createCachingComponent(configurationForType));
			try
			{
				if (type.IsClass)
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

		public bool AllowNonVirtualMember
		{
			get { return true; }
		}

		private ICachingComponent createCachingComponent(ConfigurationForType configurationForType)
		{
			return new CachingComponent(_cache, _cacheKey, configurationForType);
		}
	}
}
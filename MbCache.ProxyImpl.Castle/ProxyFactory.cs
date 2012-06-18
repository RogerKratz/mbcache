using System;
using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
	public class ProxyFactory : IProxyFactory
	{
		private static readonly ProxyGenerator _generator = new ProxyGenerator(new DefaultProxyBuilder(new ModuleScope(false, true)));
		private ICache _cache;
		private ICacheKey _cacheKey;
		private ILockObjectGenerator _lockObjectGenerator;

		public void Initialize(ICache cache, ICacheKey cacheKey, ILockObjectGenerator lockObjectGenerator)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_lockObjectGenerator = lockObjectGenerator;
		}

		public T CreateProxy<T>(ImplementationAndMethods methodData,
										params object[] parameters) where T : class
		{
			var type = typeof(T);
			var cacheInterceptor = new CacheInterceptor(_cache, _cacheKey, _lockObjectGenerator, type);
			var options = new ProxyGenerationOptions(new CacheProxyGenerationHook(methodData.Methods));
			options.AddMixinInstance(createCachingComponent(type, methodData));
			try
			{
				if (type.IsClass)
				{
					return (T)_generator.CreateClassProxy(methodData.ConcreteType, options, parameters, cacheInterceptor);
				}
				var target = Activator.CreateInstance(methodData.ConcreteType, parameters);
				return _generator.CreateInterfaceProxyWithTarget((T)target, options, cacheInterceptor);
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

		private ICachingComponent createCachingComponent(Type type, ImplementationAndMethods details)
		{
			return new CachingComponent(_cache, _cacheKey, type, details);
		}
	}
}
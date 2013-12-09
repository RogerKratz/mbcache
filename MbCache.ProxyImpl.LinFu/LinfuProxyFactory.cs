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
		private ICacheKey _cacheKey;
		private ILockObjectGenerator _lockObjectGenerator;
		private static readonly ProxyFactory _proxyFactory = new ProxyFactory();

		public void Initialize(CacheAdapter cache, ICacheKey cacheKey, ILockObjectGenerator lockObjectGenerator)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_lockObjectGenerator = lockObjectGenerator;
		}

		public T CreateProxy<T>(ConfigurationForType configurationForType, params object[] parameters) where T : class
		{
			var target = createTarget(configurationForType.ComponentType.ConcreteType, parameters);
			var interceptor = new CacheInterceptor(_cache, _cacheKey, _lockObjectGenerator, configurationForType, target);
			return _proxyFactory.CreateProxy<T>(interceptor, typeof(ICachingComponent));
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

		public bool AllowNonVirtualMember
		{
			get { return false; }
		}
	}
}
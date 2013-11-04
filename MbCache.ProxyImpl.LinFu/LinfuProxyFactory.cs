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

		public void Initialize(CacheAdapter cache, ICacheKey cacheKey, ILockObjectGenerator lockObjectGenerator)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_lockObjectGenerator = lockObjectGenerator;
		}

		public T CreateProxy<T>(ConfigurationForType methodData, params object[] parameters) where T : class
		{
			var proxyFactory = new ProxyFactory();
			var target = createTarget<T>(methodData.ConcreteType, parameters);
			var interceptor = new CacheInterceptor(_cache, _cacheKey, _lockObjectGenerator, typeof(T), methodData, target);
			return proxyFactory.CreateProxy<T>(interceptor, typeof(ICachingComponent));
		}

		private static object createTarget<T>(Type type, object[] ctorParameters)
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
using System;
using MbCache.Configuration;
using MbCache.Logic;

namespace MbCacheTest
{
	public class ProxyImplThatThrows : IProxyFactory
	{
		public void Initialize(ICache cache, IMbCacheKey mbCacheKey, ILockObjectGenerator lockObjectGenerator)
		{
		}

		public T CreateProxy<T>(ImplementationAndMethods methodData, params object[] parameters) where T : class
		{
			throw new NotSupportedException("This test creates proxies. Please derive from " + typeof(FullTest).Name + " instead.");
		}

		public bool AllowNonVirtualMember
		{
			get { return false; }
		}
	}
}
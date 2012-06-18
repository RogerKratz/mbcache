using MbCache.Configuration;
using MbCache.Logic;

namespace MbCacheTest.Configuration
{
	public class NullProxyFactory : IProxyFactory
	{
		public void Initialize(ICache cache, ICacheKey cacheKey, ILockObjectGenerator lockObjectGenerator)
		{
		}

		public T CreateProxy<T>(ImplementationAndMethods methodData, params object[] parameters) where T : class
		{
			throw new System.NotImplementedException();
		}

		public bool AllowNonVirtualMember
		{
			get { throw new System.NotImplementedException(); }
		}
	}
}
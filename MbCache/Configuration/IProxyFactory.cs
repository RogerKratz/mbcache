using MbCache.Logic;

namespace MbCache.Configuration
{
	/// <summary>
	/// Creates the proxy. 
	/// The implementation of this interface needs a default ctor
	/// </summary>
	public interface IProxyFactory
	{
		/// <summary>
		/// Called once after this object is instansiated.
		/// </summary>
		void Initialize(CacheAdapter cache);

		/// <summary>
		/// Creates the proxy.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="configurationForType">The method data.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		T CreateProxy<T>(ConfigurationForType configurationForType, params object[] parameters) where T : class;

		/// <summary>
		/// Creates the proxy with a specified target.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="uncachedComponent"></param>
		/// <param name="configurationForType"></param>
		/// <returns></returns>
		T CreateProxyWithTarget<T>(T uncachedComponent, ConfigurationForType configurationForType) where T : class;
	}
}
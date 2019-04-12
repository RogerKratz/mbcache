using MbCache.Logic;

namespace MbCache.Configuration
{
	/// <summary>
	/// Creates the proxy.
	/// </summary>
	public interface IProxyFactory
	{
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
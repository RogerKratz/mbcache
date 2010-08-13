using MbCache.Logic;

namespace MbCache.Configuration
{
    public interface IProxyFactory
    {
        T CreateProxy<T>(ImplementationAndMethods methodData,
                            params object[] parameters);
    }
}
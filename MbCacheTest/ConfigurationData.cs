namespace MbCacheTest
{
    public static class ConfigurationData
    {
        private const string castleProxy = "MbCache.ProxyImpl.Castle.ProxyFactory, MbCache.ProxyImpl.Castle";
        private const string linFuProxy = "MbCache.ProxyImpl.LinFu.ProxyFactory, MbCache.ProxyImpl.Linfu";

        //conf values
        public static string ProxyImpl = linFuProxy;
    }
}
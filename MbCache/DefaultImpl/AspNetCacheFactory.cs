using MbCache.Logic;

namespace MbCache.DefaultImpl
{
    public class AspNetCacheFactory : ICacheFactory
    {
        private readonly int _timeout;

        public AspNetCacheFactory(int timeout)
        {
            _timeout = timeout;
        }

        public ICache Create()
        {
            return new AspNetCache(_timeout);
        }
    }
}
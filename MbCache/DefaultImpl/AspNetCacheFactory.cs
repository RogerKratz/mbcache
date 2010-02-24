using MbCache.Logic;

namespace MbCache.Caches
{
    public class AspNetCacheFactory : ICacheFactory
    {
        private readonly int _timeOut;

        public AspNetCacheFactory(int timeOut)
        {
            _timeOut = timeOut;
        }

        public ICache Create()
        {
            return new AspNetCache(_timeOut);
        }
    }
}

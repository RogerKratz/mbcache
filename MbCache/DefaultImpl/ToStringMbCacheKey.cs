using MbCache.Configuration;

namespace MbCache.DefaultImpl
{
    public class ToStringMbCacheKey : MbCacheKeyBase
    {
        protected override string ParameterValue(object parameter)
        {
            return parameter.ToString();
        }
    }
}
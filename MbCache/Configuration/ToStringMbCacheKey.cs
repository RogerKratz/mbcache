namespace MbCache.Configuration
{
    public class ToStringMbCacheKey : MbCacheKeyBase
    {
        protected override string ParameterValue(object parameter)
        {
            return parameter.ToString();
        }
    }
}
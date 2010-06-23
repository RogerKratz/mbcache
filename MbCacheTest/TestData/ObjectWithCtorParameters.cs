using System;

namespace MbCacheTest.TestData
{
    public interface IObjectWithCtorParameters
    {
        int Value1 { get; set; }
        int Value2 { get; set; }
        Guid CachedMethod();
    }

    public class ObjectWithCtorParameters : IObjectWithCtorParameters
    {
        public ObjectWithCtorParameters(int value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public virtual int Value1 { get; set; }
        public virtual int Value2 { get; set; }

        public virtual Guid CachedMethod()
        {
            return Guid.NewGuid();
        }
    }
}

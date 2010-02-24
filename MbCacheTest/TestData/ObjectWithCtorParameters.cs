namespace MbCacheTest.TestData
{
    public interface IObjectWithCtorParameters
    {
        int Value1 { get; set; }
        int Value2 { get; set; }
        int CachedMethod();
    }

    public class ObjectWithCtorParameters : IObjectWithCtorParameters
    {
        private int counter;

        public ObjectWithCtorParameters(int value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public virtual int Value1 { get; set; }
        public virtual int Value2 { get; set; }

        public virtual int CachedMethod()
        {
            return ++counter;
        }
    }
}

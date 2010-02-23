namespace MbCacheTest.TestData
{
    public class ObjectWithCtorParameters
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

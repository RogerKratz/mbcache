namespace MbCacheTest.TestData
{
    public class ObjectWithNonInterfaceMethod : IObjectWithNonInterfaceMethod
    {
        public int ReturnsFour()
        {
            return 4;
        }
    }

    public interface IObjectWithNonInterfaceMethod{}
}
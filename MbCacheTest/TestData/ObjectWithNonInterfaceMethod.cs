namespace MbCacheTest.TestData;

public class ObjectWithNonInterfaceMethod : IObjectWithNonInterfaceMethod
{
	public virtual int ReturnsFour()
	{
		return 4;
	}
}

public interface IObjectWithNonInterfaceMethod{}
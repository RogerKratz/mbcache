using System.Threading;

namespace MbCacheTest.TestData;

public class ObjectTakes100MsToFill
{
	public virtual int Execute(int a)
	{
		Thread.Sleep(100);
		return a;
	}
}
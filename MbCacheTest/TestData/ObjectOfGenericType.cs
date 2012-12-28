using System;

namespace MbCacheTest.TestData
{
	public interface IObjectOfGenericType<T>
	{
		Guid CachedMethod(int first, T second);
	}

	public class ObjectOfGenericType<T> : IObjectOfGenericType<T>
	{
		 public Guid CachedMethod(int first, T second)
		 {
		 	return Guid.NewGuid();
		 }
	}
}
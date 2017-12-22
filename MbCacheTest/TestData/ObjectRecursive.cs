namespace MbCacheTest.TestData
{
	public class ObjectRecursive1
	{
		private readonly ObjectRecursive2 _objectRecursive2;

		public ObjectRecursive1(ObjectRecursive2 objectRecursive2)
		{
			_objectRecursive2 = objectRecursive2;
		}
		
		public virtual int Ref(int a)
		{
			return _objectRecursive2.Foo(a);
		}
	}

	public class ObjectRecursive2
	{
		public virtual int Foo(int a)
		{
			return a;
		}
	}
}
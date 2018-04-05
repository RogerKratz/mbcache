using System;

namespace MbCacheTest.TestData
{
	public interface IObjectCallingMethodOnItSelf
	{
		Guid NonCachedMethodCallingCachedMethod();
	}
	
	public class ObjectCallingMethodOnItSelf : IObjectCallingMethodOnItSelf
	{
		public Guid NonCachedMethodCallingCachedMethod()
		{
			return CachedMethod();
		}

		public virtual Guid CachedMethod()
		{
			return Guid.NewGuid();
		}
	}
}
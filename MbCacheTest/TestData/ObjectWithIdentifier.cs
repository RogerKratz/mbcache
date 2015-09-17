using System;

namespace MbCacheTest.TestData
{
	public class ObjectWithIdentifier : IObjectWithIdentifier
	{
		public ObjectWithIdentifier()
		{
			Id = Guid.NewGuid();
		}

		public virtual Guid Id { get; }
	}

	public interface IObjectWithIdentifier
	{
		Guid Id { get; }
	}
}
using System;

namespace MbCacheTest.TestData
{
	public class ObjectImplToString
	{
		private readonly Guid _toStringValue;

		public ObjectImplToString(Guid toStringValue)
		{
			_toStringValue = toStringValue;
		}

		public override string ToString()
		{
			return _toStringValue.ToString();
		}
	}
}
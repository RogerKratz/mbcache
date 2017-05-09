using System;
using System.Linq;

namespace MbCache.Configuration
{
	/// <summary>
	/// Holds an array of lock objects.
	/// Based on key, it will return one of these.
	/// </summary>
	[Serializable]
	public class FixedNumberOfLockObjects : ILockObjectGenerator
	{
		private readonly object[] _lockobjects;

		public FixedNumberOfLockObjects(int spread)
		{
			_lockobjects = Enumerable.Repeat(0, spread)
				.Select(x => new object())
				.ToArray();
		}

		public object GetFor(string key)
		{
			var lockObjectIndex = (key.GetHashCode() & 0x7FFFFFFF) % _lockobjects.Length;
			return _lockobjects[lockObjectIndex];
		}
	}
}
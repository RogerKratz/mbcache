using System.Linq;

namespace MbCache.Configuration
{
	/// <summary>
	/// Holds an array of lock objects.
	/// Based on key, it will return one of these.
	/// </summary>
	public class DefaultLockObjectGenerator : ILockObjectGenerator
	{
		private readonly object[] _lockobjects;

		public DefaultLockObjectGenerator(int spread)
		{
			_lockobjects = Enumerable.Range(0, spread)
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
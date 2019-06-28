using System.Linq;

namespace MbCache.Configuration
{
	public class LockObjectGenerator
	{
		private readonly object[] _lockObjects;
		
		public LockObjectGenerator(int numberOfLocksToUse)
		{
			_lockObjects = Enumerable.Repeat(0, numberOfLocksToUse)
				.Select(x => new object())
				.ToArray();
		}

        public object GetFor(KeyAndItsDependingKeys keyAndItsDependingKeys)
        {
        	var lockObjectIndex = (keyAndItsDependingKeys.Key.GetHashCode() & 0x7FFFFFFF) % _lockObjects.Length;
        	return _lockObjects[lockObjectIndex];
        }
	}
}
using System;
using System.Collections.Generic;

namespace MbCache.Configuration
{
	public struct KeyAndItsDependingKeys
	{
		public KeyAndItsDependingKeys(string key, Func<IEnumerable<string>> dependingRemoveKeys)
		{
			Key = key;
			DependingRemoveKeys = dependingRemoveKeys;
		}

		public string Key { get; }
		public Func<IEnumerable<string>> DependingRemoveKeys { get; }
	}
}
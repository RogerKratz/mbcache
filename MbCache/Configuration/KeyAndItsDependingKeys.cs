using System;
using System.Collections.Generic;

namespace MbCache.Configuration;

public readonly struct KeyAndItsDependingKeys(string key, Func<IEnumerable<string>> dependingRemoveKeys)
{
	public string Key { get; } = key;
	public Func<IEnumerable<string>> DependingRemoveKeys { get; } = dependingRemoveKeys;
}
using System;
using System.Linq;
using System.Runtime.Caching;
using MbCacheTest;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

[assembly: ClearMemoryCacheBeforeEachTest]

namespace MbCacheTest;

public class ClearMemoryCacheBeforeEachTestAttribute : Attribute, ITestAction
{
	public ActionTargets Targets => ActionTargets.Test;

	public void BeforeTest(ITest testDetails)
	{
		foreach (var cacheKey in MemoryCache.Default.Select(kvp => kvp.Key).ToList())
		{
			MemoryCache.Default.Remove(cacheKey);
		}
	}

	public void AfterTest(ITest testDetails)
	{
	}
}
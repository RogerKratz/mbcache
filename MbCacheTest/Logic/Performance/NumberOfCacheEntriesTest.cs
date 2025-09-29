using System;
using System.Runtime.Caching;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Performance;

public class NumberOfCacheEntriesTest : TestCase
{
	private ObjectWithMultipleParameters component;
	private MemoryCache memoryCacheReference;
	

	protected override void TestSetup()
	{
		memoryCacheReference = MemoryCache.Default;
		CacheBuilder.For<ObjectWithMultipleParameters>()
			.CacheMethod(x => x.Calculate(0, null, 0))
			.AsImplemented();
		component = CacheBuilder.BuildFactory().Create<ObjectWithMultipleParameters>();
	}

	[Test]
	public void ShouldOnlyCreateOneCacheEntryForMultipleParameters()
	{
		component.Calculate(1, "1", 1);
		var noOfCacheItemsBefore = memoryCacheReference.GetCount();
		component.Calculate(2, "2", 2);
		(memoryCacheReference.GetCount() - noOfCacheItemsBefore).Should().Be.EqualTo(1);
	}

	[Test]
	public void ShouldNotCreateCacheEntryForSameParameters()
	{
		component.Calculate(1, "1", 1);
		var noOfCacheItemsBefore = memoryCacheReference.GetCount();
		component.Calculate(1, "1", 1);
		(memoryCacheReference.GetCount() - noOfCacheItemsBefore).Should().Be.EqualTo(0);
	}
}
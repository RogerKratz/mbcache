using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Scope;

public class InvalidateAllScopesTest
{
	private IMbCacheFactory factory;

	[SetUp]
	public void Setup()
	{
		factory = new CacheBuilder()
			.SetCacheKey(new CacheKeyWithScope())
			.For<ObjectWithMultipleParameters>()
			.CacheMethod(c => c.Calculate(0, "", 0))
			.PerInstance()
			.AsImplemented()
			.BuildFactory();
	}

	[Test]
	public void InvalidateAll()
	{
		var instance = factory.Create<ObjectWithMultipleParameters>();
		CacheKeyWithScope.CurrentScope = "FirstScope";
		var result = instance.Calculate(0, "", 0);
		CacheKeyWithScope.CurrentScope = "SecondScope";
		factory.Invalidate();
		CacheKeyWithScope.CurrentScope = "FirstScope";
		instance.Calculate(0, "", 0)
			.Should().Not.Be.EqualTo(result);
	}

	[Test]
	public void InvalidatePerType()
	{
		var instance = factory.Create<ObjectWithMultipleParameters>();
		CacheKeyWithScope.CurrentScope = "FirstScope";
		var result = instance.Calculate(0, "", 0);
		CacheKeyWithScope.CurrentScope = "SecondScope";
		factory.Invalidate<ObjectWithMultipleParameters>();
		CacheKeyWithScope.CurrentScope = "FirstScope";
		instance.Calculate(0, "", 0)
			.Should().Not.Be.EqualTo(result);
	}

	[Test]
	public void InvalidatePerInstance()
	{
		var instance = factory.Create<ObjectWithMultipleParameters>();
		CacheKeyWithScope.CurrentScope = "FirstScope";
		var result = instance.Calculate(0, "", 0);
		CacheKeyWithScope.CurrentScope = "SecondScope";
		factory.Invalidate(instance);
		CacheKeyWithScope.CurrentScope = "FirstScope";
		instance.Calculate(0, "", 0)
			.Should().Not.Be.EqualTo(result);
	}

	[Test]
	public void InvalidatePerMethod()
	{
		var instance = factory.Create<ObjectWithMultipleParameters>();
		CacheKeyWithScope.CurrentScope = "FirstScope";
		var result = instance.Calculate(0, "", 0);
		CacheKeyWithScope.CurrentScope = "SecondScope";
		factory.Invalidate(instance, x => x.Calculate(1, "1", 1), false);
		CacheKeyWithScope.CurrentScope = "FirstScope";
		instance.Calculate(0, "", 0)
			.Should().Not.Be.EqualTo(result);
	}

	[Test]
	public void InvalidatePerMethodValues()
	{
		var instance = factory.Create<ObjectWithMultipleParameters>();
		CacheKeyWithScope.CurrentScope = "FirstScope";
		var result = instance.Calculate(0, "", 0);
		CacheKeyWithScope.CurrentScope = "SecondScope";
		factory.Invalidate(instance, x => x.Calculate(0, "", 0), true);
		CacheKeyWithScope.CurrentScope = "FirstScope";
		instance.Calculate(0, "", 0)
			.Should().Not.Be.EqualTo(result);
	}
}
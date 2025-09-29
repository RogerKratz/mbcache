using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class KnownInstanceTest : TestCase
{
	private IMbCacheFactory factory;
	
	protected override void TestSetup()
	{
		CacheBuilder
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.CacheMethod(c => c.CachedMethod2())
			.As<IObjectReturningNewGuids>();

		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void CanAskFactoryIfComponentIsKnownType()
	{
		factory.IsKnownInstance(factory.Create<IObjectReturningNewGuids>()).Should().Be.True();
		factory.IsKnownInstance(new object()).Should().Be.False();
	}
}
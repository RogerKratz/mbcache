using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class KnownInstanceTest
{
	private IMbCacheFactory factory;
	
	[SetUp]
	public void Setup() =>
		factory = new CacheBuilder()
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.CacheMethod(c => c.CachedMethod2())
			.As<IObjectReturningNewGuids>()
			.BuildFactory();

	[Test]
	public void CanAskFactoryIfComponentIsKnownType()
	{
		factory.IsKnownInstance(factory.Create<IObjectReturningNewGuids>()).Should().Be.True();
		factory.IsKnownInstance(new object()).Should().Be.False();
	}
}
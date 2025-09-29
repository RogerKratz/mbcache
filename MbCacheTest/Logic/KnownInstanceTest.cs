using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

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
		Assert.IsTrue(factory.IsKnownInstance(factory.Create<IObjectReturningNewGuids>()));
		Assert.IsFalse(factory.IsKnownInstance(new object()));
	}
}
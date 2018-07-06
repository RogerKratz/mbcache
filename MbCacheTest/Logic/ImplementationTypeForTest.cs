using System;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class ImplementationTypeForTest : FullTest
	{
		public ImplementationTypeForTest(Type proxyType) : base(proxyType)
		{
		}

		[Test]
		public void ShouldReturnCorrectTypeForInterface()
		{
			CacheBuilder
				.For<ObjectReturningNewGuids>()
				.CacheMethod(c => c.CachedMethod())
				.As<IObjectReturningNewGuids>();
			var factory = CacheBuilder.BuildFactory();
			factory.ImplementationTypeFor(typeof(IObjectReturningNewGuids))
				.Should().Be.EqualTo(typeof (ObjectReturningNewGuids));
		}

		[Test] 
		public void ShouldReturnCorrectTypeForClass()
		{
			CacheBuilder
				.For<ObjectWithNonInterfaceMethod>()
				.CacheMethod(c => c.ReturnsFour())
				.AsImplemented();
			var factory = CacheBuilder.BuildFactory();
			factory.ImplementationTypeFor(typeof(ObjectWithNonInterfaceMethod))
				.Should().Be.EqualTo(typeof(ObjectWithNonInterfaceMethod));
		}

		[Test]
		public void ShouldThrowIfUnknownType()
		{
			CacheBuilder
				.For<ObjectWithNonInterfaceMethod>()
				.CacheMethod(c => c.ReturnsFour())
				.AsImplemented();
			var factory = CacheBuilder.BuildFactory();
			Assert.Throws<ArgumentException>(() =>
				factory.ImplementationTypeFor(typeof(IObjectReturningNewGuids))
			);
		}
	}
}
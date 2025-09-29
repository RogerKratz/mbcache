using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class ProxyExceptionTest : TestCase
{
	private IMbCacheFactory factory;

	public ProxyExceptionTest(Type proxyType) : base(proxyType)
	{
	}

	protected override void TestSetup()
	{
		CacheBuilder
			.For<ObjectThrowingError>()
			.CacheMethod(c => c.ThrowsArgumentOutOfRangeException())
			.AsImplemented();

		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void ShouldThrowOriginalException()
	{
		var comp = factory.Create<ObjectThrowingError>();
		Assert.Throws<ArgumentOutOfRangeException>(() => comp.ThrowsArgumentOutOfRangeException());
	}

	[Test]
	public void ShouldKeepCorrectStackTract()
	{
		var comp = factory.Create<ObjectThrowingError>();
		try
		{
			comp.ThrowsArgumentOutOfRangeException();
		}
		catch (ArgumentOutOfRangeException ex)
		{
			ex.ToString().Should().Contain("MbCacheTest.TestData.ObjectThrowingError.ThrowsArgumentOutOfRangeException()");
		}
	}
}
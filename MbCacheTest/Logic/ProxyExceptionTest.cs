using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

public class ProxyExceptionTest
{
	private IMbCacheFactory factory;
	
	[SetUp]
	public void Setup() =>
		factory = new CacheBuilder()
			.For<ObjectThrowingError>()
			.CacheMethod(c => c.ThrowsArgumentOutOfRangeException())
			.AsImplemented().BuildFactory();

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
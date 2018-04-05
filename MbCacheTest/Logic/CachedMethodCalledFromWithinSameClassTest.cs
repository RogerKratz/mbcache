using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	[Ignore("Should be fixed")]
	public class CachedMethodCalledFromWithinSameClassTest : FullTest
	{
		private IMbCacheFactory factory;

		
		public CachedMethodCalledFromWithinSameClassTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectCallingMethodOnItSelf>()
				.CacheMethod(c => c.CachedMethod())
				.As<IObjectCallingMethodOnItSelf>();

			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void VerifyNestedMethodWorks()
		{
			var obj1 = factory.Create<IObjectCallingMethodOnItSelf>();
			var obj2 = factory.Create<IObjectCallingMethodOnItSelf>();
			
			obj1.NonCachedMethodCallingCachedMethod()
				.Should().Be.EqualTo(obj2.NonCachedMethodCallingCachedMethod());
		}
	}
}
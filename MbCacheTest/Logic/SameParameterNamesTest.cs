using System;
using MbCache.Core;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class SameParameterNamesTest : FullTest
	{
		private IMbCacheFactory factory;

		public SameParameterNamesTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<SomeEntity>()
				.CacheMethod(c => c.Run(0, 0))
				.CacheMethod(c => c.Run(null, null))
				.AsImplemented();
			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldNotShareCacheBasedOnParameterNames()
		{
			var obj = factory.Create<SomeEntity>();
			obj.Run(1, 2)
				.Should().Not.Be.EqualTo(obj.Run("1", "2"));
		}

		public class SomeEntity
		{
			public virtual Guid Run(int a, int b)
			{
				return Guid.NewGuid();
			}
			public virtual Guid Run(string a, string b)
			{
				return Guid.NewGuid();
			}
		}
	}
}
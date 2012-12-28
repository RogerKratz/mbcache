using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.MatchingCorrectMethod
{
	public class OverloadedMethodTest : FullTest
	{
		private ObjectWithOverloadedMethod component;

		public OverloadedMethodTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectWithOverloadedMethod>()
				.CacheMethod(m => m.Something(2))
				.CacheMethod(m => m.Something("d"))
				.AsImplemented();
			component = CacheBuilder.BuildFactory().Create<ObjectWithOverloadedMethod>();
		}

		[Test]
		public void ShouldCacheStringMethod()
		{
			component.Something("d")
				.Should().Be.EqualTo(component.Something("d"));
		}

		[Test]
		public void ShouldCacheIntMethod()
		{
			component.Something(2)
				.Should().Be.EqualTo(component.Something(2));
		}

		[Test]
		public void ShouldNotCacheParameterLessMethod()
		{
			component.Something()
				.Should().Not.Be.EqualTo(component.Something());
		}

		[Test]
		public void ShouldNotShareCacheBetweenOverloadedMethods()
		{
			component.Something(2)
				.Should().Not.Be.EqualTo("2");
		}

		[Test]
		public void ShouldCachePerParameterValue()
		{
			component.Something(2)
				.Should().Not.Be.EqualTo(2);
		}
	}
}
using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class InvalidateCacheTest : FullTest
	{
		private IMbCacheFactory factory;

		public InvalidateCacheTest(string proxyTypeString) : base(proxyTypeString) { }

		protected override void TestSetup()
		{
			CacheBuilder
				 .For<ObjectReturningNewGuids>()
				 .CacheMethod(c => c.CachedMethod())
				 .CacheMethod(c => c.CachedMethod2())
				 .As<IObjectReturningNewGuids>();

			CacheBuilder
				.For<ObjectWithParametersOnCachedMethod>()
				.CacheMethod(c => c.CachedMethod(null))
				.As<IObjectWithParametersOnCachedMethod>();

			factory = CacheBuilder.BuildFactory();
		}


		[Test]
		public void VerifyInvalidateByType()
		{
			var obj = factory.Create<IObjectReturningNewGuids>();
			var value1 = obj.CachedMethod();
			var value2 = obj.CachedMethod2();
			Assert.AreEqual(value1, obj.CachedMethod());
			Assert.AreEqual(value2, obj.CachedMethod2());
			Assert.AreNotEqual(value1, value2);
			factory.Invalidate<IObjectReturningNewGuids>();
			Assert.AreNotEqual(value1, obj.CachedMethod());
			Assert.AreNotEqual(value2, obj.CachedMethod2());
		}

		[Test]
		public void InvalidatingNonCachingComponentThrows()
		{
			Assert.Throws<ArgumentException>(() => factory.Invalidate(3));
		}

		[Test]
		public void InvalidatingNonCachingComponentAndMethodThrows()
		{
			Assert.Throws<ArgumentException>(() => factory.Invalidate(3, theInt => theInt.CompareTo(44), false));
		}

		[Test]
		public void VerifyInvalidateByInstance()
		{
			var obj = factory.Create<IObjectReturningNewGuids>();
			var obj2 = factory.Create<IObjectReturningNewGuids>();
			var value1 = obj.CachedMethod();
			var value2 = obj2.CachedMethod2();
			Assert.AreEqual(value1, obj.CachedMethod());
			Assert.AreEqual(value2, obj.CachedMethod2());
			Assert.AreNotEqual(value1, value2);
			factory.Invalidate(obj);
			Assert.AreNotEqual(value1, obj.CachedMethod());
			Assert.AreNotEqual(value2, obj.CachedMethod2());
		}


		[Test]
		public void InvalidateByCachingComponent()
		{
			var obj = factory.Create<IObjectReturningNewGuids>();
			var value = obj.CachedMethod();

			((ICachingComponent)obj).Invalidate();
			Assert.AreNotEqual(value, obj.CachedMethod());
		}

		[Test]
		public void InvalidateSpecificMethod()
		{
			var obj = factory.Create<IObjectReturningNewGuids>();
			var value1 = obj.CachedMethod();
			var value2 = obj.CachedMethod2();
			Assert.AreEqual(value1, obj.CachedMethod());
			Assert.AreEqual(value2, obj.CachedMethod2());
			Assert.AreNotEqual(value1, value2);
			factory.Invalidate(obj, method => obj.CachedMethod(), false);
			Assert.AreNotEqual(value1, obj.CachedMethod());
			Assert.AreEqual(value2, obj.CachedMethod2());
		}

		[Test]
		public void InvalidateSpecificMethodWithSpecificParameter()
		{
			var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
			var value1 = obj.CachedMethod("roger");
			var value2 = obj.CachedMethod("moore");
			factory.Invalidate(obj, method => method.CachedMethod("roger"), true);

			value1.Should().Not.Be.EqualTo(obj.CachedMethod("roger"));
			value2.Should().Be.EqualTo(obj.CachedMethod("moore"));
		}

		[Test]
		public void InvalidateSpecificMethodWithSpecificParameterNoConstant()
		{
			var invalidParamObject = new ObjectImplToString(Guid.NewGuid());
			var anotherParamObject = new ObjectImplToString(Guid.NewGuid());

			var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
			var value1 = obj.CachedMethod(invalidParamObject);
			var value2 = obj.CachedMethod(anotherParamObject);
			factory.Invalidate(obj, method => method.CachedMethod(invalidParamObject), true);

			value1.Should().Not.Be.EqualTo(obj.CachedMethod(invalidParamObject));
			value2.Should().Be.EqualTo(obj.CachedMethod(anotherParamObject));
		}

		[Test]
		public void InvalidateSpecificMethodWithSpecificParameterNotUsed()
		{
			var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
			var value1 = obj.CachedMethod("roger");
			var value2 = obj.CachedMethod("moore");
			factory.Invalidate(obj, method => method.CachedMethod("roger"), false);

			value1.Should().Not.Be.EqualTo(obj.CachedMethod("roger"));
			value2.Should().Not.Be.EqualTo(obj.CachedMethod("moore"));
		}

		[Test]
		public void InvalidateSpecificMethodWithParameterBeforeUse()
		{
			const string parameter = "roger";
			var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
			factory.Invalidate(obj, method => method.CachedMethod(parameter), true);
			obj.CachedMethod(parameter).Should().Be.EqualTo(obj.CachedMethod(parameter));
		}
	}
}

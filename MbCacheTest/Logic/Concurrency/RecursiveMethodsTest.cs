using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Concurrency
{
	public class RecursiveMethodsTest : TestCase
	{
		private IMbCacheFactory factory;
		
		public RecursiveMethodsTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectRecursive1>()
				.CacheMethod(c => c.Ref(1))
				.AsImplemented();
			CacheBuilder.For<ObjectRecursive2>()
				.CacheMethod(c => c.Foo(1))
				.AsImplemented();
			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldNotDeadLockDueToRecursiveCalls()
		{
			var instance2 = factory.Create<ObjectRecursive2>();
			var instance1 = factory.Create<ObjectRecursive1>(instance2);
			var tasks = new List<Task>();
			
			500.Times(x =>
			{
				tasks.Add(Task.Factory.StartNew(() =>
				{
					instance1.Ref(x)
						.Should().Be.EqualTo(x);
				}));
			});
			
			Task.WaitAll(tasks.ToArray());
		}

		[Test]
		public void ShouldHaveProperLocksBetweenNestedAndNonNested()
		{
			const int repeat = 10;
			var instance2 = factory.Create<ObjectRecursive2>();
			var instance1 = factory.Create<ObjectRecursive1>(instance2);
			
			for (var i = 0; i < repeat; i++)
			{
				factory.Invalidate();
				instance1.NumberOfCalls = 0;
				instance2.NumberOfCalls = 0;

				var tasks = new List<Task>
				{
					Task.Factory.StartNew(() => { instance1.Ref(1); }),
					Task.Factory.StartNew(() => { instance2.Foo(1); })
				};
				Task.WaitAll(tasks.ToArray());
			
				instance1.NumberOfCalls.Should().Be.EqualTo(1);
				instance2.NumberOfCalls.Should().Be.EqualTo(1);
			}
		}
	}
}
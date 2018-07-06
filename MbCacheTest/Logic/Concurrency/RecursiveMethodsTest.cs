using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.Concurrency
{
	public class RecursiveMethodsTest : TestCase
	{
		public RecursiveMethodsTest(Type proxyType) : base(proxyType)
		{
		}
		
		[Test]
		public void ShouldHandleRecursiveCalls()
		{
			CacheBuilder.For<ObjectRecursive1>()
				.CacheMethod(c => c.Ref(1))
				.AsImplemented();
			CacheBuilder.For<ObjectRecursive2>()
				.CacheMethod(c => c.Foo(1))
				.AsImplemented();

			var factory = CacheBuilder.BuildFactory();
			var instance2 = factory.Create<ObjectRecursive2>();
			var instance1 = factory.Create<ObjectRecursive1>(instance2);
			
			var tasks = new List<Task>();
			
			500.Times(x =>
			{
				tasks.Add(Task.Factory.StartNew(() =>
				{
					instance1.Ref(x);
				}));
			});
			
			Task.WaitAll(tasks.ToArray());
		}
	}
}
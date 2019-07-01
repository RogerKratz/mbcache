using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.Concurrency
{
	/*
	 * Verifies that changing state + invalidation work in combination.
	 * Not sure that this should be supported. May be deleted in the future...
	 * This forces a shared lock between ICache's GetAndPutIfNonExisting and Delete
	 */
	public class CacheInvalidationOfMutatedListTest : TestCase
	{
		private IMbCacheFactory factory;
		private ObjectWithMutableList instance;

		public CacheInvalidationOfMutatedListTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder
				.For<ObjectWithMutableList>()
				.CacheMethod(c => c.GetListContents())
				.AsImplemented();

			factory = CacheBuilder.BuildFactory();
			instance = factory.Create<ObjectWithMutableList>();
		}

		[Test]
		[Repeat(10)]
		public void ShouldNotGetDuplicatesInList_InvalidateAll()
		{
			runInParallel(() => factory.Invalidate());
		}
		
		[Test]
		[Repeat(10)]
		public void ShouldNotGetDuplicatesInList_InvalidateOnType()
		{
			runInParallel(() => factory.Invalidate<ObjectWithMutableList>());
		}

		[Test]
		[Repeat(10)]
		public void ShouldNotGetDuplicatesInList_InvalidateOnInstance()
		{
			runInParallel(() => factory.Invalidate(instance));
		}

		[Test]
		[Repeat(10)]
		public void ShouldNotGetDuplicatesInList_InvalidateOnMethod()
		{
			//add one for "true" if needed
			runInParallel(() => factory.Invalidate(instance, x => x.GetListContents(), false));
		}

		private void runInParallel(Action invalidate)
		{
			var lockK = new object();
			var tasks = new List<Task>();

			200.Times(d =>
			{
				var getOrAddIfMissing = d.ToString();

				100.Times(() =>
				{
					tasks.Add(Task.Factory.StartNew(() =>
					{
						var match = instance.GetListContents().SingleOrDefault(x => x == getOrAddIfMissing);
						if (match != null)
							return;

						lock (lockK)
						{
							match = instance.GetListContents().SingleOrDefault(x => x == getOrAddIfMissing);
							if (match != null)
								return;
							instance.AddToList(getOrAddIfMissing);
							invalidate();
							Console.WriteLine("This line makes test fail more often.");
						}
					}));
				});
			});

			Task.WaitAll(tasks.ToArray());
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.Concurrency
{
	public class CacheInvalidationOfMutatedListTest : FullTest
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
		public void ShouldNotGetDuplicatesInList_InvalidateOnType()
		{
			runInParallell(() => factory.Invalidate<ObjectWithMutableList>());
		}

		[Test]
		[Repeat(10)]
		public void ShouldNotGetDuplicatesInList_InvalidateOnInstance()
		{
			runInParallell(() => factory.Invalidate(instance));
		}

		[Test]
		[Repeat(10)]
		public void ShouldNotGetDuplicatesInList_InvalidateOnMethod()
		{
			//add one for "true" if needed
			runInParallell(() => factory.Invalidate(instance, x => x.GetListContents(), false));
		}

		private void runInParallell(Action invalidate)
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

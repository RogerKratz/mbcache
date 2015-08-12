using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic.Concurrency
{
	[TestFixture]
	public class CacheInvalidationOfMutatedListTest : FullTest
	{
		public CacheInvalidationOfMutatedListTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			base.TestSetup();
			LogManager.Shutdown();
		}

		[TearDown]
		public void AfterTest()
		{
			SetupFixtureForAssembly.StartLog4Net();
		}

		[Test]
		public void ShouldNotGetDuplicatesInList([Range(1, 10)] int attempt)
		{
			CacheBuilder
				.For<ObjectWithMutableList>()
				.CacheMethod(c => c.GetListContents())
				.AsImplemented();

			var factory = CacheBuilder.BuildFactory();

			var instance = factory.Create<ObjectWithMutableList>();
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
							factory.Invalidate(instance);
						}

					}));
				});
			});

			Task.WaitAll(tasks.ToArray());
		}
	}
}

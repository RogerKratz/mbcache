using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;
using log4net;
using log4net.Core;

namespace MbCacheTest.Logic
{
	public class CacheKeyTest : FullTest
	{
		private IMbCacheFactory factory;

		public CacheKeyTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldGiveLogWarningIfSuspectedParameterIsUsed()
		{
			var comp = factory.Create<IObjectWithParametersOnCachedMethod>();
			using (var log = new LogSpy(LogManager.GetLogger(typeof(CacheKeyBase)), Level.Warn))
			{
				comp.CachedMethod(this);
				var logOutput = log.RenderedMessages();
				logOutput.Should().Contain(GetType().ToString());
			}
		}
	}
}
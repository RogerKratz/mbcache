using log4net.Config;
using NUnit.Framework;

namespace MbCacheTest
{
    [SetUpFixture]
    public class SetupFixtureForAssembly
    {
        [SetUp]
        public void RunOnce()
        {
            StartLog4Net();
        }

        public static void StartLog4Net()
        {
            BasicConfigurator.Configure();            
        }
    }
}
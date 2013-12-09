using System;
using log4net;
using log4net.Core;

namespace MbCacheTest
{
	public class NoLogger : IDisposable
	{
		private readonly Level levelBefore;

		public NoLogger()
		{
			var repo = LogManager.GetRepository();
			levelBefore = repo.Threshold;
			repo.Threshold = Level.Off;
		}

		public void Dispose()
		{
			LogManager.GetRepository().Threshold = levelBefore;
		}
	}
}
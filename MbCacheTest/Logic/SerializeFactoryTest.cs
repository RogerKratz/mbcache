using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
	public class SerializeFactoryTest : FullTest
	{
		private IMbCacheFactory factory;

		public SerializeFactoryTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ReturningRandomNumbers>()
				.CacheMethod(c => c.CachedNumber())
				.CacheMethod(c => c.CachedNumber2())
				.As<IReturningRandomNumbers>();

			var factoryTemp = CacheBuilder.BuildFactory();
			factory = serializeAndDeserialize(factoryTemp);
		}

		private static IMbCacheFactory serializeAndDeserialize(IMbCacheFactory mbCacheFactory)
		{
			var formatter = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, mbCacheFactory);
				stream.Position = 0;
				return (IMbCacheFactory) formatter.Deserialize(stream);
			}
		}

		[Test]
		public void VerifyCacheIsWorking()
		{
			var obj1 = factory.Create<IReturningRandomNumbers>();
			var obj2 = factory.Create<IReturningRandomNumbers>();
			Assert.AreEqual(obj1.CachedNumber(), obj2.CachedNumber());
			Assert.AreEqual(obj1.CachedNumber2(), obj2.CachedNumber2());
			Assert.AreNotEqual(obj1.CachedNumber(), obj1.CachedNumber2());
			Assert.AreNotEqual(obj1.NonCachedNumber(), obj2.NonCachedNumber());
		}
	}
}
using System;

namespace MbCacheTest.TestData;

public interface IReturningRandomNumbers
{
	int CachedNumber();
	int CachedNumber2();
	int NonCachedNumber();
}

public class ReturningRandomNumbers : IReturningRandomNumbers
{
	private readonly Random r = new Random();

	public int CachedNumber()
	{
		return r.Next();
	}

	public int CachedNumber2()
	{
		return r.Next();
	}

	public int NonCachedNumber()
	{
		return r.Next();
	}
}
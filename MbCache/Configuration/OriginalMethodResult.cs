namespace MbCache.Configuration;

public class OriginalMethodResult(object value, bool shouldBeCached)
{
	public object Value { get; } = value;
	public bool ShouldBeCached { get; } = shouldBeCached;
}
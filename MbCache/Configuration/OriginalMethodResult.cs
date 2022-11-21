namespace MbCache.Configuration
{
	public class OriginalMethodResult
	{
		public OriginalMethodResult(object value, bool shouldBeCached)
		{
			Value = value;
			ShouldBeCached = shouldBeCached;
		}
		
		public object Value { get; }
		public bool ShouldBeCached { get; }
	}
}
using System.Collections.Generic;

namespace MbCacheTest.TestData
{
	public class ObjectWithComplexProperties
	{
		public string StringProp { get; set; } 
		public double DoubleProp { get; set; } 
		public IList<ObjectWithProperty> GenericListProp { get; set; } = new List<ObjectWithProperty>();
	}
}
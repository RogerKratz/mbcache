using System.Collections.Generic;
using System.Linq;

namespace MbCacheTest.TestData
{
	public class ObjectWithMutableList
	{
		private readonly IList<string> _strings = new List<string>();

		public virtual void AddToList(string aStuff)
		{
			_strings.Add(aStuff);
		}

		public virtual IEnumerable<string> GetListContents()
		{
			return _strings.ToArray();
		}
	}
}
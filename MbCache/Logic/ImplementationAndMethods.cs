using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Logic
{
    public class ImplementationAndMethods
    {
        public ImplementationAndMethods(object implementation)
        {
            Methods = new HashSet<MethodInfo>();
            Implementation = implementation;
        }

        public ICollection<MethodInfo> Methods { get; private set; }
        public object Implementation { get; private set; }
    }
}
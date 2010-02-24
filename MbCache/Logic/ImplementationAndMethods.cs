using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Logic
{
    public class ImplementationAndMethods
    {
        public ImplementationAndMethods(object[] ctorParams, object implementation)
        {
            Methods = new HashSet<MethodInfo>();
            CtorParameters = ctorParams;
            Implementation = implementation;
        }

        public ICollection<MethodInfo> Methods { get; private set; }
        public object[] CtorParameters { get; private set; }
        public object Implementation { get; private set; }
    }
}
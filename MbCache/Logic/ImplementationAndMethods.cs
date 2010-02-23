using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Logic
{
    public class ImplementationAndMethods
    {
        public ImplementationAndMethods(object[] ctorParams)
        {
            Methods = new HashSet<MethodInfo>();
            CtorParameters = ctorParams;
        }

        public ICollection<MethodInfo> Methods { get; private set; }
        public object[] CtorParameters { get; private set; }
    }
}
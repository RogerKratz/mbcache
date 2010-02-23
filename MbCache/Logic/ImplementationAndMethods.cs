using System;
using System.Collections.Generic;

namespace MbCache.Logic
{
    public class ImplementationAndMethods
    {
        public ImplementationAndMethods(Type implementationType, object[] ctorParams)
        {
            ImplementationType = implementationType;
            Methods = new HashSet<string>();
            CtorParameters = ctorParams;
        }

        public Type ImplementationType { get; private set; }
        public ICollection<string> Methods { get; private set; }
        public object[] CtorParameters { get; private set; }

    }
}
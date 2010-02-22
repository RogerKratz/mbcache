using System;
using System.Collections.Generic;

namespace MbCache.CoreImpl
{
    public class ImplementationAndMethods
    {
        public ImplementationAndMethods(Type implementationType)
        {
            ImplementationType = implementationType;
            Methods = new HashSet<string>();
        }

        public Type ImplementationType { get; private set; }
        public ICollection<string> Methods { get; private set; }

    }
}

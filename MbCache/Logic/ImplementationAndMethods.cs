using System;
using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Logic
{
    public class ImplementationAndMethods
    {
        public ImplementationAndMethods(Type concreteType)
        {
            Methods = new HashSet<MethodInfo>();
            ConcreteType = concreteType;
        }

        public ICollection<MethodInfo> Methods { get; private set; }
        public Type ConcreteType { get; private set; }
        public bool CachePerInstance { get; set; }
    }
}
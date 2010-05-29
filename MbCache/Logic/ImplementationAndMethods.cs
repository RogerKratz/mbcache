using System;
using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Logic
{
    public class ImplementationAndMethods
    {
        public ImplementationAndMethods(Delegate ctorDelegate)
        {
            Methods = new HashSet<MethodInfo>();
            CtorDelegate = ctorDelegate;
        }

        public ICollection<MethodInfo> Methods { get; private set; }
        public Delegate CtorDelegate { get; private set; }
        public bool CachePerInstance { get; set; }
    }
}
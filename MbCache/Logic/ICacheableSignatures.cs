using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Logic
{
    public interface ICacheableSignatures
    {
        IEnumerable<MethodInfo> Keys { get;}
    }
}
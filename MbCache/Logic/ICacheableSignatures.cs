using System.Collections.Generic;

namespace MbCache.Logic
{
    public interface ICacheableSignatures
    {
        IEnumerable<string> Keys { get;}
    }
}
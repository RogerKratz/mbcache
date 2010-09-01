using System;
using System.Reflection;

namespace MbCache.Logic
{
    /// <summary>
    /// Builds the cache keys. 
    /// In most cases, derive from MbCacheKeyBase instead.
    /// 
    /// If you implement this interface directly, 
    /// remember every overload with more params
    /// needs to include the less one.
    /// 
    /// Eg, if Key(Type) returns Roger, 
    /// it's valid for Key(Type, MethodInfo) to return Roger2
    /// but _not_ Rog2.
    /// </summary>
    /// <remarks>
    /// Created by: rogerkr
    /// Created date: 2010-09-01
    /// </remarks>
    public interface IMbCacheKey
    {
        string Key(Type type);
        string Key(Type type, ICachingComponent component);
        string Key(Type type, ICachingComponent component, MethodInfo method);
        string Key(Type type, ICachingComponent component, MethodInfo method, object[] parameters);
    }
}
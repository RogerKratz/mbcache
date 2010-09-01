using System;
using System.Reflection;
using System.Text;
using MbCache.Logic;

namespace MbCache.Configuration
{
    /// <summary>
    /// Base class for users to override to implement
    /// their own logic for building cache keys
    /// </summary>
    /// <remarks>
    /// Created by: rogerkr
    /// Created date: 2010-03-02
    /// </remarks>
    public abstract class MbCacheKeyBase : IMbCacheKey
    {
        public string Key(Type type)
        {
            return string.Concat(KeyStart + type + Separator);
        }

        public string Key(Type type, ICachingComponent component)
        {
            return string.Concat(Key(type), component.UniqueId, Separator);
        }

        public string Key(Type type, ICachingComponent component, MethodInfo method)
        {
            var ret = new StringBuilder(Key(type, component));
            ret.Append(method.Name);
            ret.Append(Separator);
            foreach (var parameter in method.GetParameters())
            {
                ret.Append(parameter.Name);
                ret.Append(Separator);
            }
            return ret.ToString();
        }

        public string Key(Type type, ICachingComponent component, MethodInfo method, object[] parameters)
        {
            var ret = new StringBuilder(Key(type, component, method) + Separator);
            foreach (var parameter in parameters)
            {
                ret.Append(parameter == null ? NullKey : ParameterValue(parameter));
                ret.Append(Separator);
            }
            return ret.ToString();  
        }


        protected virtual string Separator
        {
            get { return "|"; }
        }

        protected virtual string KeyStart
        {
            get { return "MbCache" + Separator; }
        }

        protected virtual string NullKey
        {
            get { return "Null"; }
        }

        protected abstract string ParameterValue(object parameter);
    }
}
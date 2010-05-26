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
        protected const string Separator = "|";

        public string CacheKey(Type type, MethodInfo methodInfo)
        {
            var start = string.Concat(type + Separator + methodInfo.Name + Separator);
            var ret = new StringBuilder(start);
            foreach (var parameter in methodInfo.GetParameters())
            {
                ret.Append(parameter.Name);
                ret.Append(Separator);
            }
            return ret.ToString();
        }

        protected virtual string NullReplacer 
        {
            get
            {
                return "null";
            }
        }

        public string AddForParameterValues(Type type, MethodInfo methodInfo, object[] parameters)
        {
            var ret = new StringBuilder();
            foreach (var parameter in parameters)
            {
                ret.Append(parameter == null ? NullReplacer : KeyPartForParameterValue(type, methodInfo, parameter));

                ret.Append(Separator);
            }
            return ret.ToString();  
        }

        public string AddForComponent(ICachingComponent component)
        {
            return component.UniqueId;
        }

        protected abstract string KeyPartForParameterValue(Type type, MethodInfo info, object parameter);
    }
}
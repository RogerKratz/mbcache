 using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbCache.Core;

namespace MbCache.Configuration
{
	/// <summary>
	/// Base class for users to override to implement
	/// their own logic for building cache keys
	/// 
	/// Will build cache key in format
	/// MbCache|Type|Component|Method|ParamValue1|ParamValue2
	/// </summary>
	public abstract class MbCacheKeyBase : IMbCacheKey
	{
		public string Key(Type type)
		{
			return string.Concat(KeyStart, type, Separator);
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

		public string Key(Type type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
		{
			var ret = new StringBuilder(Key(type, component, method));
			foreach (var parameter in parameters)
			{
				var parameterKey = ParameterValue(parameter);
				if (parameterKey == null)
					return null;
				ret.Append(parameterKey);
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


		/// <summary>
		/// Adds string to cache key for parameter values  
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <returns>
		/// A string reporesentation of the parameter.
		/// </returns>
		protected abstract string ParameterValue(object parameter);
	}
}
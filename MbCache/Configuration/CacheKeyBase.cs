using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MbCache.Core;
using log4net;

namespace MbCache.Configuration
{
	/// <summary>
	/// Base class for users to override to implement
	/// their own logic for building cache keys
	/// 
	/// Will build cache key in format
	/// MbCache|Type
	/// MbCache|Type|Component
	/// MbCache|Type|Component|Method|ParamType1|ParamType2
	/// MbCache|Type|Component|Method|ParamType1|ParamType2|ParamValue1|ParamValue2
	/// </summary>
	[Serializable]
	public abstract class CacheKeyBase : ICacheKey
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof (CacheKeyBase));
		private const string suspisiousParam =
			"Cache key of type {0} equals its own type name. Possible bug in your ICacheKey implementation.";
		private static readonly Regex findSeperator = new Regex(@"\" + separator, RegexOptions.Compiled);
		private const string separator = "|";

		public string Key(Type type)
		{
			return string.Concat(KeyStart, type);
		}

		public string Key(Type type, ICachingComponent component)
		{
			return string.Concat(Key(type), separator, component.UniqueId);
		}

		public string Key(Type type, ICachingComponent component, MethodInfo method)
		{
			var ret = new StringBuilder(Key(type, component));
			ret.Append(separator);
			ret.Append(method.Name);
			foreach (var parameter in method.GetParameters())
			{
				ret.Append(separator);
				ret.Append(parameter.ParameterType);
			}
			return ret.ToString();
		}

		public string Key(Type type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
		{
			var ret = new StringBuilder(Key(type, component, method));
			foreach (var parameter in parameters)
			{
				ret.Append(separator);
				var parameterKey = ParameterValue(parameter);
				if (parameterKey == null)
					return null;
				checkIfSuspiousParameter(parameter, parameterKey);
				ret.Append(parameterKey);
			}
			return ret.ToString();
		}

		public IEnumerable<string> UnwrapKey(string key)
		{
			var keys = new List<string>();
			var matches = findSeperator.Matches(key);
			keys.AddRange(from Match match in matches select key.Substring(0, match.Index));
			return keys;
		}

		private static void checkIfSuspiousParameter(object parameter, string parameterKey)
		{
			if (parameter !=null && logger.IsWarnEnabled)
			{
				var parameterType = parameter.GetType();
				if (parameterKey.Equals(parameterType.ToString()))
				{
					logger.WarnFormat(suspisiousParam, parameterType);
				}
			}
		}

		protected virtual string KeyStart
		{
			get { return "MbCache" + separator; }
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
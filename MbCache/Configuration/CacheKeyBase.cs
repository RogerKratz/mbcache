using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Logic;

namespace MbCache.Configuration
{
	/// <summary>
	/// Base class for users to override to implement
	/// their own logic for building cache keys
	/// 
	/// Will build cache key in format
	/// Type
	/// Type|Component
	/// Type|Component|Method|ParamType1|ParamType2
	/// Type|Component|Method|ParamType1|ParamType2|$ParamValue1$ParamValue2
	/// </summary>
	[Serializable]
	public abstract class CacheKeyBase : ICacheKey
	{
		private readonly string suspiciousParam =
			"Cache key of type {0} equals its own type name. You should specify a value for this parameter in your ICacheKey implementation." + Environment.NewLine +
			"However, even though it's not recommended, you can override this exception by calling AllowDifferentArgumentsShareSameCacheKey when configuring your cached component.";
		private static readonly Regex findSeparator = new Regex(@"\" + separator, RegexOptions.Compiled);
		private const string separator = "|";
		private const string separatorForParameters = "$";

		private IEnumerable<IEventListener> _eventListeners;

		public void Initialize(IEnumerable<IEventListener> eventListeners)
		{
			_eventListeners = eventListeners;
		}

		public string RemoveKey(ComponentType type)
		{
			return type.ToString();
		}

		public string RemoveKey(ComponentType type, ICachingComponent component)
		{
			return string.Concat(RemoveKey(type), separator, component.UniqueId);
		}

		public string RemoveKey(ComponentType type, ICachingComponent component, MethodInfo method)
		{
			return stringBuilderForMethodBody(type, component, method).ToString();
		}

		public string RemoveKey(ComponentType type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
		{
			var ret = stringBuilderForMethodBody(type, component, method);
			ret.Append(separator);
			foreach (var parameter in parameters)
			{
				ret.Append(separatorForParameters);
				var parameterKey = ParameterValue(parameter);
				if (parameterKey == null)
					return null;
				checkIfSuspiciousParameter(component, parameter, parameterKey);
				ret.Append(parameterKey);
			}

			return ret.ToString();
		}

		private StringBuilder stringBuilderForMethodBody(ComponentType type, ICachingComponent component, MethodInfo method)
		{
			var ret = new StringBuilder(RemoveKey(type, component));
			ret.Append(separator);
			ret.Append(method.Name);
			foreach (var parameter in method.GetParameters())
			{
				ret.Append(separatorForParameters);
				ret.Append(parameter.ParameterType);
			}
			return ret;
		}

		public KeyAndItsDependingKeys GetAndPutKey(ComponentType type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
		{
			var startKey = RemoveKey(type, component, method, parameters);
			if (startKey == null)
				return new KeyAndItsDependingKeys();

			var scope = Scope();
			var completeKey = scope == null ? 
				startKey : 
				string.Concat(startKey, separator, scope);
			return new KeyAndItsDependingKeys(completeKey, () => allRemoveKeys(completeKey));
		}

		private static IEnumerable<string> allRemoveKeys(string getAndPutKey)
		{
			var matches = findSeparator.Matches(getAndPutKey);
			return from Match match in matches select string.Intern(getAndPutKey.Substring(0, match.Index));
		}

		private void checkIfSuspiciousParameter(ICachingComponent component, object parameter, string parameterKey)
		{
			if (component.AllowDifferentArgumentsShareSameCacheKey || parameter == null) 
				return;
			if (parameterKey.Equals(parameter.GetType().ToString()))
			{
				throw new ArgumentException(string.Format(suspiciousParam, parameterKey), parameterKey);
			}
		}

		/// <summary>
		/// Adds a string at the beginning of the cache key.
		/// Can be used to have different cache entries based on some logic,
		/// eg in tenant scenarios.
		/// </summary>
		protected virtual string Scope()
		{
			return null;
		}

		/// <summary>
		/// Adds string to cache key for parameter values  
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <returns>
		/// A string representation of the parameter.
		/// </returns>
		protected abstract string ParameterValue(object parameter);
	}
}
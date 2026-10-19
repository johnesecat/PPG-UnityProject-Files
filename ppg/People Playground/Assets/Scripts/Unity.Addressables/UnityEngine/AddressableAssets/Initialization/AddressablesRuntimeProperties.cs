using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.AddressableAssets.Initialization
{
	public static class AddressablesRuntimeProperties
	{
		private static Dictionary<string, string> s_CachedValues = new Dictionary<string, string>();

		private static Assembly[] GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		internal static int GetCachedValueCount()
		{
			return s_CachedValues.Count;
		}

		public static void SetPropertyValue(string name, string val)
		{
			s_CachedValues[name] = val;
		}

		public static void ClearCachedPropertyValues()
		{
			s_CachedValues.Clear();
		}

		public static string EvaluateProperty(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}
			if (s_CachedValues.TryGetValue(name, out var value))
			{
				return value;
			}
			int num = name.LastIndexOf('.');
			if (num < 0)
			{
				return name;
			}
			string name2 = name.Substring(0, num);
			string name3 = name.Substring(num + 1);
			Assembly[] assemblies = GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type type = assemblies[i].GetType(name2, throwOnError: false, ignoreCase: false);
				if (type == null)
				{
					continue;
				}
				try
				{
					PropertyInfo property = type.GetProperty(name3, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					if (property != null)
					{
						object value2 = property.GetValue(null, null);
						if (value2 != null)
						{
							s_CachedValues.Add(name, value2.ToString());
							return value2.ToString();
						}
					}
					FieldInfo field = type.GetField(name3, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					if (field != null)
					{
						object value3 = field.GetValue(null);
						if (value3 != null)
						{
							s_CachedValues.Add(name, value3.ToString());
							return value3.ToString();
						}
					}
				}
				catch (Exception)
				{
				}
			}
			return name;
		}

		public static string EvaluateString(string input)
		{
			return EvaluateString(input, '{', '}', EvaluateProperty);
		}

		public static string EvaluateString(string inputString, char startDelimiter, char endDelimiter, Func<string, string> varFunc)
		{
			if (string.IsNullOrEmpty(inputString))
			{
				return string.Empty;
			}
			while (true)
			{
				int num = inputString.IndexOf(startDelimiter);
				if (num < 0)
				{
					return inputString;
				}
				int num2 = inputString.IndexOf(endDelimiter, num + 1);
				if (num2 < num)
				{
					break;
				}
				string arg = inputString.Substring(num + 1, num2 - num - 1);
				string text = ((varFunc == null) ? string.Empty : varFunc(arg));
				inputString = inputString.Substring(0, num) + text + inputString.Substring(num2 + 1);
			}
			return inputString;
		}
	}
}

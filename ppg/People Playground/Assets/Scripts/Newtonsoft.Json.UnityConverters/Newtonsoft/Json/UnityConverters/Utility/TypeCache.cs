using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.UnityConverters.Utility
{
	internal static class TypeCache
	{
		private static readonly Dictionary<string, Type> _typeByName = new Dictionary<string, Type>();

		private static readonly Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies().Reverse().ToArray();

		public static Type FindType(string name)
		{
			if (_typeByName.TryGetValue(name, out var value))
			{
				return value;
			}
			value = Type.GetType(name);
			if (value != null)
			{
				_typeByName[name] = value;
				return value;
			}
			Assembly[] assemblies = _assemblies;
			for (int i = 0; i < assemblies.Length; i++)
			{
				value = assemblies[i].GetType(name);
				if (value != null)
				{
					_typeByName[name] = value;
					return value;
				}
			}
			return null;
		}
	}
}

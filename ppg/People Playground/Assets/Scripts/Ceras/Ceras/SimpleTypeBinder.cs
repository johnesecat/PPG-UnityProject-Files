using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ceras
{
	public class SimpleTypeBinder : ITypeBinder
	{
		private readonly HashSet<Assembly> _searchAssemblies = new HashSet<Assembly>();

		public HashSet<Assembly> CustomSearchAssemblies { get; } = new HashSet<Assembly>();


		public SimpleTypeBinder()
		{
			foreach (Assembly frameworkAssembly in CerasSerializer._frameworkAssemblies)
			{
				_searchAssemblies.Add(frameworkAssembly);
			}
			_searchAssemblies.Add(Assembly.GetEntryAssembly());
			_searchAssemblies.RemoveWhere((Assembly a) => a == null);
		}

		public string GetBaseName(Type type)
		{
			if (type.IsGenericType)
			{
				return type.GetGenericTypeDefinition().FullName;
			}
			return type.FullName;
		}

		public Type GetTypeFromBase(string baseTypeName)
		{
			foreach (Assembly searchAssembly in _searchAssemblies)
			{
				Type type = searchAssembly.GetType(baseTypeName);
				if (type != null)
				{
					return type;
				}
			}
			foreach (Assembly customSearchAssembly in CustomSearchAssemblies)
			{
				if (!_searchAssemblies.Contains(customSearchAssembly))
				{
					Type type2 = customSearchAssembly.GetType(baseTypeName);
					if (type2 != null)
					{
						_searchAssemblies.Add(customSearchAssembly);
						return type2;
					}
				}
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (!_searchAssemblies.Contains(assembly) && !CustomSearchAssemblies.Contains(assembly))
				{
					Type type3 = assembly.GetType(baseTypeName);
					if (type3 != null)
					{
						_searchAssemblies.Add(assembly);
						return type3;
					}
				}
			}
			throw new Exception("Cannot find type " + baseTypeName + " after searching in all user provided assemblies and all loaded assemblies. Is the type in some plugin-module that was not yet loaded? Or did the assembly that contains the type change (ie the type got removed)?");
		}

		public Type GetTypeFromBaseAndArguments(string baseTypeName, params Type[] genericTypeArguments)
		{
			return GetTypeFromBase(baseTypeName).MakeGenericType(genericTypeArguments);
		}
	}
}

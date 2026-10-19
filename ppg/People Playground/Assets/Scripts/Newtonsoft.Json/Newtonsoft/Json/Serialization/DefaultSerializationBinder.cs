using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class DefaultSerializationBinder : SerializationBinder, ISerializationBinder
	{
		internal static readonly DefaultSerializationBinder Instance = new DefaultSerializationBinder();

		[Nullable(new byte[] { 1, 0, 2, 1, 1 })]
		private readonly ThreadSafeStore<StructMultiKey<string, string>, Type> _typeCache;

		public DefaultSerializationBinder()
		{
			_typeCache = new ThreadSafeStore<StructMultiKey<string, string>, Type>(GetTypeFromTypeNameKey);
		}

		private Type GetTypeFromTypeNameKey([Nullable(new byte[] { 0, 2, 1 })] StructMultiKey<string, string> typeNameKey)
		{
			string value = typeNameKey.Value1;
			string value2 = typeNameKey.Value2;
			if (value != null)
			{
				Assembly assembly = Assembly.LoadWithPartialName(value);
				if (assembly == null)
				{
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					foreach (Assembly assembly2 in assemblies)
					{
						if (assembly2.FullName == value || assembly2.GetName().Name == value)
						{
							assembly = assembly2;
							break;
						}
					}
				}
				if (assembly == null)
				{
					throw new JsonSerializationException("Could not load assembly '{0}'.".FormatWith(CultureInfo.InvariantCulture, value));
				}
				Type type = assembly.GetType(value2);
				if (type == null)
				{
					if (value2.IndexOf('`') >= 0)
					{
						try
						{
							type = GetGenericTypeFromTypeName(value2, assembly);
						}
						catch (Exception innerException)
						{
							throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, value2, assembly.FullName), innerException);
						}
					}
					if (type == null)
					{
						throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, value2, assembly.FullName));
					}
				}
				return type;
			}
			return Type.GetType(value2);
		}

		[return: Nullable(2)]
		private Type GetGenericTypeFromTypeName(string typeName, Assembly assembly)
		{
			Type result = null;
			int num = typeName.IndexOf('[');
			if (num >= 0)
			{
				string name = typeName.Substring(0, num);
				Type type = assembly.GetType(name);
				if (type != null)
				{
					List<Type> list = new List<Type>();
					int num2 = 0;
					int num3 = 0;
					int num4 = typeName.Length - 1;
					for (int i = num + 1; i < num4; i++)
					{
						switch (typeName[i])
						{
						case '[':
							if (num2 == 0)
							{
								num3 = i + 1;
							}
							num2++;
							break;
						case ']':
							num2--;
							if (num2 == 0)
							{
								StructMultiKey<string, string> typeNameKey = ReflectionUtils.SplitFullyQualifiedTypeName(typeName.Substring(num3, i - num3));
								list.Add(GetTypeByName(typeNameKey));
							}
							break;
						}
					}
					result = type.MakeGenericType(list.ToArray());
				}
			}
			return result;
		}

		private Type GetTypeByName([Nullable(new byte[] { 0, 2, 1 })] StructMultiKey<string, string> typeNameKey)
		{
			return _typeCache.Get(typeNameKey);
		}

		public override Type BindToType([Nullable(2)] string assemblyName, string typeName)
		{
			return GetTypeByName(new StructMultiKey<string, string>(assemblyName, typeName));
		}

		[NullableContext(2)]
		public override void BindToName([Nullable(1)] Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = serializedType.Assembly.FullName;
			typeName = serializedType.FullName;
		}
	}
}

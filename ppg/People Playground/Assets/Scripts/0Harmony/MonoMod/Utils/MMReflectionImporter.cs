using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;

namespace MonoMod.Utils
{
	internal sealed class MMReflectionImporter : IReflectionImporter
	{
		private class _Provider : IReflectionImporterProvider
		{
			public bool? UseDefault;

			public IReflectionImporter GetReflectionImporter(ModuleDefinition module)
			{
				MMReflectionImporter mMReflectionImporter = new MMReflectionImporter(module);
				if (UseDefault.HasValue)
				{
					mMReflectionImporter.UseDefault = UseDefault.Value;
				}
				return mMReflectionImporter;
			}
		}

		private enum GenericImportKind
		{
			Open,
			Definition
		}

		public static readonly IReflectionImporterProvider Provider = new _Provider();

		public static readonly IReflectionImporterProvider ProviderNoDefault = new _Provider
		{
			UseDefault = false
		};

		private readonly ModuleDefinition Module;

		private readonly DefaultReflectionImporter Default;

		private readonly Dictionary<Assembly, AssemblyNameReference> CachedAsms = new Dictionary<Assembly, AssemblyNameReference>();

		private readonly Dictionary<Module, TypeReference> CachedModuleTypes = new Dictionary<Module, TypeReference>();

		private readonly Dictionary<Type, TypeReference> CachedTypes = new Dictionary<Type, TypeReference>();

		private readonly Dictionary<FieldInfo, FieldReference> CachedFields = new Dictionary<FieldInfo, FieldReference>();

		private readonly Dictionary<MethodBase, MethodReference> CachedMethods = new Dictionary<MethodBase, MethodReference>();

		public bool UseDefault;

		private readonly Dictionary<Type, TypeReference> ElementTypes;

		public MMReflectionImporter(ModuleDefinition module)
		{
			Module = module;
			Default = new DefaultReflectionImporter(module);
			ElementTypes = new Dictionary<Type, TypeReference>
			{
				{
					typeof(void),
					module.TypeSystem.Void
				},
				{
					typeof(bool),
					module.TypeSystem.Boolean
				},
				{
					typeof(char),
					module.TypeSystem.Char
				},
				{
					typeof(sbyte),
					module.TypeSystem.SByte
				},
				{
					typeof(byte),
					module.TypeSystem.Byte
				},
				{
					typeof(short),
					module.TypeSystem.Int16
				},
				{
					typeof(ushort),
					module.TypeSystem.UInt16
				},
				{
					typeof(int),
					module.TypeSystem.Int32
				},
				{
					typeof(uint),
					module.TypeSystem.UInt32
				},
				{
					typeof(long),
					module.TypeSystem.Int64
				},
				{
					typeof(ulong),
					module.TypeSystem.UInt64
				},
				{
					typeof(float),
					module.TypeSystem.Single
				},
				{
					typeof(double),
					module.TypeSystem.Double
				},
				{
					typeof(string),
					module.TypeSystem.String
				},
				{
					typeof(TypedReference),
					module.TypeSystem.TypedReference
				},
				{
					typeof(IntPtr),
					module.TypeSystem.IntPtr
				},
				{
					typeof(UIntPtr),
					module.TypeSystem.UIntPtr
				},
				{
					typeof(object),
					module.TypeSystem.Object
				}
			};
		}

		private bool TryGetCachedType(Type type, out TypeReference typeRef, GenericImportKind importKind)
		{
			if (importKind == GenericImportKind.Definition)
			{
				typeRef = null;
				return false;
			}
			return CachedTypes.TryGetValue(type, out typeRef);
		}

		private TypeReference SetCachedType(Type type, TypeReference typeRef, GenericImportKind importKind)
		{
			if (importKind == GenericImportKind.Definition)
			{
				return typeRef;
			}
			return CachedTypes[type] = typeRef;
		}

		[Obsolete("Please use the Assembly overload instead.")]
		public AssemblyNameReference ImportReference(AssemblyName asm)
		{
			return Default.ImportReference(asm);
		}

		public AssemblyNameReference ImportReference(Assembly asm)
		{
			if (CachedAsms.TryGetValue(asm, out var value))
			{
				return value;
			}
			value = Default.ImportReference(asm.GetName());
			value.ApplyRuntimeHash(asm);
			return CachedAsms[asm] = value;
		}

		public TypeReference ImportModuleType(Module module, IGenericParameterProvider context)
		{
			if (CachedModuleTypes.TryGetValue(module, out var value))
			{
				return value;
			}
			return CachedModuleTypes[module] = new TypeReference(string.Empty, "<Module>", Module, ImportReference(module.Assembly));
		}

		public TypeReference ImportReference(Type type, IGenericParameterProvider context)
		{
			return _ImportReference(type, context, (context == null) ? GenericImportKind.Definition : GenericImportKind.Open);
		}

		private bool _IsGenericInstance(Type type, GenericImportKind importKind)
		{
			if (!type.IsGenericType || type.IsGenericTypeDefinition)
			{
				if (type.IsGenericType && type.IsGenericTypeDefinition)
				{
					return importKind == GenericImportKind.Open;
				}
				return false;
			}
			return true;
		}

		private GenericInstanceType _ImportGenericInstance(Type type, IGenericParameterProvider context, TypeReference typeRef)
		{
			GenericInstanceType genericInstanceType = new GenericInstanceType(typeRef);
			Type[] genericArguments = type.GetGenericArguments();
			foreach (Type type2 in genericArguments)
			{
				genericInstanceType.GenericArguments.Add(_ImportReference(type2, context));
			}
			return genericInstanceType;
		}

		private TypeReference _ImportReference(Type type, IGenericParameterProvider context, GenericImportKind importKind = GenericImportKind.Open)
		{
			if (TryGetCachedType(type, out var typeRef, importKind))
			{
				if (!_IsGenericInstance(type, importKind))
				{
					return typeRef;
				}
				return _ImportGenericInstance(type, context, typeRef);
			}
			if (UseDefault)
			{
				return SetCachedType(type, Default.ImportReference(type, context), importKind);
			}
			if (type.HasElementType)
			{
				if (type.IsByRef)
				{
					return SetCachedType(type, new ByReferenceType(_ImportReference(type.GetElementType(), context)), importKind);
				}
				if (type.IsPointer)
				{
					return SetCachedType(type, new PointerType(_ImportReference(type.GetElementType(), context)), importKind);
				}
				if (type.IsArray)
				{
					ArrayType arrayType = new ArrayType(_ImportReference(type.GetElementType(), context), type.GetArrayRank());
					if (type != type.GetElementType().MakeArrayType())
					{
						for (int i = 0; i < arrayType.Rank; i++)
						{
							arrayType.Dimensions[i] = new ArrayDimension(0, null);
						}
					}
					return CachedTypes[type] = arrayType;
				}
			}
			if (_IsGenericInstance(type, importKind))
			{
				return _ImportGenericInstance(type, context, _ImportReference(type.GetGenericTypeDefinition(), context, GenericImportKind.Definition));
			}
			if (type.IsGenericParameter)
			{
				return SetCachedType(type, ImportGenericParameter(type, context), importKind);
			}
			if (ElementTypes.TryGetValue(type, out typeRef))
			{
				return SetCachedType(type, typeRef, importKind);
			}
			typeRef = new TypeReference(string.Empty, type.Name, Module, ImportReference(type.Assembly), type.IsValueType);
			if (type.IsNested)
			{
				typeRef.DeclaringType = _ImportReference(type.DeclaringType, context, importKind);
			}
			else if (type.Namespace != null)
			{
				typeRef.Namespace = type.Namespace;
			}
			if (type.IsGenericType)
			{
				Type[] genericArguments = type.GetGenericArguments();
				foreach (Type type2 in genericArguments)
				{
					typeRef.GenericParameters.Add(new GenericParameter(type2.Name, typeRef));
				}
			}
			return SetCachedType(type, typeRef, importKind);
		}

		private static TypeReference ImportGenericParameter(Type type, IGenericParameterProvider context)
		{
			MethodReference methodReference = context as MethodReference;
			if (methodReference != null)
			{
				if (type.DeclaringMethod != null)
				{
					return methodReference.GenericParameters[type.GenericParameterPosition];
				}
				context = methodReference.DeclaringType;
			}
			Type declaringType = type.DeclaringType;
			if (declaringType == null)
			{
				throw new InvalidOperationException();
			}
			TypeReference typeReference = context as TypeReference;
			if (typeReference != null)
			{
				while (typeReference != null)
				{
					TypeReference elementType = typeReference.GetElementType();
					if (elementType.Is(declaringType))
					{
						return elementType.GenericParameters[type.GenericParameterPosition];
					}
					if (typeReference.Is(declaringType))
					{
						return typeReference.GenericParameters[type.GenericParameterPosition];
					}
					typeReference = typeReference.DeclaringType;
				}
			}
			throw new NotSupportedException();
		}

		public FieldReference ImportReference(FieldInfo field, IGenericParameterProvider context)
		{
			if (CachedFields.TryGetValue(field, out var value))
			{
				return value;
			}
			if (UseDefault)
			{
				return CachedFields[field] = Default.ImportReference(field, context);
			}
			Type declaringType = field.DeclaringType;
			TypeReference typeReference = ((declaringType != null) ? ImportReference(declaringType, context) : ImportModuleType(field.Module, context));
			FieldInfo key = field;
			if (declaringType != null && declaringType.IsGenericType)
			{
				field = field.Module.ResolveField(field.MetadataToken);
			}
			return CachedFields[key] = new FieldReference(field.Name, _ImportReference(field.FieldType, typeReference), typeReference);
		}

		public MethodReference ImportReference(MethodBase method, IGenericParameterProvider context)
		{
			return _ImportReference(method, context, (context == null) ? GenericImportKind.Definition : GenericImportKind.Open);
		}

		private MethodReference _ImportReference(MethodBase method, IGenericParameterProvider context, GenericImportKind importKind)
		{
			if (CachedMethods.TryGetValue(method, out var value) && importKind == GenericImportKind.Open)
			{
				return value;
			}
			MethodInfo methodInfo = method as MethodInfo;
			if ((object)methodInfo != null && methodInfo.IsDynamicMethod())
			{
				return new DynamicMethodReference(Module, methodInfo);
			}
			if (UseDefault)
			{
				return CachedMethods[method] = Default.ImportReference(method, context);
			}
			if ((method.IsGenericMethod && !method.IsGenericMethodDefinition) || (method.IsGenericMethod && method.IsGenericMethodDefinition && importKind == GenericImportKind.Open))
			{
				GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(_ImportReference((method as MethodInfo).GetGenericMethodDefinition(), context, GenericImportKind.Definition));
				Type[] genericArguments = method.GetGenericArguments();
				foreach (Type type in genericArguments)
				{
					genericInstanceMethod.GenericArguments.Add(_ImportReference(type, context));
				}
				return CachedMethods[method] = genericInstanceMethod;
			}
			Type declaringType = method.DeclaringType;
			value = new MethodReference(method.Name, _ImportReference(typeof(void), context), (declaringType != null) ? _ImportReference(declaringType, context, GenericImportKind.Definition) : ImportModuleType(method.Module, context));
			value.HasThis = (method.CallingConvention & CallingConventions.HasThis) != 0;
			value.ExplicitThis = (method.CallingConvention & CallingConventions.ExplicitThis) != 0;
			if ((method.CallingConvention & CallingConventions.VarArgs) != 0)
			{
				value.CallingConvention = MethodCallingConvention.VarArg;
			}
			MethodBase key = method;
			if (declaringType != null && declaringType.IsGenericType)
			{
				method = method.Module.ResolveMethod(method.MetadataToken);
			}
			if (method.IsGenericMethodDefinition)
			{
				Type[] genericArguments = method.GetGenericArguments();
				foreach (Type type2 in genericArguments)
				{
					value.GenericParameters.Add(new GenericParameter(type2.Name, value));
				}
			}
			value.ReturnType = _ImportReference((method as MethodInfo)?.ReturnType ?? typeof(void), value);
			ParameterInfo[] parameters = method.GetParameters();
			foreach (ParameterInfo parameterInfo in parameters)
			{
				value.Parameters.Add(new ParameterDefinition(parameterInfo.Name, (Mono.Cecil.ParameterAttributes)parameterInfo.Attributes, _ImportReference(parameterInfo.ParameterType, value)));
			}
			return CachedMethods[key] = value;
		}
	}
}

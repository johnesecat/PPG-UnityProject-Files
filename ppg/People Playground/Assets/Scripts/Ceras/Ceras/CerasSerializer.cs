using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ceras.Exceptions;
using Ceras.Formatters;
using Ceras.Helpers;
using Ceras.Resolvers;

namespace Ceras
{
	public class CerasSerializer : ICerasAdvanced
	{
		internal static readonly Type _rtTypeType;

		internal static readonly Type _rtFieldType;

		internal static readonly Type _rtPropType;

		internal static readonly Type _rtCtorType;

		internal static readonly Type _rtMethodType;

		private static readonly HashSet<Type> _formatterConstructedTypes;

		internal static HashSet<Assembly> _frameworkAssemblies;

		internal readonly SerializerConfig Config;

		private Type[] _knownTypes;

		private readonly IFormatterResolver _dynamicResolver;

		private readonly FormatterResolverCallback[] _userResolvers;

		internal readonly ITypeBinder TypeBinder;

		private readonly List<IFormatterResolver> _resolvers = new List<IFormatterResolver>();

		private readonly TypeDictionary<TypeMetaData> _metaData = new TypeDictionary<TypeMetaData>();

		private readonly TypeDictionary<TypeMetaData> _staticMetaData = new TypeDictionary<TypeMetaData>();

		private readonly FactoryPool<InstanceData> _instanceDataPool;

		internal readonly Action<object> DiscardObjectMethod;

		private readonly Stack<InstanceData> _recursionStack = new Stack<InstanceData>();

		internal InstanceData InstanceData;

		private int _recursionDepth;

		private RecursionMode _mode;

		public ICerasAdvanced Advanced => this;

		public ProtocolChecksum ProtocolChecksum { get; } = new ProtocolChecksum();


		public static void AddFormatterConstructedType(Type type)
		{
			lock (_formatterConstructedTypes)
			{
				_formatterConstructedTypes.Add(type);
			}
		}

		internal static bool IsFormatterConstructed(Type type)
		{
			if (type.IsAbstract)
			{
				return true;
			}
			lock (_formatterConstructedTypes)
			{
				return _formatterConstructedTypes.Contains(type);
			}
		}

		internal static bool IsPrimitiveType(Type type)
		{
			if (type.IsPrimitive)
			{
				return true;
			}
			if (type.IsArray)
			{
				return IsPrimitiveType(type.GetElementType());
			}
			if (type == typeof(string))
			{
				return true;
			}
			if (type == typeof(Type))
			{
				return true;
			}
			if (type == _rtTypeType || type == _rtFieldType || type == _rtPropType || type == _rtCtorType || type == _rtMethodType)
			{
				return true;
			}
			return false;
		}

		static CerasSerializer()
		{
			_formatterConstructedTypes = new HashSet<Type>();
			_frameworkAssemblies = new HashSet<Assembly>
			{
				typeof(object).Assembly,
				typeof(Uri).Assembly,
				typeof(Enumerable).Assembly
			};
			Type typeFromHandle = typeof(Type);
			_rtTypeType = typeFromHandle.GetType();
			_formatterConstructedTypes.Add(typeFromHandle);
			_formatterConstructedTypes.Add(_rtTypeType);
			_formatterConstructedTypes.Add(typeof(FieldInfo));
			_rtFieldType = typeof(MemberHelper).GetField("_field", BindingFlags.Static | BindingFlags.NonPublic).GetType();
			_formatterConstructedTypes.Add(_rtFieldType);
			_formatterConstructedTypes.Add(typeof(PropertyInfo));
			_rtPropType = typeof(MemberHelper).GetProperty("_prop", BindingFlags.Static | BindingFlags.NonPublic).GetType();
			_formatterConstructedTypes.Add(_rtPropType);
			_formatterConstructedTypes.Add(typeof(ConstructorInfo));
			_rtCtorType = typeof(CerasSerializer).GetConstructor(BindingFlags.Static | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]).GetType();
			_formatterConstructedTypes.Add(_rtCtorType);
			_formatterConstructedTypes.Add(typeof(MethodInfo));
			_rtMethodType = typeof(MemberHelper).GetMethod("_method", BindingFlags.Static | BindingFlags.NonPublic).GetType();
			_formatterConstructedTypes.Add(_rtMethodType);
			_formatterConstructedTypes.Add(typeof(string));
		}

		public static void ClearGenericCaches()
		{
			ObjectCache.RefProxyPoolRegister.TrimAll();
		}

		public SerializerConfig GetConfig()
		{
			return Config;
		}

		public CerasSerializer(SerializerConfig config = null)
		{
			Config = config ?? new SerializerConfig();
			if (Config.ExternalObjectResolver == null)
			{
				Config.ExternalObjectResolver = new ErrorResolver();
			}
			if (Config.Advanced.UseReinterpretFormatter && Config.VersionTolerance.Mode != 0)
			{
				throw new NotSupportedException("You can not use 'UseReinterpretFormatter' together with version tolerance. Either disable version tolerance, or use the old formatter for blittable types by setting 'Config.Advanced.UseReinterpretFormatter' to false.");
			}
			if (Config.Advanced.AotMode != 0 && Config.VersionTolerance.Mode != 0)
			{
				throw new NotSupportedException("You can not use 'AotMode.Enabled' and version tolerance at the same time for now. If you would like this feature implemented, please open an issue on GitHub explaining your use-case, or join the Discord server.");
			}
			if (Config.VersionTolerance.Mode == VersionToleranceMode.Extended)
			{
				throw new NotSupportedException("Extended VersionTolerance mode has not yet been implemented. Please read the documentation by hovering over 'VersionToleranceMode.Extended' or pressing F12 with the cursor on it.");
			}
			TypeBinder = Config.Advanced.TypeBinder;
			DiscardObjectMethod = Config.Advanced.DiscardObjectMethod;
			_userResolvers = Config.OnResolveFormatter.ToArray();
			_resolvers.Add(new PrimitiveResolver(this));
			_resolvers.Add(new ReinterpretFormatterResolver(this));
			_resolvers.Add(new StandardFormatterResolver(this));
			_resolvers.Add(new CollectionFormatterResolver(this));
			IFormatter formatter = ((Config.Advanced.SizeLimits.MaxStringLength >= uint.MaxValue) ? ((IFormatter<string>)new StringFormatter()) : ((IFormatter<string>)new MaxSizeStringFormatter(Config.Advanced.SizeLimits.MaxStringLength)));
			InjectDependencies(formatter);
			SetFormatters(typeof(string), formatter, formatter);
			TypeFormatter typeFormatter = new TypeFormatter(this);
			Type type = GetType().GetType();
			SetFormatters(typeof(Type), typeFormatter, typeFormatter);
			SetFormatters(type, typeFormatter, typeFormatter);
			_resolvers.Add(new ReflectionFormatterResolver(this));
			_dynamicResolver = new DynamicObjectFormatterResolver(this);
			_resolvers.Add(new ExpressionFormatterResolver());
			_knownTypes = Config.KnownTypes.ToArray();
			if (Config.KnownTypes.Distinct().Count() != _knownTypes.Length)
			{
				HashSet<Type> hashSet = new HashSet<Type>();
				List<Type> list = new List<Type>();
				for (int i = 0; i < _knownTypes.Length; i++)
				{
					Type item = _knownTypes[i];
					if (!hashSet.Add(item) && !list.Contains(item))
					{
						list.Add(item);
					}
				}
				string text = string.Join(", ", list.Select((Type t) => t.FriendlyName()));
				throw new Exception("KnownTypes can not contain any type multiple times! Your KnownTypes collection contains the following types more than once: " + text);
			}
			Type[] knownTypes = _knownTypes;
			foreach (Type type2 in knownTypes)
			{
				ProtocolChecksum.Add(type2.FullName);
				if (type2.IsEnum)
				{
					ProtocolChecksum.Add(type2.GetEnumUnderlyingType().FullName);
				}
				else
				{
					if (type2.ContainsGenericParameters)
					{
						continue;
					}
					TypeMetaData typeMetaData = GetTypeMetaData(type2);
					if (typeMetaData.PrimarySchema == null)
					{
						continue;
					}
					foreach (SchemaMember member in typeMetaData.PrimarySchema.Members)
					{
						ProtocolChecksum.Add(member.MemberType.FullName);
						ProtocolChecksum.Add(member.MemberName);
						object[] customAttributes = member.MemberInfo.GetCustomAttributes(inherit: true);
						foreach (object obj in customAttributes)
						{
							ProtocolChecksum.Add(obj.ToString());
						}
					}
				}
			}
			ProtocolChecksum.Finish();
			knownTypes = _knownTypes;
			foreach (Type type3 in knownTypes)
			{
				if (!type3.ContainsGenericParameters)
				{
					GetReferenceFormatter(type3);
				}
			}
			_instanceDataPool = new FactoryPool<InstanceData>(delegate
			{
				InstanceData result = default(InstanceData);
				result.CurrentRoot = null;
				result.ObjectCache = new ObjectCache();
				result.TypeCache = new TypeCache(_knownTypes);
				result.EncounteredSchemaTypes = new HashSet<Type>();
				return result;
			});
			InstanceData = _instanceDataPool.RentObject();
			if (Config.Advanced.SealTypesWhenUsingKnownTypes && _knownTypes.Length != 0)
			{
				typeFormatter.Seal();
			}
		}

		public byte[] Serialize<T>(T obj)
		{
			ICerasBufferPool obj2 = CerasBufferPool.Pool ?? NullPool.Instance;
			byte[] buffer = obj2.RentBuffer(4096);
			int num = Serialize(obj, ref buffer);
			byte[] array = new byte[num];
			if (num > 0)
			{
				SerializerBinary.FastCopy(buffer, 0, array, 0, num);
			}
			obj2.Return(buffer);
			return array;
		}

		public int Serialize<T>(T obj, ref byte[] buffer, int offset = 0)
		{
			EnterRecursive(RecursionMode.Serialization);
			if (buffer == null)
			{
				ICerasBufferPool cerasBufferPool = CerasBufferPool.Pool ?? NullPool.Instance;
				buffer = cerasBufferPool.RentBuffer(16384);
			}
			try
			{
				InstanceData.CurrentRoot = obj as IExternalRootObject;
				int num = offset;
				if (Config.Advanced.EmbedChecksum)
				{
					SerializerBinary.WriteInt32Fixed(ref buffer, ref offset, ProtocolChecksum.Checksum);
				}
				((IFormatter<T>)GetReferenceFormatter(typeof(T))).Serialize(ref buffer, ref offset, obj);
				int num2 = offset;
				if (!Config.Advanced.PersistTypeCache)
				{
					InstanceData.TypeCache.ResetSerializationCache();
				}
				InstanceData.ObjectCache.ClearSerializationCache();
				return num2 - num;
			}
			finally
			{
				InstanceData.EncounteredSchemaTypes.Clear();
				InstanceData.CurrentRoot = null;
				LeaveRecursive(RecursionMode.Serialization);
			}
		}

		byte[] ICerasAdvanced.SerializeStatic(Type type)
		{
			if (type.ContainsGenericParameters)
			{
				throw new InvalidOperationException();
			}
			TypeMetaData staticTypeMetaData = GetStaticTypeMetaData(type);
			IFormatter formatter = staticTypeMetaData.SpecificFormatter;
			if (formatter == null)
			{
				Type type2 = typeof(DynamicFormatter<>).MakeGenericType(type);
				formatter = (staticTypeMetaData.SpecificFormatter = (IFormatter)Activator.CreateInstance(type2, this, true));
			}
			MethodInfo method = formatter.GetType().GetMethod("Serialize");
			byte[] array = new byte[4096];
			object[] array2 = new object[3] { array, 0, null };
			method.Invoke(formatter, array2);
			array = (byte[])array2[0];
			int newSize = (int)array2[1];
			Array.Resize(ref array, newSize);
			return array;
		}

		void ICerasAdvanced.DeserializeStatic(Type type, byte[] buffer)
		{
			if (type.ContainsGenericParameters)
			{
				throw new InvalidOperationException();
			}
			TypeMetaData staticTypeMetaData = GetStaticTypeMetaData(type);
			IFormatter formatter = staticTypeMetaData.SpecificFormatter;
			if (formatter == null)
			{
				Type type2 = typeof(DynamicFormatter<>).MakeGenericType(type);
				formatter = (staticTypeMetaData.SpecificFormatter = (IFormatter)Activator.CreateInstance(type2, this, true));
			}
			MethodInfo method = formatter.GetType().GetMethod("Deserialize");
			object[] parameters = new object[3] { buffer, 0, null };
			method.Invoke(formatter, parameters);
		}

		public T Deserialize<T>(byte[] buffer)
		{
			T value = default(T);
			int offset = 0;
			Deserialize(ref value, buffer, ref offset);
			return value;
		}

		public void Deserialize<T>(ref T value, byte[] buffer)
		{
			int offset = 0;
			Deserialize(ref value, buffer, ref offset);
		}

		public void Deserialize<T>(ref T value, byte[] buffer, ref int offset, int expectedReadLength = -1)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("Must provide a buffer to deserialize from!");
			}
			EnterRecursive(RecursionMode.Deserialization);
			try
			{
				int num = offset;
				if (Config.Advanced.EmbedChecksum)
				{
					int num2 = SerializerBinary.ReadInt32Fixed(buffer, ref offset);
					if (num2 != ProtocolChecksum.Checksum)
					{
						throw new InvalidOperationException($"Checksum does not match embedded checksum (Serializer={ProtocolChecksum.Checksum}, Data={num2})");
					}
				}
				((IFormatter<T>)GetReferenceFormatter(typeof(T))).Deserialize(buffer, ref offset, ref value);
				if (expectedReadLength != -1)
				{
					int num3 = offset - num;
					if (num3 != expectedReadLength)
					{
						throw new UnexpectedBytesConsumedException("The deserialization has completed, but not all of the given bytes were consumed.  Maybe you tried to deserialize something directly from a larger byte-array?", expectedReadLength, num3, num, offset);
					}
				}
				if (!Config.Advanced.PersistTypeCache)
				{
					InstanceData.TypeCache.ResetDeserializationCache();
				}
				InstanceData.ObjectCache.ClearDeserializationCache();
				InstanceData.EncounteredSchemaTypes.Clear();
				if (offset > buffer.Length)
				{
					throw new IndexOutOfRangeException("The read cursor ended up beyond the array bounds, meaning that the given data is corrupted. Was the given data serialized using different settings? Did any of the classes/structs change?");
				}
			}
			finally
			{
				LeaveRecursive(RecursionMode.Deserialization);
			}
		}

		Type ICerasAdvanced.PeekType(byte[] buffer)
		{
			Type value = null;
			int offset = 0;
			GetFormatter<Type>().Deserialize(buffer, ref offset, ref value);
			return value;
		}

		IEnumerable<IFormatterResolver> ICerasAdvanced.GetFormatterResolvers()
		{
			foreach (IFormatterResolver resolver in _resolvers)
			{
				yield return resolver;
			}
			yield return _dynamicResolver;
		}

		IFormatterResolver ICerasAdvanced.GetFormatterResolver<TResolver>()
		{
			return Advanced.GetFormatterResolvers().OfType<TResolver>().FirstOrDefault();
		}

		public IFormatter<T> GetFormatter<T>()
		{
			return (IFormatter<T>)GetReferenceFormatter(typeof(T));
		}

		public IFormatter GetReferenceFormatter(Type type)
		{
			TypeMetaData typeMetaData = GetTypeMetaData(type);
			if (typeMetaData.IsValueType)
			{
				return GetSpecificFormatter(type, typeMetaData);
			}
			if (typeMetaData.ReferenceFormatter != null)
			{
				return typeMetaData.ReferenceFormatter;
			}
			return typeMetaData.ReferenceFormatter = (IFormatter)Activator.CreateInstance(typeof(ReferenceFormatter<>).MakeGenericType(type), this);
		}

		public IFormatter GetSpecificFormatter(Type type)
		{
			TypeMetaData typeMetaData = GetTypeMetaData(type);
			return GetSpecificFormatter(type, typeMetaData);
		}

		private IFormatter GetSpecificFormatter(Type type, TypeMetaData meta)
		{
			if (meta.SpecificFormatter != null)
			{
				return meta.SpecificFormatter;
			}
			if (type.IsAbstract() || type.IsInterface || type.ContainsGenericParameters)
			{
				throw new InvalidOperationException("You cannot get a formatter for abstract, static, open generic, or interface types.");
			}
			if (!meta.IsPrimitive && meta.TypeConfig.CustomFormatter != null)
			{
				meta.SpecificFormatter = meta.TypeConfig.CustomFormatter;
				FormatterHelper.ThrowOnMismatch(meta.SpecificFormatter, type);
				InjectDependencies(meta.SpecificFormatter);
				return meta.SpecificFormatter;
			}
			if (!meta.IsPrimitive && meta.TypeConfig.CustomResolver != null)
			{
				IFormatter formatter = meta.TypeConfig.CustomResolver(this, type);
				meta.SpecificFormatter = formatter ?? throw new InvalidOperationException("The custom formatter-resolver registered for Type '" + type.FullName + "' has returned 'null'.");
				FormatterHelper.ThrowOnMismatch(meta.SpecificFormatter, type);
				InjectDependencies(meta.SpecificFormatter);
				return meta.SpecificFormatter;
			}
			if (!meta.IsPrimitive)
			{
				for (int i = 0; i < _userResolvers.Length; i++)
				{
					IFormatter formatter2 = _userResolvers[i](this, type);
					if (formatter2 != null)
					{
						meta.SpecificFormatter = formatter2;
						FormatterHelper.ThrowOnMismatch(meta.SpecificFormatter, type);
						InjectDependencies(formatter2);
						return formatter2;
					}
				}
			}
			for (int j = 0; j < _resolvers.Count; j++)
			{
				IFormatter formatter3 = _resolvers[j].GetFormatter(type);
				if (formatter3 != null)
				{
					meta.SpecificFormatter = formatter3;
					InjectDependencies(formatter3);
					return formatter3;
				}
			}
			IFormatter formatter4 = _dynamicResolver.GetFormatter(type);
			if (formatter4 != null)
			{
				meta.SpecificFormatter = formatter4;
				InjectDependencies(formatter4);
				return formatter4;
			}
			throw new NotSupportedException("Ceras could not find any IFormatter<T> for the type '" + type.FullName + "'. Maybe exclude that field/prop from serializaion or write a custom formatter for it.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal TypeMetaData GetTypeMetaData(Type type)
		{
			ref TypeMetaData orAddValueRef = ref _metaData.GetOrAddValueRef(type);
			if (orAddValueRef != null)
			{
				return orAddValueRef;
			}
			return CreateMetaData(type, isStatic: false);
		}

		internal TypeMetaData GetStaticTypeMetaData(Type type)
		{
			ref TypeMetaData orAddValueRef = ref _staticMetaData.GetOrAddValueRef(type);
			if (orAddValueRef != null)
			{
				return orAddValueRef;
			}
			return CreateMetaData(type, isStatic: true);
		}

		private TypeMetaData CreateMetaData(Type type, bool isStatic)
		{
			ref TypeMetaData orAddValueRef = ref (isStatic ? _staticMetaData : _metaData).GetOrAddValueRef(type);
			if (orAddValueRef != null)
			{
				return orAddValueRef;
			}
			BannedTypes.ThrowIfBanned(type);
			bool flag = IsPrimitiveType(type);
			bool isFrameworkType = _frameworkAssemblies.Contains(type.Assembly);
			TypeConfig typeConfig = (flag ? null : Config.GetTypeConfig(type, isStatic));
			orAddValueRef = new TypeMetaData(type, typeConfig, isFrameworkType, flag);
			if (!flag)
			{
				orAddValueRef.CurrentSchema = (orAddValueRef.PrimarySchema = CreatePrimarySchema(type, isStatic));
			}
			return orAddValueRef;
		}

		private void SetFormatters(Type type, IFormatter specific, IFormatter reference)
		{
			TypeMetaData typeMetaData = GetTypeMetaData(type);
			typeMetaData.SpecificFormatter = specific;
			typeMetaData.ReferenceFormatter = reference;
		}

		private void InjectDependencies(IFormatter formatter)
		{
			Type type = formatter.GetType();
			CerasInjectAttribute cerasInjectAttribute = type.GetCustomAttribute<CerasInjectAttribute>() ?? CerasInjectAttribute.Default;
			if (type.GetCustomAttribute<CerasNoInjectAttribute>() != null)
			{
				return;
			}
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			if (cerasInjectAttribute.IncludePrivate)
			{
				bindingFlags |= BindingFlags.NonPublic;
			}
			FieldInfo[] fields = formatter.GetType().GetFields(bindingFlags);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.GetCustomAttribute<CerasNoInjectAttribute>() != null)
				{
					continue;
				}
				bool flag = fieldInfo.GetCustomAttribute<CerasNoReference>() != null;
				Type fieldType = fieldInfo.FieldType;
				if (fieldType == typeof(CerasSerializer))
				{
					SafeInject(formatter, fieldInfo, this);
				}
				else if (fieldType == typeof(SerializerConfig))
				{
					SafeInject(formatter, fieldInfo, Config);
				}
				else if (fieldType == typeof(IAdvancedConfigOptions))
				{
					SafeInject(formatter, fieldInfo, Config.Advanced);
				}
				else if (fieldType == typeof(ISizeLimitsConfig))
				{
					SafeInject(formatter, fieldInfo, Config.Advanced.SizeLimits);
				}
				else
				{
					if (!typeof(IFormatter).IsAssignableFrom(fieldType))
					{
						continue;
					}
					Type type2 = ReflectionHelper.FindClosedType(fieldType, typeof(IFormatter<>));
					if (type2 == null)
					{
						continue;
					}
					Type type3 = type2.GetGenericArguments()[0];
					if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(IFormatter<>))
					{
						IFormatter value2 = (flag ? GetSpecificFormatter(type3) : GetReferenceFormatter(type3));
						SafeInject(formatter, fieldInfo, value2);
					}
					else
					{
						if (!ReflectionHelper.IsAssignableToGenericType(fieldType, typeof(IFormatter<>)))
						{
							continue;
						}
						IFormatter formatter2 = (flag ? GetSpecificFormatter(type3) : GetReferenceFormatter(type3));
						if (formatter2.GetType() == fieldType)
						{
							SafeInject(formatter, fieldInfo, formatter2);
							continue;
						}
						IFormatter specificFormatter = GetSpecificFormatter(type3);
						if (!(specificFormatter.GetType() == fieldType))
						{
							IFormatter formatter3 = specificFormatter ?? formatter2;
							throw new InvalidOperationException("The formatter '" + formatter.GetType().FriendlyName(fullName: true) + "' has a dependency on '" + fieldType.GetType().FriendlyName() + "' (via the field '" + fieldInfo.Name + "') to format '" + type3.FriendlyName(fullName: true) + "', but this Ceras instance is already using '" + formatter3.GetType().FriendlyName(fullName: true) + "' to handle this type.");
						}
						SafeInject(formatter, fieldInfo, specificFormatter);
					}
				}
			}
			void SafeInject(object f, FieldInfo field, object value)
			{
				object value3 = field.GetValue(f);
				if (value3 == null)
				{
					field.SetValue(f, value);
				}
				else if (value3 != value)
				{
					throw new InvalidOperationException($"Error while injecting dependencies into Formatter '{f}'. The field '{field.Name}' already has a value ('{value3}') that is not equal to the intended value '{value}'");
				}
			}
		}

		internal void ActivateSchemaOverride(Type type, Schema schema)
		{
			TypeMetaData typeMetaData = GetTypeMetaData(type);
			if (!object.Equals(typeMetaData.CurrentSchema, schema))
			{
				typeMetaData.CurrentSchema = schema;
				GetSpecificFormatter(type, typeMetaData);
				for (int i = 0; i < typeMetaData.OnSchemaChangeTargets.Count; i++)
				{
					typeMetaData.OnSchemaChangeTargets[i].OnSchemaChanged(typeMetaData);
				}
			}
		}

		private Schema CreatePrimarySchema(Type type, bool isStatic)
		{
			if (IsPrimitiveType(type))
			{
				return null;
			}
			if (type.IsAbstract() || type.IsInterface || type.ContainsGenericParameters)
			{
				return null;
			}
			TypeConfig typeConfig = Config.GetTypeConfig(type, isStatic);
			typeConfig.Seal();
			Schema schema = new Schema(isPrimary: true, type, typeConfig, isStatic);
			foreach (MemberConfig allMember in typeConfig._allMembers)
			{
				if (allMember.ComputeFinalInclusionFast())
				{
					SchemaMember item = new SchemaMember(allMember.PersistentName, allMember.Member, allMember.WriteBackOrder);
					schema.Members.Add(item);
				}
			}
			schema.Members.Sort(SchemaMemberComparer.Instance);
			return schema;
		}

		internal Schema ReadSchema(byte[] buffer, ref int offset, Type type, bool isStatic)
		{
			TypeMetaData typeMetaData = GetTypeMetaData(type);
			if (typeMetaData.IsPrimitive)
			{
				throw new InvalidOperationException("Cannot read a Schema for a primitive type! This must be either a serious bug, or the given data has been tampered with. Please report it on GitHub!");
			}
			TypeConfig typeConfig = Config.GetTypeConfig(type, isStatic);
			Schema schema = new Schema(isPrimary: false, type, typeConfig, isStatic);
			int num = SerializerBinary.ReadInt32(buffer, ref offset);
			for (int i = 0; i < num; i++)
			{
				string text = SerializerBinary.ReadString(buffer, ref offset);
				MemberInfo memberInfo = Schema.FindMemberInType(type, text, isStatic);
				if (memberInfo == null)
				{
					schema.Members.Add(new SchemaMember(text));
					continue;
				}
				MemberConfig memberConfig = typeConfig.GetMemberConfig(memberInfo);
				schema.Members.Add(new SchemaMember(text, memberInfo, memberConfig.WriteBackOrder));
			}
			List<Schema> secondarySchemata = typeMetaData.SecondarySchemata;
			int num2 = secondarySchemata.IndexOf(schema);
			if (num2 == -1)
			{
				secondarySchemata.Add(schema);
				return schema;
			}
			return secondarySchemata[num2];
		}

		internal static void WriteSchema(ref byte[] buffer, ref int offset, Schema schema)
		{
			if (!schema.IsPrimary)
			{
				throw new InvalidOperationException("Can't write schema that doesn't match the primary. This is a bug, please report it on GitHub!");
			}
			List<SchemaMember> members = schema.Members;
			SerializerBinary.WriteInt32(ref buffer, ref offset, members.Count);
			for (int i = 0; i < members.Count; i++)
			{
				SerializerBinary.WriteString(ref buffer, ref offset, members[i].PersistentName);
			}
		}

		private static void VerifyName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new Exception("Member name can not be null/empty");
			}
			if (char.IsNumber(name[0]) || char.IsControl(name[0]))
			{
				throw new Exception("Name must start with a letter");
			}
			for (int i = 1; i < name.Length; i++)
			{
				if (!char.IsLetterOrDigit(name[i]) && !"_".Contains(name[i]))
				{
					throw new Exception($"The name '{name}' has character '{name[i]}' at index '{i}', which is not allowed. Must be a letter or digit.");
				}
			}
		}

		public string GenerateSerializationDebugReport(Type type)
		{
			if (IsPrimitiveType(type))
			{
				return type.FullName + " is a serialization primitive. It's serialization logic is hard-coded, so it has no schema.";
			}
			List<MemberConfig> allMembers = Config.GetTypeConfig(type, isStatic: false)._allMembers;
			MemberConfig[] array = allMembers.Where((MemberConfig m) => m.ComputeFinalInclusionFast()).ToArray();
			MemberConfig[] array2 = allMembers.Where((MemberConfig m) => !m.ComputeFinalInclusionFast()).ToArray();
			string text = $"Schema report for Type '{type.FriendlyName()}' ({allMembers.Count} data members):\r\n";
			Type type2 = GetSpecificFormatter(type).GetType();
			if (type2.IsGenericType && type2.GetGenericTypeDefinition() != typeof(DynamicFormatter<>))
			{
				text += "\r\n";
				text += "!! Warning: This report only makes sense for types handled by 'DynamicFormatter<>'.\r\n";
				text = text + "!! Ceras uses '" + type2.FriendlyName() + "' for this type.\r\n";
				text += "\r\n";
			}
			text += "\r\n";
			text += $"Serialized Members (Count = {array.Length}):\r\n";
			MemberConfig[] array3 = array;
			foreach (MemberConfig memberConfig in array3)
			{
				text = text + "[" + memberConfig.Member.Name + "] " + memberConfig.ComputeFinalInclusion().Reason + "\r\n";
			}
			if (array.Length == 0)
			{
				text += "(empty)\r\n";
			}
			text += "\r\n";
			text += $"Members excluded from serialization (Count = {array2.Length}):\r\n";
			array3 = array2;
			foreach (MemberConfig memberConfig2 in array3)
			{
				text = text + "[" + memberConfig2.Member.Name + "] " + memberConfig2.ComputeFinalInclusion().Reason + "\r\n";
			}
			if (array2.Length == 0)
			{
				text += "(empty)\r\n";
			}
			return text;
		}

		private void EnterRecursive(RecursionMode enteringMode)
		{
			_recursionDepth++;
			if (_recursionDepth == 1)
			{
				_mode = enteringMode;
				return;
			}
			if (_mode != enteringMode)
			{
				throw new InvalidOperationException("Cannot start a serialization call while a deserialization is still in progress (and vice versa)");
			}
			_recursionStack.Push(InstanceData);
			InstanceData = _instanceDataPool.RentObject();
		}

		private void LeaveRecursive(RecursionMode leavingMode)
		{
			_recursionDepth--;
			if (_recursionDepth == 0)
			{
				_mode = RecursionMode.Idle;
				return;
			}
			_instanceDataPool.ReturnObject(InstanceData);
			InstanceData = _recursionStack.Pop();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Helpers;
using Ceras.Resolvers;

namespace Ceras.Formatters
{
	public sealed class ReferenceFormatter<T> : IFormatter<T>, IFormatter, ISchemaTaintedFormatter where T : class
	{
		private class DispatcherEntry
		{
			public readonly Type Type;

			public Func<object> Constructor;

			public readonly bool IsType;

			public readonly bool IsExternalRootObject;

			public readonly bool IsValueType;

			public Schema CurrentSchema;

			public SerializeDelegate<T> CurrentSerializeDispatcher;

			public DeserializeDelegate<T> CurrentDeserializeDispatcher;

			public readonly Dictionary<Schema, DispatcherPair> SchemaDispatchers;

			public DispatcherEntry(Type type, bool hasSchema, Schema currentSchema)
			{
				Type = type;
				CurrentSchema = currentSchema;
				IsType = typeof(Type).IsAssignableFrom(type);
				IsExternalRootObject = typeof(IExternalRootObject).IsAssignableFrom(type);
				IsValueType = type.IsValueType;
				if (hasSchema)
				{
					SchemaDispatchers = new Dictionary<Schema, DispatcherPair>();
				}
			}
		}

		private struct DispatcherPair
		{
			public readonly SerializeDelegate<T> SerializeDispatcher;

			public readonly DeserializeDelegate<T> DeserializeDispatcher;

			public DispatcherPair(SerializeDelegate<T> serialize, DeserializeDelegate<T> deserialize)
			{
				SerializeDispatcher = serialize;
				DeserializeDispatcher = deserialize;
			}
		}

		private const int Null = -1;

		private const int NewValue = -2;

		private const int NewValueSameType = -3;

		private const int ExternalObject = -4;

		private const int InlineType = -5;

		private const int Bias = 5;

		private static readonly Func<object> _nullResultDelegate = () => null;

		private readonly CerasSerializer _ceras;

		private readonly TypeFormatter _typeFormatter;

		private readonly TypeDictionary<DispatcherEntry> _dispatchers = new TypeDictionary<DispatcherEntry>();

		private readonly bool _allowReferences;

		public ReferenceFormatter(CerasSerializer ceras)
		{
			_ceras = ceras;
			if (typeof(T).IsStatic())
			{
				throw new InvalidOperationException("static");
			}
			_typeFormatter = (TypeFormatter)ceras.GetSpecificFormatter(typeof(Type));
			_allowReferences = _ceras.Config.PreserveReferences;
		}

		public void Serialize(ref byte[] buffer, ref int offset, T value)
		{
			if (value == null)
			{
				SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -1, 5);
				return;
			}
			Type type = value.GetType();
			DispatcherEntry orCreateEntry = GetOrCreateEntry(type);
			if (orCreateEntry.IsType)
			{
				SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -5, 5);
				_typeFormatter.Serialize(ref buffer, ref offset, (Type)(object)value);
				return;
			}
			if (orCreateEntry.IsExternalRootObject)
			{
				IExternalRootObject externalRootObject = (IExternalRootObject)value;
				if (_ceras.InstanceData.CurrentRoot != value)
				{
					SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -4, 5);
					int referenceId = externalRootObject.GetReferenceId();
					SerializerBinary.WriteInt32(ref buffer, ref offset, referenceId);
					_ceras.Config.OnExternalObject?.Invoke(externalRootObject);
					return;
				}
			}
			if (_allowReferences)
			{
				if (_ceras.InstanceData.ObjectCache.TryGetExistingObjectId(value, out var id))
				{
					SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, id, 5);
					return;
				}
				_ceras.InstanceData.ObjectCache.RegisterObject(value);
				if ((object)typeof(T) == type)
				{
					SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -3, 5);
				}
				else
				{
					SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -2, 5);
					_typeFormatter.Serialize(ref buffer, ref offset, type);
				}
				orCreateEntry.CurrentSerializeDispatcher(ref buffer, ref offset, value);
			}
			else
			{
				if ((object)typeof(T) == type)
				{
					SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -3, 5);
				}
				else
				{
					SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -2, 5);
					_typeFormatter.Serialize(ref buffer, ref offset, type);
				}
				orCreateEntry.CurrentSerializeDispatcher(ref buffer, ref offset, value);
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref T value)
		{
			int num = SerializerBinary.ReadUInt32Bias(buffer, ref offset, 5);
			if (num == -1)
			{
				if (value != null)
				{
					_ceras.DiscardObjectMethod?.Invoke(value);
				}
				value = null;
				return;
			}
			if (num == -5)
			{
				Type type = null;
				_typeFormatter.Deserialize(buffer, ref offset, ref type);
				value = (T)(object)type;
				return;
			}
			if (num >= 0)
			{
				value = _ceras.InstanceData.ObjectCache.GetExistingObject<T>(num);
				return;
			}
			if (num == -4)
			{
				int id = SerializerBinary.ReadInt32(buffer, ref offset);
				_ceras.Config.ExternalObjectResolver.Resolve<T>(id, out value);
				return;
			}
			Type type2 = null;
			if (num == -2)
			{
				_typeFormatter.Deserialize(buffer, ref offset, ref type2);
			}
			else
			{
				type2 = typeof(T);
			}
			DispatcherEntry orCreateEntry = GetOrCreateEntry(type2);
			if (!orCreateEntry.IsValueType)
			{
				if (value != null)
				{
					if (value.GetType() != type2)
					{
						_ceras.DiscardObjectMethod?.Invoke(value);
						value = (T)orCreateEntry.Constructor();
					}
				}
				else
				{
					value = (T)orCreateEntry.Constructor();
				}
			}
			if (!_allowReferences)
			{
				orCreateEntry.CurrentDeserializeDispatcher(buffer, ref offset, ref value);
				return;
			}
			ObjectCache.RefProxy<T> refProxy = _ceras.InstanceData.ObjectCache.CreateDeserializationProxy<T>();
			refProxy.Value = value;
			orCreateEntry.CurrentDeserializeDispatcher(buffer, ref offset, ref refProxy.Value);
			value = refProxy.Value;
		}

		private DispatcherEntry GetOrCreateEntry(Type type)
		{
			ref DispatcherEntry orAddValueRef = ref _dispatchers.GetOrAddValueRef(type);
			if (orAddValueRef != null)
			{
				return orAddValueRef;
			}
			TypeMetaData typeMetaData = _ceras.GetTypeMetaData(type);
			orAddValueRef = new DispatcherEntry(type, typeMetaData.HasSchema, typeMetaData.CurrentSchema);
			if (orAddValueRef.IsType)
			{
				return orAddValueRef;
			}
			IFormatter specificFormatter = _ceras.GetSpecificFormatter(type);
			if (_ceras.Config.Advanced.AotMode == AotMode.None)
			{
				orAddValueRef.CurrentSerializeDispatcher = CreateSpecificSerializerDispatcher(type, specificFormatter);
				orAddValueRef.CurrentDeserializeDispatcher = CreateSpecificDeserializerDispatcher(type, specificFormatter);
			}
			else
			{
				orAddValueRef.CurrentSerializeDispatcher = CreateSpecificSerializerDispatcher_Aot(type, specificFormatter);
				orAddValueRef.CurrentDeserializeDispatcher = CreateSpecificDeserializerDispatcher_Aot(type, specificFormatter);
			}
			orAddValueRef.Constructor = CreateObjectConstructor(type);
			if (typeMetaData.HasSchema)
			{
				DispatcherPair value = new DispatcherPair(orAddValueRef.CurrentSerializeDispatcher, orAddValueRef.CurrentDeserializeDispatcher);
				orAddValueRef.SchemaDispatchers[orAddValueRef.CurrentSchema] = value;
			}
			return orAddValueRef;
		}

		private Func<object> CreateObjectConstructor(Type type)
		{
			if (type.IsArray)
			{
				return _nullResultDelegate;
			}
			if (CerasSerializer.IsFormatterConstructed(type) || type.IsValueType)
			{
				return _nullResultDelegate;
			}
			TypeConstruction typeConstruction = _ceras.Config.GetTypeConfig(type, isStatic: false).TypeConstruction;
			if (typeConstruction == null)
			{
				throw new InvalidOperationException("Ceras can not serialize/deserialize the type '" + type.FullName + "' because it has no 'default constructor'. You can either set a default setting for all types (config.DefaultTypeConstructionMode) or configure it for individual types in config.ConfigType<YourType>()... For more examples take a look at the tutorial.");
			}
			if (typeConstruction.HasDataArguments || typeConstruction is ConstructNull)
			{
				return _nullResultDelegate;
			}
			bool allowDynamicCodeGen = _ceras.Config.Advanced.AotMode == AotMode.None;
			return typeConstruction.GetRefFormatterConstructor(allowDynamicCodeGen);
		}

		private static SerializeDelegate<T> CreateSpecificSerializerDispatcher(Type type, IFormatter specificFormatter)
		{
			MethodInfo method = specificFormatter.GetType().GetMethod("Serialize", new Type[3]
			{
				typeof(byte[]).MakeByRefType(),
				typeof(int).MakeByRefType(),
				type
			});
			ParameterExpression parameterExpression = Expression.Parameter(typeof(byte[]).MakeByRefType(), "buffer");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(int).MakeByRefType(), "offset");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T), "value");
			Expression arg = ((typeof(T) == type) ? ((Expression)parameterExpression3) : ((Expression)(type.IsValueType ? Expression.Convert(parameterExpression3, type) : Expression.TypeAs(parameterExpression3, type))));
			return Expression.Lambda<SerializeDelegate<T>>(Expression.Block(Expression.Call(Expression.Constant(specificFormatter), method, parameterExpression, parameterExpression2, arg)), new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
		}

		private static DeserializeDelegate<T> CreateSpecificDeserializerDispatcher(Type type, IFormatter specificFormatter)
		{
			MethodInfo method = specificFormatter.GetType().GetMethod("Deserialize", new Type[3]
			{
				typeof(byte[]),
				typeof(int).MakeByRefType(),
				type.MakeByRefType()
			});
			ParameterExpression parameterExpression = Expression.Parameter(typeof(byte[]), "buffer");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(int).MakeByRefType(), "offset");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T).MakeByRefType(), "value");
			ParameterExpression parameterExpression4 = Expression.Variable(type, "valAsSpecific");
			Expression expression;
			Expression expression2;
			if (typeof(T) == type)
			{
				expression = Expression.Assign(parameterExpression4, parameterExpression3);
				expression2 = Expression.Assign(parameterExpression3, parameterExpression4);
			}
			else if (!typeof(T).IsValueType && type.IsValueType)
			{
				expression = Expression.IfThenElse(Expression.ReferenceEqual(parameterExpression3, Expression.Constant(null)), Expression.Default(type), Expression.Unbox(parameterExpression3, type));
				expression2 = Expression.Assign(parameterExpression3, Expression.Convert(parameterExpression4, typeof(T)));
			}
			else
			{
				expression = Expression.Assign(parameterExpression4, Expression.TypeAs(parameterExpression3, type));
				expression2 = Expression.Assign(parameterExpression3, parameterExpression4);
			}
			return Expression.Lambda<DeserializeDelegate<T>>(Expression.Block(new ParameterExpression[1] { parameterExpression4 }, expression, Expression.Call(Expression.Constant(specificFormatter), method, parameterExpression, parameterExpression2, parameterExpression4), expression2), new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
		}

		private static SerializeDelegate<T> CreateSpecificSerializerDispatcher_Aot(Type type, IFormatter specificFormatter)
		{
			MethodInfo serializeMethod = specificFormatter.GetType().GetMethod("Serialize", new Type[1] { type });
			if (type == typeof(T))
			{
				IFormatter<T> firstArgument = (IFormatter<T>)specificFormatter;
				return (SerializeDelegate<T>)Delegate.CreateDelegate(typeof(SerializeDelegate<T>), firstArgument, serializeMethod);
			}
			object[] args = new object[3];
			return delegate(ref byte[] buffer, ref int offset, T value)
			{
				args[0] = buffer;
				args[1] = offset;
				args[2] = value;
				serializeMethod.Invoke(specificFormatter, args);
				buffer = (byte[])args[0];
				offset = (int)args[1];
			};
		}

		private static DeserializeDelegate<T> CreateSpecificDeserializerDispatcher_Aot(Type type, IFormatter specificFormatter)
		{
			MethodInfo deserializeMethod = specificFormatter.GetType().GetMethod("Deserialize", new Type[1] { type });
			if (type == typeof(T))
			{
				IFormatter<T> firstArgument = (IFormatter<T>)specificFormatter;
				return (DeserializeDelegate<T>)Delegate.CreateDelegate(typeof(DeserializeDelegate<T>), firstArgument, deserializeMethod);
			}
			object[] args = new object[3];
			return delegate(byte[] buffer, ref int offset, ref T value)
			{
				args[0] = buffer;
				args[1] = offset;
				args[2] = value;
				deserializeMethod.Invoke(specificFormatter, args);
				offset = (int)args[1];
				value = (T)args[2];
			};
		}

		void ISchemaTaintedFormatter.OnSchemaChanged(TypeMetaData meta)
		{
			if (_dispatchers.TryGetValue(meta.Type, out var value))
			{
				if (value.SchemaDispatchers.TryGetValue(meta.CurrentSchema, out var value2))
				{
					value.CurrentSerializeDispatcher = value2.SerializeDispatcher;
					value.CurrentDeserializeDispatcher = value2.DeserializeDispatcher;
				}
				else
				{
					value.CurrentSerializeDispatcher = null;
					value.CurrentDeserializeDispatcher = null;
				}
			}
		}
	}
}

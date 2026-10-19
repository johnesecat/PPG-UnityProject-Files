using System;
using Ceras.Helpers;

namespace Ceras.Formatters
{
	internal sealed class TypeFormatter : IFormatter<Type>, IFormatter
	{
		private readonly CerasSerializer _ceras;

		private readonly ITypeBinder _typeBinder;

		private const int Null = -1;

		private const int NewGeneric = -2;

		private const int NewSingle = -3;

		private const int Bias = 3;

		private bool _isSealed;

		public TypeFormatter(CerasSerializer ceras)
		{
			_ceras = ceras;
			_typeBinder = ceras.TypeBinder;
		}

		public void Serialize(ref byte[] buffer, ref int offset, Type type)
		{
			if (type == null)
			{
				SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -1, 3);
				return;
			}
			TypeCache typeCache = _ceras.InstanceData.TypeCache;
			if (typeCache.TryGetExistingObjectId(type, out var id))
			{
				SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, id, 3);
				return;
			}
			if (_isSealed)
			{
				ThrowSealed(type, serializing: true);
			}
			if (!type.ContainsGenericParameters && type.IsGenericType)
			{
				SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -2, 3);
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				Serialize(ref buffer, ref offset, genericTypeDefinition);
				Type[] genericArguments = type.GetGenericArguments();
				SerializerBinary.WriteByte(ref buffer, ref offset, (byte)genericArguments.Length);
				for (int i = 0; i < genericArguments.Length; i++)
				{
					Serialize(ref buffer, ref offset, genericArguments[i]);
				}
				typeCache.RegisterObject(type);
			}
			else
			{
				SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -3, 3);
				string baseName = _typeBinder.GetBaseName(type);
				SerializerBinary.WriteString(ref buffer, ref offset, baseName);
				typeCache.RegisterObject(type);
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Type type)
		{
			int num = SerializerBinary.ReadUInt32Bias(buffer, ref offset, 3);
			if (num == -1)
			{
				type = null;
				return;
			}
			TypeCache typeCache = _ceras.InstanceData.TypeCache;
			if (num >= 0)
			{
				int id = num;
				type = typeCache.GetExistingObject(id);
			}
			else if (num == -2)
			{
				Type type2 = type;
				Deserialize(buffer, ref offset, ref type2);
				byte b = SerializerBinary.ReadByte(buffer, ref offset);
				Type[] array = new Type[b];
				for (int i = 0; i < b; i++)
				{
					Deserialize(buffer, ref offset, ref array[i]);
				}
				TypeCache.TypeRefProxy typeRefProxy = typeCache.CreateDeserializationProxy();
				type = _typeBinder.GetTypeFromBaseAndArguments(type2.FullName, array);
				typeRefProxy.Type = type;
				if (_isSealed)
				{
					ThrowSealed(type, serializing: false);
				}
			}
			else
			{
				TypeCache.TypeRefProxy typeRefProxy2 = typeCache.CreateDeserializationProxy();
				string baseTypeName = SerializerBinary.ReadString(buffer, ref offset);
				type = _typeBinder.GetTypeFromBase(baseTypeName);
				typeRefProxy2.Type = type;
				if (_isSealed)
				{
					ThrowSealed(type, serializing: false);
				}
			}
		}

		private static void ThrowSealed(Type type, bool serializing)
		{
			if (serializing)
			{
				throw new InvalidOperationException("Serialization Error: The type '" + type.FullName + "' cannot be added to the TypeCache because the cache is sealed (most likely on purpose to protect against exploits). Check your SerializerConfig (KnownTypes, SealType... ), or open a github issue if you think this is not supposed to happen with your settings.");
			}
			throw new InvalidOperationException("Deserialization Error: The data contained the type '" + type.FullName + "', but embedding of types that are not known in advance is not allowed in the current SerializerConfig (most likely on purpose to protect against exploits). Check your SerializerConfig (KnownTypes, SealType... ), or open a github issue if you think this is not supposed to happen with your settings.");
		}

		public void Seal()
		{
			_isSealed = true;
		}
	}
}

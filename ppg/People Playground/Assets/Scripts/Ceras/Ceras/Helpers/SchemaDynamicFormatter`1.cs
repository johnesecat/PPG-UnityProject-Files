using System;
using System.Collections.Generic;
using Ceras.Formatters;

namespace Ceras.Helpers
{
	internal class SchemaDynamicFormatter<T> : IFormatter<T>, IFormatter, ISchemaTaintedFormatter
	{
		private struct SerializerPair
		{
			public readonly SerializeDelegate<T> Serializer;

			public readonly DeserializeDelegate<T> Deserializer;

			public SerializerPair(SerializeDelegate<T> serializer, DeserializeDelegate<T> deserializer)
			{
				Serializer = serializer;
				Deserializer = deserializer;
			}
		}

		private readonly CerasSerializer _ceras;

		private readonly Dictionary<Schema, SerializerPair> _generatedSerializerPairs = new Dictionary<Schema, SerializerPair>();

		private readonly bool _isStatic;

		private Schema _currentSchema;

		private SerializeDelegate<T> _serializer;

		private DeserializeDelegate<T> _deserializer;

		private int _deserializationDepth;

		public SchemaDynamicFormatter(CerasSerializer ceras, Schema schema, bool isStatic)
		{
			_ceras = ceras;
			_currentSchema = schema;
			_isStatic = isStatic;
			Type typeFromHandle = typeof(T);
			BannedTypes.ThrowIfNonspecific(typeFromHandle);
			_ceras.Config.GetTypeConfig(typeFromHandle, isStatic).VerifyConstructionMethod();
			ActivateSchema(_currentSchema);
			RegisterForSchemaChanges();
		}

		public void Serialize(ref byte[] buffer, ref int offset, T value)
		{
			if (!_ceras.InstanceData.EncounteredSchemaTypes.Contains(typeof(T)))
			{
				_ceras.InstanceData.EncounteredSchemaTypes.Add(typeof(T));
				CerasSerializer.WriteSchema(ref buffer, ref offset, _currentSchema);
			}
			_serializer(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref T value)
		{
			Type typeFromHandle = typeof(T);
			if (!_ceras.InstanceData.EncounteredSchemaTypes.Contains(typeFromHandle))
			{
				_ceras.InstanceData.EncounteredSchemaTypes.Add(typeFromHandle);
				Schema schema = _ceras.ReadSchema(buffer, ref offset, typeFromHandle, _isStatic);
				_ceras.ActivateSchemaOverride(typeFromHandle, schema);
			}
			try
			{
				_deserializationDepth++;
				_deserializer(buffer, ref offset, ref value);
			}
			finally
			{
				_deserializationDepth--;
			}
		}

		void ISchemaTaintedFormatter.OnSchemaChanged(TypeMetaData meta)
		{
			ActivateSchema(meta.CurrentSchema);
		}

		private void RegisterForSchemaChanges()
		{
			_ceras.GetTypeMetaData(typeof(T)).OnSchemaChangeTargets.Add(this);
			foreach (SchemaMember member in _ceras.GetTypeMetaData(typeof(T)).PrimarySchema.Members)
			{
				if (member.MemberType.IsValueType)
				{
					_ceras.GetTypeMetaData(member.MemberType).OnSchemaChangeTargets.Add(this);
				}
			}
		}

		private void ActivateSchema(Schema schema)
		{
			if (_deserializationDepth > 0 && schema.Type.IsValueType)
			{
				throw new InvalidOperationException("Schema of a value-type has changed while an object-type is being deserialized. This is feature is WIP, check out GitHub for more information.");
			}
			if (_generatedSerializerPairs.TryGetValue(schema, out var value2))
			{
				_serializer = value2.Serializer;
				_deserializer = value2.Deserializer;
				_currentSchema = schema;
				return;
			}
			if (schema.Members.Count == 0)
			{
				_serializer = delegate
				{
				};
				_deserializer = delegate
				{
				};
				return;
			}
			bool isStatic = schema.IsStatic;
			if (schema.IsPrimary)
			{
				_serializer = DynamicFormatter<T>.GenerateSerializer(_ceras, schema, isSchemaFormatter: true, isStatic).Compile();
				_deserializer = DynamicFormatter<T>.GenerateDeserializer(_ceras, schema, isSchemaFormatter: true, isStatic).Compile();
			}
			else
			{
				_serializer = ErrorSerializer;
				_deserializer = DynamicFormatter<T>.GenerateDeserializer(_ceras, schema, isSchemaFormatter: true, isStatic).Compile();
			}
			_currentSchema = schema;
			_generatedSerializerPairs.Add(schema, new SerializerPair(_serializer, _deserializer));
		}

		private static void ErrorSerializer(ref byte[] buffer, ref int offset, T value)
		{
			throw new InvalidOperationException("Trying to write using a non-primary ObjectSchema. This should never happen and is a bug, please report it on GitHub!");
		}
	}
}

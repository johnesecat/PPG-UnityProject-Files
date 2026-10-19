using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Scripting
{
	public class LayerMaskConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			if (!(objectType == typeof(LayerMask)))
			{
				return objectType == typeof(LayerMask?);
			}
			return true;
		}

		[return: System.Diagnostics.CodeAnalysis.MaybeNull]
		public override object ReadJson(JsonReader reader, Type objectType, [System.Diagnostics.CodeAnalysis.AllowNull] object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return (objectType == typeof(LayerMask?)) ? null : new LayerMask?(default(LayerMask));
			}
			if (reader.TokenType == JsonToken.Integer)
			{
				LayerMask layerMask = default(LayerMask);
				layerMask.value = GetInt(reader.Value);
				return layerMask;
			}
			throw reader.CreateSerializationException($"Unexpected token when reading LayerMask. Expected 'null' or 'integer', got '{reader.TokenType}'.");
		}

		private static int GetInt(object value)
		{
			if (value is int)
			{
				return (int)value;
			}
			checked
			{
				if (value is uint)
				{
					uint num = (uint)value;
					return (int)num;
				}
				if (value is long)
				{
					long num2 = (long)value;
					return (int)num2;
				}
				if (value is ulong)
				{
					ulong num3 = (ulong)value;
					return (int)num3;
				}
				return 0;
			}
		}

		public override void WriteJson(JsonWriter writer, [System.Diagnostics.CodeAnalysis.AllowNull] object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
			}
			else
			{
				writer.WriteValue(((LayerMask)value).value);
			}
		}
	}
}

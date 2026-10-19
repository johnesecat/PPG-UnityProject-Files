using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Hashing
{
	public class Hash128Converter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			if (!(objectType == typeof(Hash128)))
			{
				return objectType == typeof(Hash128?);
			}
			return true;
		}

		[return: System.Diagnostics.CodeAnalysis.MaybeNull]
		public override object ReadJson(JsonReader reader, Type objectType, [System.Diagnostics.CodeAnalysis.AllowNull] object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return (objectType == typeof(Hash128?)) ? null : new Hash128?(default(Hash128));
			}
			if (reader.TokenType == JsonToken.String)
			{
				string text = reader.Value as string;
				if (text != null)
				{
					return Hash128.Parse(text);
				}
			}
			throw reader.CreateSerializationException($"Expected string when reading UnityEngine.Hash128 type, got '{reader.TokenType}' <{reader.Value}>.");
		}

		public override void WriteJson(JsonWriter writer, [System.Diagnostics.CodeAnalysis.AllowNull] object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
			}
			else
			{
				writer.WriteValue(((Hash128)value).ToString());
			}
		}
	}
}

using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.UnityConverters.Helpers;

namespace Newtonsoft.Json.UnityConverters
{
	public abstract class PartialConverter<T> : JsonConverter where T : new()
	{
		protected abstract void ReadValue(ref T value, string name, JsonReader reader, JsonSerializer serializer);

		protected abstract void WriteJsonProperties(JsonWriter writer, T value, JsonSerializer serializer);

		public override bool CanConvert(Type objectType)
		{
			if (!(objectType == typeof(T)))
			{
				if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return objectType.GenericTypeArguments[0] == typeof(T);
				}
				return false;
			}
			return true;
		}

		[return: System.Diagnostics.CodeAnalysis.MaybeNull]
		public override object ReadJson(JsonReader reader, Type objectType, [System.Diagnostics.CodeAnalysis.AllowNull] object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				if (!objectType.IsGenericType || !(objectType.GetGenericTypeDefinition() == typeof(Nullable<>)))
				{
					return default(T);
				}
				return null;
			}
			return InternalReadJson(reader, serializer, existingValue);
		}

		[return: System.Diagnostics.CodeAnalysis.MaybeNull]
		private T InternalReadJson(JsonReader reader, JsonSerializer serializer, [System.Diagnostics.CodeAnalysis.AllowNull] object existingValue)
		{
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw reader.CreateSerializationException($"Failed to read type '{typeof(T).Name}'. Expected object start, got '{reader.TokenType}' <{reader.Value}>");
			}
			reader.Read();
			T value = ((!(existingValue is T)) ? new T() : ((T)existingValue));
			string text = null;
			while (reader.TokenType == JsonToken.PropertyName)
			{
				string text2 = reader.Value as string;
				if (text2 != null)
				{
					if (text2 == text)
					{
						throw reader.CreateSerializationException("Failed to read type '" + typeof(T).Name + "'. Possible loop when reading property '" + text2 + "'");
					}
					text = text2;
					ReadValue(ref value, text2, reader, serializer);
				}
				else
				{
					reader.Skip();
				}
				reader.Read();
			}
			return value;
		}

		public override void WriteJson(JsonWriter writer, [System.Diagnostics.CodeAnalysis.AllowNull] object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			writer.WriteStartObject();
			T value2 = (T)value;
			WriteJsonProperties(writer, value2, serializer);
			writer.WriteEndObject();
		}
	}
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Newtonsoft.Json.UnityConverters.Helpers
{
	internal static class JsonHelperExtensions
	{
		internal static readonly ConstructorInfo _JsonSerializationExceptionPositionalCtor = typeof(JsonSerializationException).GetConstructor(new Type[5]
		{
			typeof(string),
			typeof(string),
			typeof(int),
			typeof(int),
			typeof(Exception)
		});

		private static JsonSerializationException NewJsonSerializationException(string message, string path, int lineNumber, int linePosition, [System.Diagnostics.CodeAnalysis.AllowNull] Exception innerException)
		{
			if (_JsonSerializationExceptionPositionalCtor != null)
			{
				return (JsonSerializationException)_JsonSerializationExceptionPositionalCtor.Invoke(new object[5] { message, path, lineNumber, linePosition, innerException });
			}
			return new JsonSerializationException(message, innerException);
		}

		public static JsonSerializationException CreateSerializationException(this JsonReader reader, string message, [System.Diagnostics.CodeAnalysis.AllowNull] Exception innerException = null)
		{
			StringBuilder stringBuilder = CreateStringBuilderWithSpaceAfter(message);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Path '{0}'", reader.Path);
			IJsonLineInfo jsonLineInfo = reader as IJsonLineInfo;
			int num = 0;
			int num2 = 0;
			if (jsonLineInfo != null && jsonLineInfo.HasLineInfo())
			{
				num = jsonLineInfo.LineNumber;
				num2 = jsonLineInfo.LinePosition;
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ", line {0}, position {1}", num, num2);
			}
			stringBuilder.Append('.');
			return NewJsonSerializationException(stringBuilder.ToString(), reader.Path, num, num2, innerException);
		}

		public static JsonWriterException CreateWriterException(this JsonWriter writer, string message, [System.Diagnostics.CodeAnalysis.AllowNull] Exception innerException = null)
		{
			StringBuilder stringBuilder = CreateStringBuilderWithSpaceAfter(message);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Path '{0}'.", writer.Path);
			return new JsonWriterException(stringBuilder.ToString(), writer.Path, innerException);
		}

		private static StringBuilder CreateStringBuilderWithSpaceAfter(string message)
		{
			StringBuilder stringBuilder = new StringBuilder(message);
			if (message.EndsWith("."))
			{
				stringBuilder.Append(' ');
			}
			else if (!message.EndsWith(". "))
			{
				stringBuilder.Append(". ");
			}
			return stringBuilder;
		}

		[return: System.Diagnostics.CodeAnalysis.MaybeNull]
		public static T ReadViaSerializer<T>(this JsonReader reader, JsonSerializer serializer)
		{
			reader.Read();
			return serializer.Deserialize<T>(reader);
		}

		public static float? ReadAsFloat(this JsonReader reader)
		{
			string text = reader.ReadAsString();
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
			{
				return result;
			}
			return 0f;
		}

		public static byte? ReadAsInt8(this JsonReader reader)
		{
			return checked((byte)reader.ReadAsInt32().GetValueOrDefault());
		}
	}
}

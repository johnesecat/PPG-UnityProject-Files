using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json
{
	[Serializable]
	[NullableContext(1)]
	[Nullable(0)]
	public class JsonException : Exception
	{
		public JsonException()
		{
		}

		public JsonException(string message)
			: base(message)
		{
		}

		public JsonException(string message, [Nullable(2)] Exception innerException)
			: base(message, innerException)
		{
		}

		public JsonException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal static JsonException Create(IJsonLineInfo lineInfo, string path, string message)
		{
			message = JsonPosition.FormatMessage(lineInfo, path, message);
			return new JsonException(message);
		}
	}
}

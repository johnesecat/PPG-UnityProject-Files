using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class JsonConverter
	{
		public virtual bool CanRead => true;

		public virtual bool CanWrite => true;

		public abstract void WriteJson(JsonWriter writer, [Nullable(2)] object value, JsonSerializer serializer);

		[return: Nullable(2)]
		public abstract object ReadJson(JsonReader reader, Type objectType, [Nullable(2)] object existingValue, JsonSerializer serializer);

		public abstract bool CanConvert(Type objectType);
	}
}

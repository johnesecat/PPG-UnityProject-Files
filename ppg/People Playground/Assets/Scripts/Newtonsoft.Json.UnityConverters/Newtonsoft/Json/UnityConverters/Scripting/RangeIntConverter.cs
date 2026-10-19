using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Scripting
{
	public class RangeIntConverter : PartialConverter<RangeInt>
	{
		protected override void ReadValue(ref RangeInt value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "start":
				value.start = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "length":
				value.length = reader.ReadAsInt32().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, RangeInt value, JsonSerializer serializer)
		{
			writer.WritePropertyName("start");
			writer.WriteValue(value.start);
			writer.WritePropertyName("length");
			writer.WriteValue(value.length);
		}
	}
}

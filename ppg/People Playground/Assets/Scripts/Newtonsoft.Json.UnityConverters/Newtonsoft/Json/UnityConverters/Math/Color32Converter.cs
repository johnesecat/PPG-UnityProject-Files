using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Math
{
	public class Color32Converter : PartialConverter<Color32>
	{
		protected override void ReadValue(ref Color32 value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "r":
				value.r = reader.ReadAsInt8().GetValueOrDefault();
				break;
			case "g":
				value.g = reader.ReadAsInt8().GetValueOrDefault();
				break;
			case "b":
				value.b = reader.ReadAsInt8().GetValueOrDefault();
				break;
			case "a":
				value.a = reader.ReadAsInt8().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Color32 value, JsonSerializer serializer)
		{
			writer.WritePropertyName("r");
			writer.WriteValue(value.r);
			writer.WritePropertyName("g");
			writer.WriteValue(value.g);
			writer.WritePropertyName("b");
			writer.WriteValue(value.b);
			writer.WritePropertyName("a");
			writer.WriteValue(value.a);
		}
	}
}

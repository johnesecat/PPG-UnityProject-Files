using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Math
{
	public class ColorConverter : PartialConverter<Color>
	{
		protected override void ReadValue(ref Color value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "r":
				value.r = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "g":
				value.g = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "b":
				value.b = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "a":
				value.a = reader.ReadAsFloat().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Color value, JsonSerializer serializer)
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

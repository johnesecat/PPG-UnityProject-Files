using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Math
{
	public class Vector2Converter : PartialConverter<Vector2>
	{
		protected override void ReadValue(ref Vector2 value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "x":
				value.x = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "y":
				value.y = reader.ReadAsFloat().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Vector2 value, JsonSerializer serializer)
		{
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
		}
	}
}

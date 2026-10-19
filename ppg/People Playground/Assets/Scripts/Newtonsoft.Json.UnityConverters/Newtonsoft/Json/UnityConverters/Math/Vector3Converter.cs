using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Math
{
	public class Vector3Converter : PartialConverter<Vector3>
	{
		protected override void ReadValue(ref Vector3 value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "x":
				value.x = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "y":
				value.y = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "z":
				value.z = reader.ReadAsFloat().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Vector3 value, JsonSerializer serializer)
		{
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
			writer.WritePropertyName("z");
			writer.WriteValue(value.z);
		}
	}
}

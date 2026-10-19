using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Math
{
	public class QuaternionConverter : PartialConverter<Quaternion>
	{
		protected override void ReadValue(ref Quaternion value, string name, JsonReader reader, JsonSerializer serializer)
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
			case "w":
				value.w = reader.ReadAsFloat().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Quaternion value, JsonSerializer serializer)
		{
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
			writer.WritePropertyName("z");
			writer.WriteValue(value.z);
			writer.WritePropertyName("w");
			writer.WriteValue(value.w);
		}
	}
}

using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Math
{
	public class Vector3IntConverter : PartialConverter<Vector3Int>
	{
		protected override void ReadValue(ref Vector3Int value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "x":
				value.x = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "y":
				value.y = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "z":
				value.z = reader.ReadAsInt32().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Vector3Int value, JsonSerializer serializer)
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

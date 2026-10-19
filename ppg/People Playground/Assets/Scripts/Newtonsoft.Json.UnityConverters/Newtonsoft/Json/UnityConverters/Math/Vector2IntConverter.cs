using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Math
{
	public class Vector2IntConverter : PartialConverter<Vector2Int>
	{
		protected override void ReadValue(ref Vector2Int value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "x":
				value.x = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "y":
				value.y = reader.ReadAsInt32().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
		{
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
		}
	}
}

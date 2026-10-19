using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Geometry
{
	public class RectIntConverter : PartialConverter<RectInt>
	{
		protected override void ReadValue(ref RectInt value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "x":
				value.x = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "y":
				value.y = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "width":
				value.width = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "height":
				value.height = reader.ReadAsInt32().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, RectInt value, JsonSerializer serializer)
		{
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
			writer.WritePropertyName("width");
			writer.WriteValue(value.width);
			writer.WritePropertyName("height");
			writer.WriteValue(value.height);
		}
	}
}

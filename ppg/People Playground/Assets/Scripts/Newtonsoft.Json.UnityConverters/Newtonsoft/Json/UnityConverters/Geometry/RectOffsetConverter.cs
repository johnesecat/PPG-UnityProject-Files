using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Geometry
{
	public class RectOffsetConverter : PartialConverter<RectOffset>
	{
		protected override void ReadValue(ref RectOffset value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "left":
				value.left = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "right":
				value.right = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "top":
				value.top = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "bottom":
				value.bottom = reader.ReadAsInt32().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, RectOffset value, JsonSerializer serializer)
		{
			writer.WritePropertyName("left");
			writer.WriteValue(value.left);
			writer.WritePropertyName("right");
			writer.WriteValue(value.right);
			writer.WritePropertyName("top");
			writer.WriteValue(value.top);
			writer.WritePropertyName("bottom");
			writer.WriteValue(value.bottom);
		}
	}
}

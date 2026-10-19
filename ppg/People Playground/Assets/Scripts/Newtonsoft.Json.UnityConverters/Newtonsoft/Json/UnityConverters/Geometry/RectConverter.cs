using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Geometry
{
	public class RectConverter : PartialConverter<Rect>
	{
		protected override void ReadValue(ref Rect value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "x":
				value.x = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "y":
				value.y = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "width":
				value.width = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "height":
				value.height = reader.ReadAsFloat().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Rect value, JsonSerializer serializer)
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

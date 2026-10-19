using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Geometry
{
	public class BoundsConverter : PartialConverter<Bounds>
	{
		protected override void ReadValue(ref Bounds value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "center":
				value.center = reader.ReadViaSerializer<Vector3>(serializer);
				break;
			case "size":
				value.size = reader.ReadViaSerializer<Vector3>(serializer);
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Bounds value, JsonSerializer serializer)
		{
			writer.WritePropertyName("center");
			serializer.Serialize(writer, value.center, typeof(Vector3));
			writer.WritePropertyName("size");
			serializer.Serialize(writer, value.size, typeof(Vector3));
		}
	}
}

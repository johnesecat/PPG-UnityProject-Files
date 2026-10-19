using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Geometry
{
	public class BoundsIntConverter : PartialConverter<BoundsInt>
	{
		protected override void ReadValue(ref BoundsInt value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "position":
				value.position = reader.ReadViaSerializer<Vector3Int>(serializer);
				break;
			case "size":
				value.size = reader.ReadViaSerializer<Vector3Int>(serializer);
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, BoundsInt value, JsonSerializer serializer)
		{
			writer.WritePropertyName("position");
			serializer.Serialize(writer, value.position, typeof(Vector3Int));
			writer.WritePropertyName("size");
			serializer.Serialize(writer, value.size, typeof(Vector3Int));
		}
	}
}

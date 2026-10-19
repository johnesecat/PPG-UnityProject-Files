using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Geometry
{
	public class PlaneConverter : PartialConverter<Plane>
	{
		protected override void ReadValue(ref Plane value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "normal":
				value.normal = reader.ReadViaSerializer<Vector3>(serializer);
				break;
			case "distance":
				value.distance = reader.ReadAsFloat().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Plane value, JsonSerializer serializer)
		{
			writer.WritePropertyName("normal");
			serializer.Serialize(writer, value.normal, typeof(Vector3));
			writer.WritePropertyName("distance");
			writer.WriteValue(value.distance);
		}
	}
}

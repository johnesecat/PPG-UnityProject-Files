using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace Newtonsoft.Json.UnityConverters.AI.NavMesh
{
	public class NavMeshTriangulationConverter : PartialConverter<NavMeshTriangulation>
	{
		protected override void ReadValue(ref NavMeshTriangulation value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "vertices":
				value.vertices = reader.ReadViaSerializer<Vector3[]>(serializer);
				break;
			case "indices":
				value.indices = reader.ReadViaSerializer<int[]>(serializer);
				break;
			case "areas":
				value.areas = reader.ReadViaSerializer<int[]>(serializer);
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, NavMeshTriangulation value, JsonSerializer serializer)
		{
			writer.WritePropertyName("vertices");
			serializer.Serialize(writer, value.vertices, typeof(Vector3[]));
			writer.WritePropertyName("indices");
			serializer.Serialize(writer, value.indices, typeof(int[]));
			writer.WritePropertyName("areas");
			serializer.Serialize(writer, value.areas, typeof(int[]));
		}
	}
}

using System.Reflection;
using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Physics2D
{
	public class ColliderDistance2DConverter : PartialConverter<ColliderDistance2D>
	{
		private static readonly FieldInfo _normalField = typeof(ColliderDistance2D).GetFieldInfoOrThrow("m_Normal");

		protected override void ReadValue(ref ColliderDistance2D value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "pointA":
				value.pointA = reader.ReadViaSerializer<Vector2>(serializer);
				break;
			case "pointB":
				value.pointB = reader.ReadViaSerializer<Vector2>(serializer);
				break;
			case "normal":
				_normalField.SetValueDirectRef(ref value, reader.ReadViaSerializer<Vector2>(serializer));
				break;
			case "distance":
				value.distance = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "isValid":
				value.isValid = reader.ReadAsBoolean().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, ColliderDistance2D value, JsonSerializer serializer)
		{
			writer.WritePropertyName("pointA");
			serializer.Serialize(writer, value.pointA, typeof(Vector2));
			writer.WritePropertyName("pointB");
			serializer.Serialize(writer, value.pointB, typeof(Vector2));
			writer.WritePropertyName("normal");
			serializer.Serialize(writer, value.normal, typeof(Vector2));
			writer.WritePropertyName("distance");
			writer.WriteValue(value.distance);
			writer.WritePropertyName("isValid");
			writer.WriteValue(value.isValid);
		}
	}
}

using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Physics2D
{
	public class ContactFilter2DConverter : PartialConverter<ContactFilter2D>
	{
		protected override void ReadValue(ref ContactFilter2D value, string name, JsonReader reader, JsonSerializer serializer)
		{
			if (name != null)
			{
				switch (name)
				{
				case "useTriggers":
					value.useTriggers = reader.ReadAsBoolean().GetValueOrDefault();
					break;
				case "useLayerMask":
					value.useLayerMask = reader.ReadAsBoolean().GetValueOrDefault();
					break;
				case "useDepth":
					value.useDepth = reader.ReadAsBoolean().GetValueOrDefault();
					break;
				case "useOutsideDepth":
					value.useOutsideDepth = reader.ReadAsBoolean().GetValueOrDefault();
					break;
				case "useNormalAngle":
					value.useNormalAngle = reader.ReadAsBoolean().GetValueOrDefault();
					break;
				case "useOutsideNormalAngle":
					value.useOutsideNormalAngle = reader.ReadAsBoolean().GetValueOrDefault();
					break;
				case "layerMask":
					value.layerMask = reader.ReadViaSerializer<LayerMask>(serializer);
					break;
				case "minDepth":
					value.minDepth = reader.ReadAsFloat().GetValueOrDefault();
					break;
				case "maxDepth":
					value.maxDepth = reader.ReadAsFloat().GetValueOrDefault();
					break;
				case "minNormalAngle":
					value.minNormalAngle = reader.ReadAsFloat().GetValueOrDefault();
					break;
				case "maxNormalAngle":
					value.maxNormalAngle = reader.ReadAsFloat().GetValueOrDefault();
					break;
				}
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, ContactFilter2D value, JsonSerializer serializer)
		{
			writer.WritePropertyName("useTriggers");
			writer.WriteValue(value.useTriggers);
			writer.WritePropertyName("useLayerMask");
			writer.WriteValue(value.useLayerMask);
			writer.WritePropertyName("useDepth");
			writer.WriteValue(value.useDepth);
			writer.WritePropertyName("useOutsideDepth");
			writer.WriteValue(value.useOutsideDepth);
			writer.WritePropertyName("useNormalAngle");
			writer.WriteValue(value.useNormalAngle);
			writer.WritePropertyName("useOutsideNormalAngle");
			writer.WriteValue(value.useOutsideNormalAngle);
			writer.WritePropertyName("layerMask");
			serializer.Serialize(writer, value.layerMask, typeof(LayerMask));
			writer.WritePropertyName("minDepth");
			writer.WriteValue(value.minDepth);
			writer.WritePropertyName("maxDepth");
			writer.WriteValue(value.maxDepth);
			writer.WritePropertyName("minNormalAngle");
			writer.WriteValue(value.minNormalAngle);
			writer.WritePropertyName("maxNormalAngle");
			writer.WriteValue(value.maxNormalAngle);
		}
	}
}

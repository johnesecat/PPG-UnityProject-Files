using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Physics
{
	public class JointLimitsConverter : PartialConverter<JointLimits>
	{
		protected override void ReadValue(ref JointLimits value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "min":
				value.min = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "max":
				value.max = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "bounciness":
				value.bounciness = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "bounceMinVelocity":
				value.bounceMinVelocity = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "contactDistance":
				value.contactDistance = reader.ReadAsFloat().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, JointLimits value, JsonSerializer serializer)
		{
			writer.WritePropertyName("min");
			writer.WriteValue(value.min);
			writer.WritePropertyName("max");
			writer.WriteValue(value.max);
			writer.WritePropertyName("bounciness");
			writer.WriteValue(value.bounciness);
			writer.WritePropertyName("bounceMinVelocity");
			writer.WriteValue(value.bounceMinVelocity);
			writer.WritePropertyName("contactDistance");
			writer.WriteValue(value.contactDistance);
		}
	}
}

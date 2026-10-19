using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Physics
{
	public class JointDriveConverter : PartialConverter<JointDrive>
	{
		protected override void ReadValue(ref JointDrive value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "positionSpring":
				value.positionSpring = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "positionDamper":
				value.positionDamper = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "maximumForce":
				value.maximumForce = reader.ReadAsFloat().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, JointDrive value, JsonSerializer serializer)
		{
			writer.WritePropertyName("positionSpring");
			writer.WriteValue(value.positionSpring);
			writer.WritePropertyName("positionDamper");
			writer.WriteValue(value.positionDamper);
			writer.WritePropertyName("maximumForce");
			writer.WriteValue(value.maximumForce);
		}
	}
}

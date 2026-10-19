using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Physics
{
	public class SoftJointLimitConverter : PartialConverter<SoftJointLimit>
	{
		protected override void ReadValue(ref SoftJointLimit value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "limit":
				value.limit = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "bounciness":
				value.bounciness = reader.ReadAsFloat().GetValueOrDefault();
				break;
			case "contactDistance":
				value.contactDistance = reader.ReadAsFloat().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, SoftJointLimit value, JsonSerializer serializer)
		{
			writer.WritePropertyName("limit");
			writer.WriteValue(value.limit);
			writer.WritePropertyName("bounciness");
			writer.WriteValue(value.bounciness);
			writer.WritePropertyName("contactDistance");
			writer.WriteValue(value.contactDistance);
		}
	}
}

using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace Newtonsoft.Json.UnityConverters.AI.NavMesh
{
	public class NavMeshQueryFilterConverter : PartialConverter<NavMeshQueryFilter>
	{
		private const int AREA_COST_ELEMENT_COUNT = 32;

		protected override void ReadValue(ref NavMeshQueryFilter value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "costs":
			{
				float[] array = reader.ReadViaSerializer<float[]>(serializer);
				if (array != null)
				{
					for (int num = Mathf.Min(array.Length, 32) - 1; num >= 0; num--)
					{
						value.SetAreaCost(num, array[num]);
					}
				}
				break;
			}
			case "areaMask":
				value.areaMask = reader.ReadAsInt32().GetValueOrDefault();
				break;
			case "agentTypeId":
				value.agentTypeID = reader.ReadAsInt32().GetValueOrDefault();
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, NavMeshQueryFilter value, JsonSerializer serializer)
		{
			writer.WritePropertyName("costs");
			writer.WriteStartArray();
			for (int i = 0; i < 32; i++)
			{
				writer.WriteValue(value.GetAreaCost(i));
			}
			writer.WriteEndArray();
			writer.WritePropertyName("areaMask");
			writer.WriteValue(value.areaMask);
			writer.WritePropertyName("agentTypeId");
			writer.WriteValue(value.agentTypeID);
		}
	}
}

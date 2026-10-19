using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Camera
{
	public class CullingGroupEventConverter : PartialConverter<CullingGroupEvent>
	{
		private const byte DISTANCE_MASK = 127;

		[System.Diagnostics.CodeAnalysis.MaybeNull]
		private static readonly FieldInfo _indexField = typeof(CullingGroupEvent).GetFieldInfoOrThrow("m_Index");

		[System.Diagnostics.CodeAnalysis.MaybeNull]
		private static readonly FieldInfo _prevStateField = typeof(CullingGroupEvent).GetFieldInfoOrThrow("m_PrevState");

		[System.Diagnostics.CodeAnalysis.MaybeNull]
		private static readonly FieldInfo _thisStateField = typeof(CullingGroupEvent).GetFieldInfoOrThrow("m_ThisState");

		protected override void ReadValue(ref CullingGroupEvent value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "index":
				_indexField.SetValueDirectRef(ref value, reader.ReadAsInt32().GetValueOrDefault());
				break;
			case "isVisible":
				SetStateField(_thisStateField, ref value, value.currentDistance, reader.ReadAsBoolean().GetValueOrDefault());
				break;
			case "wasVisible":
				SetStateField(_prevStateField, ref value, value.previousDistance, reader.ReadAsBoolean().GetValueOrDefault());
				break;
			case "currentDistance":
				SetStateField(_thisStateField, ref value, reader.ReadAsInt32().GetValueOrDefault(), value.isVisible);
				break;
			case "previousDistance":
				SetStateField(_prevStateField, ref value, reader.ReadAsInt32().GetValueOrDefault(), value.wasVisible);
				break;
			}
		}

		private static void SetStateField(FieldInfo field, ref CullingGroupEvent value, int distance, bool isVisible)
		{
			byte value2 = (byte)((isVisible ? 128u : 0u) | ((uint)distance & 0x7Fu));
			field.SetValueDirectRef(ref value, value2);
		}

		protected override void WriteJsonProperties(JsonWriter writer, CullingGroupEvent value, JsonSerializer serializer)
		{
			writer.WritePropertyName("index");
			writer.WriteValue(value.index);
			writer.WritePropertyName("isVisible");
			writer.WriteValue(value.isVisible);
			writer.WritePropertyName("wasVisible");
			writer.WriteValue(value.wasVisible);
			writer.WritePropertyName("currentDistance");
			writer.WriteValue(value.currentDistance);
			writer.WritePropertyName("previousDistance");
			writer.WriteValue(value.previousDistance);
		}
	}
}

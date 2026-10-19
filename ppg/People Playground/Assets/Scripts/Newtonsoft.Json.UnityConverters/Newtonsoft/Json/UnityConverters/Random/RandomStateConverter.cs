using System.Reflection;
using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Random
{
	public class RandomStateConverter : PartialConverter<UnityEngine.Random.State>
	{
		private static readonly FieldInfo _s0Field = typeof(UnityEngine.Random.State).GetFieldInfoOrThrow("s0");

		private static readonly FieldInfo _s1Field = typeof(UnityEngine.Random.State).GetFieldInfoOrThrow("s1");

		private static readonly FieldInfo _s2Field = typeof(UnityEngine.Random.State).GetFieldInfoOrThrow("s2");

		private static readonly FieldInfo _s3Field = typeof(UnityEngine.Random.State).GetFieldInfoOrThrow("s3");

		protected override void ReadValue(ref UnityEngine.Random.State value, string name, JsonReader reader, JsonSerializer serializer)
		{
			switch (name)
			{
			case "s0":
				_s0Field.SetValueDirectRef(ref value, reader.ReadAsInt32().GetValueOrDefault());
				break;
			case "s1":
				_s1Field.SetValueDirectRef(ref value, reader.ReadAsInt32().GetValueOrDefault());
				break;
			case "s2":
				_s2Field.SetValueDirectRef(ref value, reader.ReadAsInt32().GetValueOrDefault());
				break;
			case "s3":
				_s3Field.SetValueDirectRef(ref value, reader.ReadAsInt32().GetValueOrDefault());
				break;
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, UnityEngine.Random.State value, JsonSerializer serializer)
		{
			writer.WritePropertyName("s0");
			writer.WriteValue(_s0Field.GetValue(value));
			writer.WritePropertyName("s1");
			writer.WriteValue(_s1Field.GetValue(value));
			writer.WritePropertyName("s2");
			writer.WriteValue(_s2Field.GetValue(value));
			writer.WritePropertyName("s3");
			writer.WriteValue(_s3Field.GetValue(value));
		}
	}
}

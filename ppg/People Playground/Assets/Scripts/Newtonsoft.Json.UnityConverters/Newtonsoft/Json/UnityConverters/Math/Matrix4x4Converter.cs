using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Math
{
	public class Matrix4x4Converter : PartialConverter<Matrix4x4>
	{
		private static readonly string[] _names = GetMemberNames();

		private static readonly Dictionary<string, int> _namesToIndex = GetNamesToIndex(_names);

		private static string[] GetMemberNames()
		{
			string[] indexes = new string[4] { "0", "1", "2", "3" };
			return indexes.SelectMany((string row) => indexes.Select((string column) => "m" + column + row)).ToArray();
		}

		private static Dictionary<string, int> GetNamesToIndex(string[] names)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int i = 0; i < names.Length; i++)
			{
				dictionary[names[i]] = i;
			}
			return dictionary;
		}

		protected override void ReadValue(ref Matrix4x4 value, string name, JsonReader reader, JsonSerializer serializer)
		{
			if (_namesToIndex.TryGetValue(name, out var value2))
			{
				value[value2] = reader.ReadAsFloat().GetValueOrDefault();
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, Matrix4x4 value, JsonSerializer serializer)
		{
			for (int i = 0; i < _names.Length; i++)
			{
				writer.WritePropertyName(_names[i]);
				writer.WriteValue(value[i]);
			}
		}
	}
}

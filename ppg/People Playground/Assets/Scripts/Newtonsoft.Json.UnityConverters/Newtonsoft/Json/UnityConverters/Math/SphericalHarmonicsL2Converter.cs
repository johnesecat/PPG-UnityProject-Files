using System.Collections.Generic;
using Newtonsoft.Json.UnityConverters.Helpers;
using UnityEngine.Rendering;

namespace Newtonsoft.Json.UnityConverters.Math
{
	public class SphericalHarmonicsL2Converter : PartialConverter<SphericalHarmonicsL2>
	{
		private const int COEFFICIENT_COUNT = 9;

		private const int ARRAY_SIZE = 27;

		private static readonly (string name, int rgb, int coefficient)[] _indices = GetMemberNames();

		private static readonly Dictionary<string, (int color, int coefficient)> _nameToIndex = GetNamesToIndexDictionary(_indices);

		private static (string name, int rgb, int coefficient)[] GetMemberNames()
		{
			(string, int, int)[] array = new(string, int, int)[27];
			for (int i = 0; i < 9; i++)
			{
				array[i] = ("r" + i, 0, i);
			}
			for (int j = 0; j < 9; j++)
			{
				array[9 + j] = ("g" + j, 1, j);
			}
			for (int k = 0; k < 9; k++)
			{
				array[18 + k] = ("b" + k, 2, k);
			}
			return array;
		}

		private static Dictionary<string, (int color, int coefficient)> GetNamesToIndexDictionary((string name, int rgb, int coefficient)[] indices)
		{
			Dictionary<string, (int, int)> dictionary = new Dictionary<string, (int, int)>();
			for (int i = 0; i < indices.Length; i++)
			{
				(string name, int rgb, int coefficient) tuple = indices[i];
				string item = tuple.name;
				int item2 = tuple.rgb;
				int item3 = tuple.coefficient;
				dictionary[item] = (item2, item3);
			}
			return dictionary;
		}

		protected override void ReadValue(ref SphericalHarmonicsL2 value, string name, JsonReader reader, JsonSerializer serializer)
		{
			if (_nameToIndex.TryGetValue(name, out var value2))
			{
				value[value2.Item1, value2.Item2] = reader.ReadAsFloat().GetValueOrDefault();
			}
		}

		protected override void WriteJsonProperties(JsonWriter writer, SphericalHarmonicsL2 value, JsonSerializer serializer)
		{
			(string, int, int)[] indices = _indices;
			for (int i = 0; i < indices.Length; i++)
			{
				var (name, rgb, coefficient) = indices[i];
				writer.WritePropertyName(name);
				writer.WriteValue(value[rgb, coefficient]);
			}
		}
	}
}

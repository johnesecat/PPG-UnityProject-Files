using System;
using System.Collections.Generic;
using System.IO;
using Ceras;
using UnityEngine;

internal static class NonSteamStatManager
{
	internal class StatCollection
	{
		public string Identifier;

		public Dictionary<string, float> Stats = new Dictionary<string, float>();

		public StatCollection()
		{
			Identifier = Hash128.Compute(Environment.MachineName).ToString();
		}

		public float GetStat(string key, float fallback = 0f)
		{
			if (Stats.TryGetValue(key, out var value))
			{
				return value;
			}
			Stats.Add(key, fallback);
			return fallback;
		}

		public void SetStat(string key, float value)
		{
			if (Stats.ContainsKey(key))
			{
				Stats[key] = value;
			}
			else
			{
				Stats.Add(key, value);
			}
		}

		public bool HasStat(string key)
		{
			return Stats.ContainsKey(key);
		}

		public void Increment(string key, float delta = 1f)
		{
			float stat = GetStat(key);
			SetStat(key, stat + delta);
		}
	}

	internal static StatCollection Stats = new StatCollection();

	internal const string DefaultPath = "stats";

	private static CerasSerializer serializer = new CerasSerializer();

	internal const string BodyCount = "BODY_COUNT";

	internal const string SpawnCount = "TOTAL_SPAWNED_ITEMS";

	internal const string NuclearExplosions = "NUCLEAR_EXPLOSIONS";

	internal const string EntitiesRevived = "ENTITIES_REVIVED";

	internal const string WiresLaid = "WIRES_LAID";

	internal const string ContraptionsSaved = "CONTRAPTIONS_SAVED";

	public static void SaveToFile(string path)
	{
		byte[] bytes = serializer.Serialize(Stats);
		File.WriteAllBytes(path, bytes);
	}

	public static void LoadFromFile(string path)
	{
		if (File.Exists(path))
		{
			byte[] buffer = File.ReadAllBytes(path);
			StatCollection statCollection;
			try
			{
				statCollection = serializer.Deserialize<StatCollection>(buffer);
			}
			catch (Exception)
			{
				Debug.LogWarning("Corrupt stats file. File will be deleted.");
				File.Delete(path);
				return;
			}
			if (Stats.Identifier != statCollection.Identifier)
			{
				Debug.LogWarning("Attempted to load stats file from a different machine. File will be deleted.");
				File.Delete(path);
			}
			else
			{
				Stats = statCollection;
			}
		}
	}
}

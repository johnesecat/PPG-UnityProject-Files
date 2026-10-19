using System;
using System.Collections.Generic;
using UnityEngine;

public static class MapRegistry
{
	private static readonly Dictionary<string, Map> maps = new Dictionary<string, Map>();

	public static void Register(Map map)
	{
		if (maps.ContainsKey(map.UniqueIdentity))
		{
			throw new Exception("There was already a map with this identity registered");
		}
		maps.Add(map.UniqueIdentity, map);
		Debug.LogFormat("Registered map \"{0}\" with ID {1}", map.name, map.UniqueIdentity);
	}

	public static Map GetMap(string identity)
	{
		if (maps.TryGetValue(identity, out var value))
		{
			return value;
		}
		return null;
	}

	public static bool HasMap(string identity)
	{
		return maps.ContainsKey(identity);
	}

	public static bool TryGetMap(string identity, out Map map)
	{
		return maps.TryGetValue(identity, out map);
	}

	public static void Clear()
	{
		maps.Clear();
	}

	public static IEnumerable<Map> GetAllMaps()
	{
		foreach (KeyValuePair<string, Map> map in maps)
		{
			if ((bool)map.Value)
			{
				yield return map.Value;
			}
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Ceras;
using UnityEngine;

internal static class TrustCache
{
	private const string path = "People Playground_Data/tc.bin";

	private static readonly ConcurrentDictionary<string, bool> trustByModID = new ConcurrentDictionary<string, bool>();

	private static readonly CerasSerializer serialiser = new CerasSerializer();

	internal static void Load()
	{
		AssertCaller("load the trust cache");
		try
		{
			byte[] buffer = File.ReadAllBytes("People Playground_Data/tc.bin");
			Dictionary<string, bool> dictionary = serialiser.Deserialize<Dictionary<string, bool>>(buffer);
			trustByModID.Clear();
			foreach (KeyValuePair<string, bool> item in dictionary)
			{
				trustByModID.TryAdd(item.Key, item.Value);
			}
			Debug.LogFormat("Trust cache of length {0} read", dictionary.Count);
		}
		catch (FileNotFoundException)
		{
			Debug.Log("Trust cache does not exist yet :)");
		}
		catch (Exception ex2)
		{
			Debug.LogErrorFormat("Failed to read trust cache: {0}", ex2.Message);
		}
	}

	internal static void Save()
	{
		AssertCaller("save the trust cache");
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>(trustByModID);
		byte[] bytes = serialiser.Serialize(dictionary);
		File.WriteAllBytes("People Playground_Data/tc.bin", bytes);
		Debug.LogFormat("Trust cache of length {0} written", dictionary.Count);
	}

	internal static void Trust(ModMetaData modMeta)
	{
		AssertCaller("force trust " + modMeta.GetUniqueName());
		string uniqueName = modMeta.GetUniqueName();
		if (trustByModID.ContainsKey(uniqueName))
		{
			trustByModID[uniqueName] = true;
		}
		else
		{
			trustByModID.TryAdd(uniqueName, value: true);
		}
	}

	internal static void AssertCaller(string action)
	{
		Assembly callingAssembly = Assembly.GetCallingAssembly();
		if (callingAssembly != Assembly.GetAssembly(typeof(LimbBehaviour)))
		{
			throw new Exception($"SUSPICIOUS BEHAVIOUR: \"{callingAssembly}\" attempted to {action}");
		}
	}

	internal static void Distrust(ModMetaData modMeta)
	{
		AssertCaller("force distrust " + modMeta.GetUniqueName());
		string uniqueName = modMeta.GetUniqueName();
		if (trustByModID.ContainsKey(uniqueName))
		{
			trustByModID[uniqueName] = false;
		}
		else
		{
			trustByModID.TryAdd(uniqueName, value: false);
		}
	}

	internal static bool IsTrusted(ModMetaData modMeta)
	{
		bool value;
		return trustByModID.TryGetValue(modMeta.GetUniqueName(), out value) && value;
	}
}

using System;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

internal static class StatManager
{
	public struct Stat
	{
		public readonly string APIName;

		public static readonly Stat UNIQUE_SPAWN_COUNT = new Stat("UNIQUE_SPAWN_COUNT");

		public static readonly Stat BODY_COUNT = new Stat("BODY_COUNT");

		public static readonly Stat RAGDOLL_AIR_TIME = new Stat("RAGDOLL_AIR_TIME");

		public static readonly Stat TOTAL_SPAWNED_ITEMS = new Stat("TOTAL_SPAWNED_ITEMS");

		public static readonly Stat SPAWNED_HUMANS = new Stat("SPAWNED_HUMANS");

		public static readonly Stat SGNR = new Stat("SGNRSTAT");

		public static readonly Stat WDJZM = new Stat("WDJZMSTAT");

		public static readonly Stat PACIFIST = new Stat("PACIFISTSTAT");

		public static readonly Stat KEYPAD = new Stat("KEYPADSTAT");

		public Stat(string apiName)
		{
			APIName = apiName;
		}

		public override string ToString()
		{
			return APIName;
		}

		public override bool Equals(object obj)
		{
			if (obj is Stat)
			{
				Stat stat = (Stat)obj;
				return APIName == stat.APIName;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return -1814423452 + EqualityComparer<string>.Default.GetHashCode(APIName);
		}
	}

	private static readonly Dictionary<Stat, int> integers = new Dictionary<Stat, int>();

	private static readonly Dictionary<Stat, float> floats = new Dictionary<Stat, float>();

	private static readonly HashSet<string> achievementsUnlockedThisSession = new HashSet<string>();

	private const short MaxAttempts = 64;

	private static short attempts;

	private static bool SteamManagerCheck()
	{
		if (attempts >= 64)
		{
			return false;
		}
		if (!SteamworksInitialiser.IsInitialised)
		{
			attempts++;
			Debug.LogWarning("Attempt to call SteamWorks API before initialisation");
			if (attempts >= 64)
			{
				Debug.LogError("Max stat failure count reached: stop trying :(");
			}
			return false;
		}
		return true;
	}

	public static void ClearCache()
	{
		integers.Clear();
		floats.Clear();
	}

	public static void UnlockAchievement(string name)
	{
		if (!achievementsUnlockedThisSession.Contains(name))
		{
			Debug.Log("Achievement unlocked: " + name);
			achievementsUnlockedThisSession.Add(name);
			try
			{
				new Achievement(name).Trigger();
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Failed to unlock achievement: {0}\n{1}", name, ex.Message);
			}
		}
	}

	public static void IncrementInteger(Stat stat, int value = 1)
	{
		if (SteamManagerCheck() && attempts < 64)
		{
			int @int = GetInt(stat);
			@int += value;
			SetInt(stat, @int);
			SteamUserStats.StoreStats();
			ClearCache();
		}
	}

	public static void IncrementFloat(Stat stat, float value = 1f)
	{
		if (SteamManagerCheck() && attempts < 64)
		{
			float @float = GetFloat(stat);
			@float += value;
			SetFloat(stat, @float);
			SteamUserStats.StoreStats();
			ClearCache();
		}
	}

	public static void SetFloat(Stat stat, float value)
	{
		if (SteamManagerCheck() && attempts < 64 && !float.IsNaN(value))
		{
			if (!SteamUserStats.SetStat(stat.APIName, value))
			{
				attempts++;
				Stat stat2 = stat;
				Debug.LogWarning("Stat setting failed: " + stat2.ToString());
			}
			else
			{
				SteamUserStats.StoreStats();
			}
		}
	}

	public static void SetInt(Stat stat, int value)
	{
		if (SteamManagerCheck() && attempts < 64)
		{
			if (!SteamUserStats.SetStat(stat.APIName, value))
			{
				attempts++;
				Stat stat2 = stat;
				Debug.LogWarning("Stat setting failed: " + stat2.ToString());
			}
			else
			{
				SteamUserStats.StoreStats();
			}
		}
	}

	public static float GetFloat(Stat stat)
	{
		if (!SteamManagerCheck() || attempts >= 64)
		{
			return float.NaN;
		}
		if (!floats.TryGetValue(stat, out var value))
		{
			try
			{
				value = SteamUserStats.GetStatFloat(stat.APIName);
				floats.Add(stat, value);
				return value;
			}
			catch (Exception)
			{
				attempts++;
				Stat stat2 = stat;
				Debug.LogWarning("Stat retrieval failed: " + stat2.ToString());
				return value;
			}
		}
		return value;
	}

	public static int GetInt(Stat stat)
	{
		if (!SteamManagerCheck() || attempts >= 64)
		{
			return -1;
		}
		if (!integers.TryGetValue(stat, out var value))
		{
			try
			{
				value = SteamUserStats.GetStatInt(stat.APIName);
				integers.Add(stat, value);
				return value;
			}
			catch (Exception)
			{
				attempts++;
				Stat stat2 = stat;
				Debug.LogWarning("Stat retrieval failed: " + stat2.ToString());
				return value;
			}
		}
		return value;
	}
}

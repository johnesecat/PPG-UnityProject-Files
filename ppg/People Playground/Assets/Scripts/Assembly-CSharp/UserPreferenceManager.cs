using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class UserPreferenceManager
{
	public const string PreferencePath = "config.json";

	public static Preferences Current = new Preferences();

	private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
	{
		Formatting = Formatting.Indented,
		MissingMemberHandling = MissingMemberHandling.Ignore
	};

	public static void Load()
	{
		if (File.Exists("config.json"))
		{
			try
			{
				Current = JsonConvert.DeserializeObject<Preferences>(File.ReadAllText("config.json"), jsonSettings);
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to load preferences file: " + ex.Message);
				Current = LoadFromLegacyStorage();
			}
		}
	}

	public static void Save()
	{
		if (Current == null)
		{
			Current = new Preferences();
		}
		if (Current.WindowMode == WindowMode.Windowed)
		{
			Current.Resolution = new Vector2Int(Screen.width, Screen.height);
		}
		string contents = JsonConvert.SerializeObject(Current, jsonSettings);
		try
		{
			File.WriteAllText("config.json", contents);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to save preferences file: " + ex.Message);
		}
	}

	[Obsolete]
	public static Preferences LoadFromLegacyStorage()
	{
		Preferences preferences = new Preferences();
		preferences.DropOnDeath = PlayerPrefs.GetString("dropOnDeath", "on") == "on";
		preferences.LimbCrushing = PlayerPrefs.GetString("crushing", "on") == "on";
		preferences.TracerBullets = PlayerPrefs.GetString("tracers", "off") == "on";
		preferences.FancyEffects = PlayerPrefs.GetString("extraDecals", "on") == "on";
		preferences.Decals = PlayerPrefs.GetString("decals", "on") == "on";
		preferences.ClampVolume = PlayerPrefs.GetString("clampVolume", "on") == "on";
		preferences.ShowOutlines = PlayerPrefs.GetString("showOutlines", "on") == "on";
		preferences.LogDebugMessages = PlayerPrefs.GetString("logDebug", "off") == "on";
		preferences.ShowFramerate = PlayerPrefs.GetString("showFramerate", "disabled") == "enabled";
		preferences.Lighting = PlayerPrefs.GetString("lighting", "on") == "on";
		preferences.VSync = PlayerPrefs.GetString("vsync", "off") == "on";
		preferences.ZoomSensitivity = PlayerPrefs.GetFloat("zoomSensitivity", 1f);
		preferences.MasterVolume = PlayerPrefs.GetFloat("master", 1f);
		preferences.SfxVolume = PlayerPrefs.GetFloat("sfx", 1f);
		preferences.UserInterfaceVolume = PlayerPrefs.GetFloat("userInterface", 1f);
		preferences.AmbienceVolume = PlayerPrefs.GetFloat("ambience", 1f);
		preferences.ShakeIntensity = PlayerPrefs.GetFloat("shakeIntensity", 1f);
		preferences.SlowMotionSpeed = PlayerPrefs.GetFloat("slowMoSpeed", 20f);
		preferences.CrushForceMultiplier = PlayerPrefs.GetFloat("crushForce", 1f);
		switch (PlayerPrefs.GetString("aa", "off"))
		{
		case "2x":
		case "4x":
		case "8x":
			preferences.SMAA = true;
			break;
		default:
			preferences.SMAA = false;
			break;
		}
		switch (PlayerPrefs.GetString("windowMode", "borderless"))
		{
		case "windowed":
			preferences.WindowMode = WindowMode.Windowed;
			break;
		case "fullscreen":
			preferences.WindowMode = WindowMode.Fullscreen;
			break;
		default:
			preferences.WindowMode = WindowMode.Borderless;
			break;
		}
		switch (PlayerPrefs.GetString("collision", "discrete"))
		{
		case "continuous":
			preferences.CollisionQuality = CollisionQuality.Continuous;
			break;
		default:
			preferences.CollisionQuality = CollisionQuality.Discrete;
			break;
		}
		switch (PlayerPrefs.GetString("bloom", "fancy"))
		{
		case "off":
			preferences.BloomMode = BloomMode.Off;
			break;
		case "fast":
			preferences.BloomMode = BloomMode.Fast;
			break;
		default:
			preferences.BloomMode = BloomMode.Fancy;
			break;
		}
		preferences.PhysicsIterations = Mathf.RoundToInt(PlayerPrefs.GetFloat("physicsIterations", 16f));
		string @string = PlayerPrefs.GetString("resolution", "automatic");
		if (@string == null || @string == "automatic")
		{
			preferences.Resolution = null;
		}
		else
		{
			string[] array = @string.Split('x');
			int x = int.Parse(array[0]);
			int y = int.Parse(array[1]);
			preferences.Resolution = new Vector2Int(x, y);
		}
		return preferences;
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

internal static class ModificationManager
{
	internal static Dictionary<Modification, ModMetaData> Modifications = new Dictionary<Modification, ModMetaData>();

	internal static List<Type> BackgroundScripts = new List<Type>();

	internal static List<Type> CustomTools = new List<Type>();

	internal static List<Type> CustomPowers = new List<Type>();

	internal static void InvokeOnLoad()
	{
		TimeSpan zero = TimeSpan.Zero;
		List<(ModMetaData, TimeSpan)> list = new List<(ModMetaData, TimeSpan)>();
		foreach (ModMetaData loadedMod in ModLoader.LoadedMods)
		{
			if (loadedMod.Active)
			{
				TimeSpan timeSpan = InvokeOnLoad(loadedMod);
				list.Add((loadedMod, timeSpan));
				zero += timeSpan;
			}
		}
		if (!(zero.TotalMilliseconds >= 500.0))
		{
			return;
		}
		UnityEngine.Debug.LogWarningFormat("Mods caused a {0}ms freeze on load...", zero.TotalMilliseconds);
		if (UserPreferenceManager.Current.ShowModLoadingFreeze)
		{
			IEnumerable<(ModMetaData, TimeSpan)> source = from d in list
				orderby d.Item2.Ticks descending
				where d.Item2.TotalMilliseconds > 100.0
				select d;
			string text = null;
			text = ((!source.Any()) ? $"Some mods caused a {Mathf.RoundToInt((float)zero.TotalMilliseconds)}ms freeze!\nConsider disabling some mods for faster load times." : ($"Some mods caused a {Mathf.RoundToInt((float)zero.TotalMilliseconds)}ms freeze!\nConsider disabling these mods for faster load times:\n\n" + string.Join("\n", from d in source.Take(4)
				select d.Item1.Name)));
			UISoundBehaviour.Main.Warning();
			DialogBox dialogBox = DialogBoxManager.Dialog(text, new DialogButton("Don't show this again", true, delegate
			{
				UserPreferenceManager.Current.ShowModLoadingFreeze = false;
			}), new DialogButton("OK", true));
			dialogBox.SetWidth(900f);
			dialogBox.SetHeight(470f);
		}
	}

	internal static TimeSpan InvokeOnLoad(ModMetaData metaData)
	{
		if (ModLoader.alreadyInitialisedMods.Contains(metaData))
		{
			return TimeSpan.Zero;
		}
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		if (Invoke("OnLoad", metaData))
		{
			ModLoader.alreadyInitialisedMods.Add(metaData);
			UnityEngine.Debug.Log(metaData.Name + " initialised...");
		}
		stopwatch.Stop();
		return stopwatch.Elapsed;
	}

	internal static TimeSpan InvokeOnUnload(ModMetaData metaData)
	{
		if (!ModLoader.alreadyInitialisedMods.Contains(metaData))
		{
			return TimeSpan.Zero;
		}
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		if (Invoke("OnUnload", metaData))
		{
			ModLoader.alreadyInitialisedMods.Remove(metaData);
			UnityEngine.Debug.Log(metaData.Name + " deinitialised...");
		}
		stopwatch.Stop();
		return stopwatch.Elapsed;
	}

	internal static void InvokeMain()
	{
		ModAPI.ClearEvents();
		BackgroundScripts.Clear();
		Modifications.Clear();
		foreach (ModMetaData loadedMod in ModLoader.LoadedMods)
		{
			Invoke("Main", loadedMod);
		}
	}

	internal static bool Invoke(string methodName, ModMetaData metaData)
	{
		if (!metaData.Active || metaData == null || !ModLoader.ModScripts.TryGetValue(metaData, out var value) || value == null)
		{
			return false;
		}
		try
		{
			ModAPI.Metadata = metaData;
			ModAPI.metaDataIsValid = true;
			MethodInfo method = value.LoadedAssembly.GetType(metaData.EntryPoint).GetMethod(methodName);
			if (method == null)
			{
				UnityEngine.Debug.LogWarning("No static \"" + methodName + "\" method found in mod " + metaData.Author + "." + metaData.Name);
				ModAPI.metaDataIsValid = false;
				return false;
			}
			method.Invoke(null, Array.Empty<object>());
			ModAPI.metaDataIsValid = false;
			return true;
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Error invoking $" + methodName + ": " + metaData.Name + "\n" + ex);
			ModAPI.metaDataIsValid = false;
			return false;
		}
	}
}

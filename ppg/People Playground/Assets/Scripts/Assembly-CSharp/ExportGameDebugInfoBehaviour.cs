using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

public class ExportGameDebugInfoBehaviour : MonoBehaviour
{
	public static string GetDebugInfoString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("# EXPORTED GAME DEBUG INFO\n");
		stringBuilder.AppendFormat("## People Playground [v{0}] [{1} bit]\n", "1.27.5", (IntPtr.Size == 8) ? "64" : "32");
		stringBuilder.AppendLine();
		stringBuilder.Append("## System info\n\n");
		stringBuilder.AppendFormat(" - **OS**: {0}\n\n", SystemInfo.operatingSystem);
		stringBuilder.AppendFormat(" - **CPU**: {0}\n\n", SystemInfo.processorType);
		stringBuilder.AppendFormat(" - **GPU**: {0}\n\n", SystemInfo.graphicsDeviceName);
		stringBuilder.AppendFormat(" - **Memory**: {0}\n\n", Utils.GetFormattedByteString((ulong)SystemInfo.systemMemorySize * 1000000uL));
		stringBuilder.AppendLine();
		stringBuilder.Append("## All loaded mods\n\n");
		foreach (ModMetaData loadedMod in ModLoader.LoadedMods)
		{
			stringBuilder.AppendFormat("\t{0} v{1} for game v{2}\n", loadedMod.GetUniqueName(), loadedMod.ModVersion, loadedMod.GameVersion);
			stringBuilder.AppendFormat("\t\tCurrently {0}\n", loadedMod.Active ? "ACTIVE" : "inactive");
			if (ModLoader.alreadyInitialisedMods.Contains(loadedMod))
			{
				stringBuilder.Append("\t\tHas been initialised\n");
			}
			if (loadedMod.HasErrors)
			{
				stringBuilder.Append("\t\tMalfunctioning\n");
			}
			stringBuilder.AppendLine();
		}
		stringBuilder.AppendLine();
		return stringBuilder.ToString();
	}

	public void Export()
	{
		string debugInfoString = GetDebugInfoString();
		try
		{
			File.WriteAllText("exported game debug info.md", debugInfoString);
			string full = Path.GetFullPath("exported game debug info.md");
			DialogBoxManager.Dialog("Exported to\n" + full, new DialogButton("Show in Explorer", false, delegate
			{
				Process.Start("explorer.exe", "/select,\"" + full + "\"");
			}), new DialogButton("Open file", false, delegate
			{
				Process.Start(full);
			}), new DialogButton("Okay", true)).SetHeight(400f);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning(ex);
			DialogBoxManager.Notification("Could not save file.\n" + ex.Message);
		}
	}
}

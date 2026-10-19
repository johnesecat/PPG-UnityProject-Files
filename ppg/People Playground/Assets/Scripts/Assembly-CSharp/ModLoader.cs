using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ModModels;
using Newtonsoft.Json;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class ModLoader
{
	public const string CompiledAssemblyFolder = "CompiledModAssemblies";

	public const string ModCompilerWorkingDirectory = "ppgModCompiler";

	public const string RelativeModCompilerPath = "PPGModCompiler.exe";

	public const string MetaFileName = "mod.json";

	public const string ModFolder = "Mods";

	public static List<ModMetaData> LoadedMods;

	public static Dictionary<ModMetaData, ModScript> ModScripts;

	private static readonly Dictionary<ModMetaData, Sprite> thumbnails;

	internal static HashSet<ModMetaData> alreadyInitialisedMods;

	internal static string[] assemblyLocations;

	public static event EventHandler ModListChanged;

	internal static void InvokeModListChange()
	{
		ModLoader.ModListChanged?.Invoke(null, EventArgs.Empty);
	}

	static ModLoader()
	{
		LoadedMods = new List<ModMetaData>();
		ModScripts = new Dictionary<ModMetaData, ModScript>();
		thumbnails = new Dictionary<ModMetaData, Sprite>();
		alreadyInitialisedMods = new HashSet<ModMetaData>();
		assemblyLocations = new string[22]
		{
			Path.GetFullPath("People Playground_Data\\Managed\\netstandard.dll"),
			typeof(float).Assembly.Location,
			typeof(ParticleSystem).Assembly.Location,
			typeof(AudioClip).Assembly.Location,
			typeof(Input).Assembly.Location,
			typeof(UnityEvent).Assembly.Location,
			typeof(TextMesh).Assembly.Location,
			typeof(TextMeshProUGUI).Assembly.Location,
			typeof(MaskableGraphic).Assembly.Location,
			typeof(VideoClip).Assembly.Location,
			typeof(Canvas).Assembly.Location,
			typeof(Physics2D).Assembly.Location,
			typeof(JsonConvert).Assembly.Location,
			typeof(Physics).Assembly.Location,
			typeof(Debug).Assembly.Location,
			typeof(PhysicalBehaviour).Assembly.Location,
			typeof(SpriteMask).Assembly.Location,
			typeof(UnityEngine.Object).Assembly.Location,
			typeof(Enum).Assembly.Location,
			typeof(AimConstraint).Assembly.Location,
			typeof(Enumerable).Assembly.Location,
			typeof(HashSet<>).Assembly.Location
		};
	}

	internal static Assembly GetModAssembly(ModMetaData mod)
	{
		if (ModScripts.TryGetValue(mod, out var value))
		{
			return value.LoadedAssembly;
		}
		return null;
	}

	internal static ModMetaData GetModMetaFromAssembly(Assembly assembly)
	{
		foreach (KeyValuePair<ModMetaData, ModScript> modScript in ModScripts)
		{
			if (modScript.Value.LoadedAssembly == assembly)
			{
				return modScript.Key;
			}
		}
		return null;
	}

	internal static void RemoveMod(ModMetaData mod)
	{
		LoadedMods.Remove(mod);
		ModScripts.Remove(mod);
	}

	internal static (ModCompilationResult result, ModScript script) Compile(ModMetaData mod)
	{
		if (!ShouldRecompile(mod))
		{
			BackgroundItemLoaderStatusBehaviour.SetDisplayState("Reading");
			string outputAssemblyPath = GetOutputAssemblyPath(mod);
			try
			{
				return (new ModCompilationResult(success: true, null, suspicious: false), new ModScript
				{
					AssemblyData = File.ReadAllBytes(outputAssemblyPath),
					AbsoluteScriptPaths = GetModSourcePaths(mod).ToArray(),
					RelativePath = mod.MetaPath
				});
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to read mod assembly meta file: " + ex.Message);
			}
		}
		ModScript modScript;
		ModCompilationResult item = TryCompileMod(mod, out modScript);
		mod.HasErrors = !item.Success;
		return (item, modScript);
	}

	internal static void UploadMod(ModMetaData modMeta)
	{
		if (Utils.IsFamilyShared())
		{
			Debug.LogWarning("Invalid Steam user for mod upload");
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification("Mods cannot be uploaded from family shared games.");
			return;
		}
		if (thumbnails.TryGetValue(modMeta, out var value))
		{
			UnityEngine.Object.Destroy(value);
			thumbnails.Remove(modMeta);
		}
		Sprite thumbnail = GetThumbnail(modMeta);
		if (thumbnail == null)
		{
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification("Mods require a thumbnail before they can be uploaded to the Workshop.");
			return;
		}
		try
		{
			Utils.AssertValidThumbnail(Path.Combine(modMeta.MetaLocation, modMeta.ThumbnailPath));
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Invalid thumbnail: " + ex);
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification(ex.Message);
			return;
		}
		if (thumbnail.texture.width != thumbnail.texture.height)
		{
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification("The thumbnail resolution is invalid: image is not square.");
			return;
		}
		if (thumbnail.texture.width < 64)
		{
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification("The thumbnail resolution is invalid: image should be at least 64x64 (ideally 512x512).");
			return;
		}
		if (thumbnail.texture.width > 512)
		{
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification("The thumbnail resolution is invalid: image should be at most 512x512.");
			return;
		}
		if (!modMeta.Active)
		{
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification("Your mod needs to be active to be uploaded.");
			return;
		}
		string name = SteamClient.Name;
		if (modMeta.Author != name)
		{
			UISoundBehaviour.Main.Warning();
			DialogBoxManager.Dialog("The mod author \"" + modMeta.Author + "\" does not match your current username \"" + name + "\". Are you sure you want to continue?", new DialogButton("Yes", true, delegate
			{
				SteamWorkshopModUploader.Main.PublishMod(modMeta);
			}), new DialogButton("No", true));
		}
		else
		{
			SteamWorkshopModUploader.Main.PublishMod(modMeta);
		}
	}

	internal static ModMetaData LoadModAt(string filePath)
	{
		Debug.Log("Attempt load mod at " + filePath);
		ModMetaData modMetaData;
		try
		{
			modMetaData = JsonConvert.DeserializeObject<ModMetaData>(File.ReadAllText(filePath));
			modMetaData.MetaPath = filePath;
			modMetaData.MetaLocation = Path.GetDirectoryName(filePath);
			string text = modMetaData.MetaLocation + "/README.txt";
			if (File.Exists(text))
			{
				if (new FileInfo(text).Length > 5000)
				{
					Debug.LogWarningFormat("README.txt at '{0}' exceeded the maximum filesize of 5 KB", text);
				}
				else
				{
					string text2 = File.ReadAllText(text);
					text2 = text2.Replace("<page", "<");
					text2 = text2.Replace("<link", "<");
					text2 = text2.Replace("<font", "<");
					text2 = text2.Replace("<sprite", "<");
					text2 = (modMetaData.RichReadme = text2.Replace("<style", "<"));
				}
			}
			LoadedMods.Add(modMetaData);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return null;
		}
		Debug.Log("Loaded mod: " + modMetaData.Name);
		return modMetaData;
	}

	public static Sprite GetThumbnail(ModMetaData modmeta)
	{
		if (thumbnails.TryGetValue(modmeta, out var value))
		{
			return value;
		}
		try
		{
			string text = Path.Combine(modmeta.MetaLocation, modmeta.ThumbnailPath);
			if (!File.Exists(text))
			{
				return null;
			}
			Sprite sprite = LoadTexture(text);
			thumbnails.Add(modmeta, sprite);
			return sprite;
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return null;
		}
	}

	public static void SetModActive(ModMetaData modmeta, bool active = true)
	{
		if (modmeta.MetaLocation == null)
		{
			throw new ArgumentException(modmeta.Name + " is unloaded");
		}
		modmeta.Active = active;
		if (modmeta.Active)
		{
			ModificationManager.InvokeOnLoad(modmeta);
		}
		UpdateJSON(modmeta);
		try
		{
			ModExtraMetadataLoader.StoreActive();
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to sync extra meta data: " + ex);
		}
	}

	internal static void UpdateJSON(ModMetaData modmeta)
	{
		string contents = JsonConvert.SerializeObject(modmeta, Formatting.Indented);
		File.WriteAllText(modmeta.MetaPath, contents);
	}

	private static Sprite LoadTexture(string fullPath, FilterMode mode = FilterMode.Bilinear)
	{
		byte[] data = File.ReadAllBytes(fullPath);
		Texture2D texture2D = new Texture2D(0, 0);
		if (!texture2D.LoadImage(data))
		{
			throw new InvalidDataException("Texture at " + fullPath + " cannot be loaded as a PNG");
		}
		texture2D.filterMode = mode;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), 0.5f * Vector2.one, 256f);
	}

	private static ModCompilationResult TryCompileMod(ModMetaData modmeta, out ModScript modScript)
	{
		BackgroundItemLoaderStatusBehaviour.SetDisplayState("Preparing");
		modScript = new ModScript();
		string assemblyMetaPath = GetAssemblyMetaPath(modmeta);
		try
		{
			modScript.AbsoluteScriptPaths = GetModSourcePaths(modmeta).ToArray();
			modScript.RelativePath = modmeta.MetaLocation;
			ModCompileInstructions instructions = new ModCompileInstructions
			{
				MainClass = modmeta.EntryPoint,
				Paths = modScript.AbsoluteScriptPaths,
				OutputFileName = GetOutputAssemblyPath(modmeta),
				RejectShadyCode = (!TrustCache.IsTrusted(modmeta) && (UserPreferenceManager.Current?.RejectShadyCode ?? true)),
				AssemblyReferenceLocations = assemblyLocations
			};
			if (!Directory.Exists("CompiledModAssemblies"))
			{
				Directory.CreateDirectory("CompiledModAssemblies");
			}
			if (File.Exists(instructions.OutputFileName))
			{
				File.Delete(instructions.OutputFileName);
			}
			BackgroundItemLoaderStatusBehaviour.SetDisplayState("Sending request");
			Task<CompilerReply> task = Task.Run(() => CompilerClient.RequestCompilationSynchronous(instructions));
			while (!task.IsCompleted)
			{
				Thread.Sleep(16);
			}
			BackgroundItemLoaderStatusBehaviour.SetDisplayState("Reading");
			Thread.Sleep(16);
			if (task.IsFaulted)
			{
				string errors = "Task faulted without exception thrown";
				if (task.Exception != null)
				{
					AggregateException exception = task.Exception;
					errors = ((exception == null) ? task.Exception.ToString() : string.Join(Environment.NewLine, exception.InnerExceptions.Select((Exception s) => s.ToString())));
				}
				return new ModCompilationResult(success: false, errors, suspicious: false);
			}
			CompilerReply result = task.Result;
			switch (result.State)
			{
			case CompilationState.Error:
				if (File.Exists(assemblyMetaPath))
				{
					File.Delete(assemblyMetaPath);
				}
				return new ModCompilationResult(success: false, result.Message, result.Suspicious);
			default:
				return new ModCompilationResult(success: false, "Invalid reply from compiler", result.Suspicious);
			case CompilationState.Success:
				try
				{
					byte[] assemblyData = File.ReadAllBytes(instructions.OutputFileName);
					modScript.AssemblyData = assemblyData;
					IEnumerable<byte> modAssemblyMeta = GetModAssemblyMeta(modmeta);
					using (FileStream fileStream = File.Open(assemblyMetaPath, FileMode.Create))
					{
						foreach (byte item in modAssemblyMeta)
						{
							fileStream.WriteByte(item);
						}
						fileStream.Close();
					}
				}
				catch (Exception ex)
				{
					if (File.Exists(assemblyMetaPath))
					{
						File.Delete(assemblyMetaPath);
					}
					return new ModCompilationResult(success: false, ex.ToString(), result.Suspicious);
				}
				return new ModCompilationResult(success: true, null, suspicious: false);
			}
		}
		catch (Exception ex2)
		{
			if (File.Exists(assemblyMetaPath))
			{
				File.Delete(assemblyMetaPath);
			}
			return new ModCompilationResult(success: false, ex2.ToString(), suspicious: false);
		}
	}

	private static IEnumerable<string> GetModSourcePaths(ModMetaData modmeta)
	{
		for (int i = 0; i < modmeta.Scripts.Length; i++)
		{
			string path = modmeta.Scripts[i];
			yield return Path.GetFullPath(Path.Combine(modmeta.MetaLocation, path));
		}
	}

	private static string AlphaNumeric(string a)
	{
		return Regex.Replace(a, "[^a-zA-Z0-9 -]", "");
	}

	private static string GetOutputAssemblyPath(ModMetaData mod)
	{
		return Path.GetFullPath("CompiledModAssemblies\\" + AlphaNumeric(mod.GetUniqueName()) + ".dll");
	}

	private static string GetAssemblyMetaPath(ModMetaData mod)
	{
		return Path.GetFullPath("CompiledModAssemblies\\" + AlphaNumeric(mod.GetUniqueName()) + ".meta");
	}

	public static IEnumerable<byte> GetModAssemblyMeta(ModMetaData mod)
	{
		List<byte> list = new List<byte>();
		foreach (string modSourcePath in GetModSourcePaths(mod))
		{
			if (File.Exists(modSourcePath))
			{
				list.AddRange(Utils.GetMD5ForFile(modSourcePath));
			}
		}
		list.AddRange(Utils.GetMD5(mod.ModVersion));
		list.AddRange(Utils.GetMD5(mod.GameVersion));
		list.AddRange(Utils.GetMD5("1.27.5"));
		return list;
	}

	public static bool ShouldRecompile(ModMetaData mod)
	{
		if (mod.HasErrors)
		{
			Debug.LogFormat("{0} recompiled because it had errors", mod.Name);
			return true;
		}
		string assemblyMetaPath = GetAssemblyMetaPath(mod);
		string outputAssemblyPath = GetOutputAssemblyPath(mod);
		if (!File.Exists(assemblyMetaPath) || !File.Exists(outputAssemblyPath))
		{
			Debug.LogFormat("{0} recompiled because it had missing files (probably because it was malfunctioning and was not compiled)", mod.Name);
			return true;
		}
		byte[] array = File.ReadAllBytes(assemblyMetaPath);
		try
		{
			byte[] array2 = GetModAssemblyMeta(mod).ToArray();
			if (array2.Length != array.Length)
			{
				Debug.LogFormat("{0} recompiled because the hash does not match", mod.Name);
				return true;
			}
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i] != array[i])
				{
					Debug.LogFormat("{0} recompiled because the hash does not match", mod.Name);
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Failed to compute mod hash for " + mod.Name + ": " + ex.Message);
			return true;
		}
		return false;
	}
}

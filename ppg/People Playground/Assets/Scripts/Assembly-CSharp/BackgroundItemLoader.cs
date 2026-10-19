using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steamworks;
using Steamworks.Ugc;
using UnityEngine;

public class BackgroundItemLoader : MonoBehaviour
{
	public Map[] BuiltInMaps;

	public Sprite SteamWorkshopThumbnail;

	private static bool alreadyStarted;

	public static BackgroundItemLoader Instance { get; private set; }

	private void Awake()
	{
		if ((bool)Instance)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		if (!alreadyStarted)
		{
			StartCoroutine(LoadingCoroutine());
		}
	}

	private IEnumerator LoadingCoroutine()
	{
		alreadyStarted = true;
		MapRegistry.Clear();
		if (BuiltInMaps != null)
		{
			for (int i = 0; i < BuiltInMaps.Length; i++)
			{
				try
				{
					MapRegistry.Register(BuiltInMaps[i]);
				}
				catch (Exception message)
				{
					UnityEngine.Debug.LogWarning(message);
				}
			}
		}
		ModLoader.alreadyInitialisedMods.Clear();
		ModLoader.LoadedMods.Clear();
		ModLoader.ModScripts.Clear();
		TrustCache.Load();
		if (SteamworksInitialiser.IsInitialised)
		{
			yield return DoAsyncTask(async delegate
			{
				await LoadSubscriptionList();
			}, "Steam Workshop", "Retrieving list", SteamWorkshopThumbnail);
			ContraptionMetaData[] allContraptions = null;
			yield return DoAsyncTask(async delegate
			{
				allContraptions = ContraptionSerialiser.GetAllContraptions().ToArray();
				await Task.Delay(16);
			}, "Contraptions", "Reading", SteamWorkshopThumbnail);
			yield return DoAsyncTask(async delegate
			{
				await DeleteUnsubscribedWorkshopContraptions(allContraptions);
			}, "Contraptions", "Synchronising", SteamWorkshopThumbnail);
			HashSet<string> persistentActiveModIds = ModExtraMetadataLoader.Deserialise("externalactives");
			foreach (Item item in LoadedWorkshopSubscriptions.All)
			{
				if (item.Result != Result.OK || !item.IsSubscribed)
				{
					continue;
				}
				if (!item.IsInstalled || item.NeedsUpdate)
				{
					if (item.Download())
					{
						yield return DownloadWorkshopItem(item);
					}
					else
					{
						UnityEngine.Debug.LogWarning("Could not start downloading " + item.Title);
						if (!item.IsInstalled)
						{
							continue;
						}
					}
				}
				else if (item.IsDownloading || item.IsDownloadPending)
				{
					yield return DownloadWorkshopItem(item);
				}
				if (item.HasTag("Mods"))
				{
					LoadedWorkshopSubscriptions.Mods.Add(item);
					ModMetaData modMeta = null;
					yield return DoAsyncTask(async delegate
					{
						string text = item.Directory + "\\mod.json";
						if (File.Exists(text))
						{
							modMeta = ModLoader.LoadModAt(text);
						}
						else
						{
							UnityEngine.Debug.LogWarning("Meta file for " + item.Title + " does not exist");
						}
						await Task.Delay(16);
					}, item.Title, "Parsing metadata", SteamWorkshopThumbnail);
					if (modMeta != null)
					{
						(ModCompilationResult result, ModScript script) result = default((ModCompilationResult, ModScript));
						yield return DoAsyncTask(async delegate
						{
							result = await CompileMod(modMeta);
						}, item.Title, "Processing", ModLoader.GetThumbnail(modMeta));
						ProcessModCompilationResult(result, modMeta);
						modMeta.UGCIdentity = item.Id.ToString();
						modMeta.Active = persistentActiveModIds.Contains(modMeta.UGCIdentity);
					}
				}
				else
				{
					LoadedWorkshopSubscriptions.Contraptions.Add(item);
					yield return DoAsyncTask(async delegate
					{
						LoadWorkshopContraption(item, allContraptions);
						await Task.Delay(16);
					}, item.Title, "Processing", SteamWorkshopThumbnail);
				}
				yield return new WaitForEndOfFrame();
			}
		}
		if (!Directory.Exists("Mods"))
		{
			Directory.CreateDirectory("Mods");
		}
		else
		{
			IEnumerable<string> enumerable = Directory.EnumerateFiles("Mods", "mod.json", SearchOption.AllDirectories);
			foreach (string item2 in enumerable)
			{
				ModMetaData modMeta2 = ModLoader.LoadModAt(item2);
				if (modMeta2 != null)
				{
					(ModCompilationResult result, ModScript script) result2 = default((ModCompilationResult, ModScript));
					yield return DoAsyncTask(async delegate
					{
						result2 = await CompileMod(modMeta2);
					}, modMeta2.Name, "Processing", ModLoader.GetThumbnail(modMeta2));
					ProcessModCompilationResult(result2, modMeta2);
				}
			}
		}
		ModLoader.InvokeModListChange();
		ModificationManager.InvokeOnLoad();
		if ((bool)UnityEngine.Object.FindObjectOfType<NotificationControllerBehaviour>())
		{
			NotificationControllerBehaviour.Show("Mod compilation complete, make sure to restart the map to apply activated mods!");
		}
	}

	public void ProcessModCompilationResult((ModCompilationResult result, ModScript script) tuple, ModMetaData mod)
	{
		BackgroundItemLoaderStatusBehaviour.Show(base.name, ModLoader.GetThumbnail(mod));
		BackgroundItemLoaderStatusBehaviour.SetDisplayState("Processing");
		var (modCompilationResult, modScript) = tuple;
		if (modCompilationResult.Success)
		{
			try
			{
				BackgroundItemLoaderStatusBehaviour.SetDisplayState("Loading");
				modScript.LoadedAssembly = Assembly.Load(modScript.AssemblyData);
				modScript.AssemblyData = null;
				if (ModLoader.ModScripts.ContainsKey(mod))
				{
					ModLoader.ModScripts.Remove(mod);
					ModLoader.ModScripts.Add(mod, modScript);
				}
				else
				{
					ModLoader.ModScripts.Add(mod, modScript);
				}
			}
			catch (Exception ex)
			{
				mod.Active = false;
				mod.HasErrors = true;
				mod.Errors = ex.Message;
				mod.Suspicious = modCompilationResult.Suspicious;
				UnityEngine.Debug.LogWarning("Could not load mod assembly " + mod.Name + ":\n" + ex.Message + "\n" + ex.StackTrace);
			}
		}
		else
		{
			mod.Active = false;
			mod.Errors = modCompilationResult.Errors;
			mod.Suspicious = modCompilationResult.Suspicious;
			UnityEngine.Debug.LogWarning("Could not compile mod " + mod.Name + ":\n" + modCompilationResult.Errors);
		}
		BackgroundItemLoaderStatusBehaviour.Hide();
	}

	private void OnApplicationQuit()
	{
		TrustCache.Save();
		TimeSpan zero = TimeSpan.Zero;
		foreach (ModMetaData loadedMod in ModLoader.LoadedMods)
		{
			if (loadedMod.Active)
			{
				TimeSpan timeSpan = ModificationManager.InvokeOnUnload(loadedMod);
				zero += timeSpan;
			}
		}
		UnityEngine.Debug.Log($"Unloading mods took {zero.TotalMilliseconds} ms");
	}

	public async Task<(ModCompilationResult result, ModScript script)> CompileMod(ModMetaData mod)
	{
		if (mod == null)
		{
			ModCompilationResult item = default(ModCompilationResult);
			item.Errors = "Mod not installed";
			item.Success = false;
			return (item, null);
		}
		await Task.CompletedTask;
		return ModLoader.Compile(mod);
	}

	private IEnumerator DownloadWorkshopItem(Item item)
	{
		TimeSpan timeout = TimeSpan.FromSeconds(10.0);
		Stopwatch watch = new Stopwatch();
		watch.Start();
		BackgroundItemLoaderStatusBehaviour.Show(item.Title, SteamWorkshopThumbnail);
		BackgroundItemLoaderStatusBehaviour.SetDisplayState("Waiting for Steam");
		yield return new WaitUntil(() => item.IsDownloading || watch.Elapsed > timeout);
		if (watch.Elapsed > timeout)
		{
			BackgroundItemLoaderStatusBehaviour.SetDisplayState("Steam took too long to respond!");
			yield return new WaitForSecondsRealtime(0.25f);
			BackgroundItemLoaderStatusBehaviour.Hide();
			yield break;
		}
		watch.Reset();
		timeout = TimeSpan.FromSeconds(60.0);
		watch.Restart();
		BackgroundItemLoaderStatusBehaviour.SetDisplayState("Downloading");
		while (!item.IsInstalled)
		{
			if (watch.Elapsed > timeout)
			{
				UnityEngine.Debug.LogWarningFormat("{0} is taking over {1} seconds to download and will be skipped", item.Title, timeout.Seconds);
				BackgroundItemLoaderStatusBehaviour.Hide();
				watch.Stop();
				yield break;
			}
			BackgroundItemLoaderStatusBehaviour.SetDisplayState($"{Mathf.RoundToInt(item.DownloadAmount * 100f)}% downloaded");
			yield return new WaitForEndOfFrame();
		}
		BackgroundItemLoaderStatusBehaviour.Hide();
	}

	private async Task DeleteUnsubscribedWorkshopContraptions(ContraptionMetaData[] allContraptions)
	{
		foreach (ContraptionMetaData contr in allContraptions)
		{
			if (!contr.IsWorkshopItem)
			{
				continue;
			}
			try
			{
				Item? item = await Utils.TaskTimeout(Item.GetAsync(contr.PublishedFileID), TimeSpan.FromSeconds(20.0));
				if (!item.HasValue || !item.Value.IsSubscribed)
				{
					try
					{
						Directory.Delete("Contraptions//" + contr.Name, recursive: true);
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogWarningFormat("Unable to delete folder for {0}: {1}", contr.DisplayName, ex);
					}
				}
			}
			catch (Exception ex2)
			{
				UnityEngine.Debug.LogWarningFormat("Failed to synchronise {0}: {1}", contr.DisplayName, ex2);
			}
		}
	}

	private void LoadWorkshopContraption(Item item, ContraptionMetaData[] allContraptions)
	{
		if (!item.IsInstalled)
		{
			UnityEngine.Debug.LogWarningFormat("Attempt to load contraption that is not installed: {0}", item.Id);
			return;
		}
		foreach (ContraptionMetaData contraptionMetaData in allContraptions)
		{
			if (contraptionMetaData.IsWorkshopItem && item.Id == contraptionMetaData.PublishedFileID)
			{
				return;
			}
		}
		string text = string.Format("{0}{1}/", "Contraptions/", item.Id);
		if (!Directory.Exists(text))
		{
			try
			{
				Directory.CreateDirectory(text);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogWarningFormat("Failed to create directory for {0}: {1}", item.Title, ex);
				return;
			}
		}
		string[] files = Directory.GetFiles(item.Directory);
		foreach (string obj in files)
		{
			File.Copy(obj, string.Format(arg2: new FileInfo(obj).Extension, format: "{0}{1}{2}", arg0: text, arg1: item.Id), overwrite: true);
		}
		string text2 = text + item.Id.ToString() + ".json";
		ContraptionMetaData contraptionMetaData2;
		try
		{
			contraptionMetaData2 = JsonConvert.DeserializeObject<ContraptionMetaData>(File.ReadAllText(text2));
		}
		catch (Exception ex2)
		{
			UnityEngine.Debug.LogWarningFormat("Malformed contraption ignored at {0}: {1}", text2, ex2);
			return;
		}
		contraptionMetaData2.Name = item.Id.Value.ToString();
		contraptionMetaData2.DisplayName = item.Title;
		contraptionMetaData2.PublishedFileID = item.Id;
		contraptionMetaData2.CreatorUGCIdentity = null;
		File.WriteAllText(string.Format("{0}{1}{2}", text, item.Id, ".json"), JsonConvert.SerializeObject(contraptionMetaData2));
	}

	private async Task LoadSubscriptionList()
	{
		Query subscriptions = Query.UsableInGame.WhereUserSubscribed(SteamClient.SteamId);
		LoadedWorkshopSubscriptions.All.Clear();
		LoadedWorkshopSubscriptions.Mods.Clear();
		LoadedWorkshopSubscriptions.Contraptions.Clear();
		int pageNumber = 1;
		while (true)
		{
			ResultPage? resultPage = await subscriptions.GetPageAsync(pageNumber);
			if (!resultPage.HasValue || resultPage.Value.ResultCount == 0 || resultPage.Value.TotalCount == 0)
			{
				break;
			}
			foreach (Item entry in resultPage.Value.Entries)
			{
				if (entry.Result != Result.OK)
				{
					UnityEngine.Debug.LogWarning("Failed to retrieve entry in subscription query page");
				}
				else if ((uint)entry.CreatorApp == 1118200)
				{
					LoadedWorkshopSubscriptions.All.Add(entry);
				}
			}
			resultPage.Value.Dispose();
			pageNumber++;
		}
		UnityEngine.Debug.LogFormat("Retrieved {0} subscriptions", LoadedWorkshopSubscriptions.All.Count);
	}

	public IEnumerator DoAsyncTask(Func<Task> action, string name, string description, Sprite sprite = null)
	{
		BackgroundItemLoaderStatusBehaviour.Show(name, sprite);
		BackgroundItemLoaderStatusBehaviour.SetDisplayState(description);
		Task task = Task.Run(action);
		string taskName = name + " " + description;
		UnityEngine.Debug.LogFormat("Started background loading task: {0}", taskName);
		yield return new WaitUntil(() => task.IsCompleted);
		UnityEngine.Debug.LogFormat("Finished background loading task: {0}", taskName);
		if (task.IsFaulted)
		{
			foreach (Exception innerException in task.Exception.InnerExceptions)
			{
				UnityEngine.Debug.LogError(innerException);
			}
		}
		BackgroundItemLoaderStatusBehaviour.Hide();
		task.Dispose();
	}
}

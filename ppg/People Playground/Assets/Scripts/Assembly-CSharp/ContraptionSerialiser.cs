using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ceras;
using Ceras.Formatters;
using Newtonsoft.Json;
using UnityEngine;

public class ContraptionSerialiser
{
	public const string Path = "Contraptions/";

	public static readonly Vector2Int ThumbnailSize = new Vector2Int(256, 256);

	public const float ThumbnailPadding = 1f;

	public const string ThumbnailExtension = ".png";

	private static CerasSerializer cerasSerializer;

	public static void Initialise()
	{
		SerializerConfig config = new SerializerConfig();
		CerasUnityFormatters.ApplyToConfig(config);
		CustomStringFormatter customStringFormatter = new CustomStringFormatter();
		cerasSerializer = new CerasSerializer(config);
		StringFormatter.DeserializeOverride = customStringFormatter.Deserialize;
		StringFormatter.SerializeOverride = customStringFormatter.Serialize;
	}

	public static IEnumerable<ContraptionMetaData> GetAllContraptions()
	{
		if (!Directory.Exists("Contraptions/"))
		{
			yield break;
		}
		foreach (string item in Directory.EnumerateFiles("Contraptions/", "*.json", SearchOption.AllDirectories))
		{
			ContraptionMetaData contraptionMetaData;
			try
			{
				string value = File.ReadAllText(item);
				string fullName = new FileInfo(item).Directory.FullName;
				contraptionMetaData = JsonConvert.DeserializeObject<ContraptionMetaData>(value);
				contraptionMetaData.PathToMetadata = item;
				contraptionMetaData.PathToDataFile = System.IO.Path.Combine(fullName, contraptionMetaData.Name + ".jaap");
				contraptionMetaData.PathToThumbnail = System.IO.Path.Combine(fullName, contraptionMetaData.Name + ".png");
				contraptionMetaData.PathToOutlineFile = System.IO.Path.Combine(fullName, contraptionMetaData.Name + ".outline");
				if (string.IsNullOrWhiteSpace(contraptionMetaData.DisplayName))
				{
					contraptionMetaData.DisplayName = contraptionMetaData.Name;
				}
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Invalid JSON file found\n" + item);
				Debug.LogException(exception);
				continue;
			}
			if (contraptionMetaData != null)
			{
				yield return contraptionMetaData;
			}
		}
	}

	public static void SaveContraption(string name, ObjectState[] states)
	{
		ContraptionMetaData metaData = new ContraptionMetaData(name);
		Contraption obj = new Contraption(states);
		string path = "Contraptions/" + name + "/";
		if (!Directory.Exists("Contraptions/"))
		{
			Directory.CreateDirectory("Contraptions/");
		}
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		Type bloodContainerType = typeof(BloodContainer);
		Assembly gameAssembly = Assembly.GetAssembly(typeof(PhysicalBehaviour));
		foreach (ObjectState objectState in states)
		{
			MonoBehaviourPrototype[] rootComponentData = objectState.RootComponentData;
			for (int j = 0; j < rootComponentData.Length; j++)
			{
				addRequiredModIfApplicable(rootComponentData[j]);
			}
			foreach (KeyValuePair<string, MonoBehaviourPrototype[]> childComponentDatum in objectState.ChildComponentData)
			{
				rootComponentData = childComponentDatum.Value;
				for (int j = 0; j < rootComponentData.Length; j++)
				{
					addRequiredModIfApplicable(rootComponentData[j]);
				}
			}
			SpawnableAsset spawnable = CatalogBehaviour.Main.GetSpawnable(objectState.SpawnableAssetName);
			if ((bool)spawnable)
			{
				addRequiredMod(CatalogBehaviour.Main.GetModOfSpawnable(spawnable));
				continue;
			}
			Debug.LogErrorFormat("Trying to save invalid spawnable: \"{0}\"??", objectState.SpawnableAssetName);
		}
		FileStream fileStream = new FileStream(Contraption.GetPath(name), FileMode.Create);
		try
		{
			byte[] array = cerasSerializer.Serialize(obj);
			fileStream.Write(array, 0, array.Length);
			string path2 = ContraptionMetaData.GetPath(name);
			if (File.Exists(path2))
			{
				ModMetaData modMetaData = JsonConvert.DeserializeObject<ModMetaData>(File.ReadAllText(path2));
				metaData.CreatorUGCIdentity = modMetaData.CreatorUGCIdentity;
			}
			File.WriteAllText(path2, JsonConvert.SerializeObject(metaData));
			NonSteamStatManager.Stats.Increment("CONTRAPTIONS_SAVED");
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			fileStream.Close();
		}
		void addRequiredMod(ModMetaData requiredMod)
		{
			if (requiredMod != null)
			{
				string uniqueId = requiredMod.GetUniqueName();
				if (requiredMod != null && !metaData.RequiredMods.Any((RequiredMod r) => r.UniqueIdentity == uniqueId))
				{
					metaData.RequiredMods.Add(new RequiredMod
					{
						Name = requiredMod.Name,
						UniqueIdentity = uniqueId,
						WorkshopId = (requiredMod.UGCIdentity ?? requiredMod.CreatorUGCIdentity)
					});
					Debug.LogFormat("Contraption that is being saved requires {0}", requiredMod.Name);
				}
			}
		}
		void addRequiredModIfApplicable(MonoBehaviourPrototype comp)
		{
			if (bloodContainerType.IsAssignableFrom(comp.Type) && comp.storedFields.TryGetValue("SerialisableDistributions", out var value))
			{
				BloodContainer.SerialisableDistribution[] array2 = value as BloodContainer.SerialisableDistribution[];
				if (array2 != null)
				{
					for (int k = 0; k < array2.Length; k++)
					{
						Liquid liquid = Liquid.GetLiquid(array2[k].LiquidID);
						if (liquid != null)
						{
							Assembly assembly = liquid.GetType().Assembly;
							if (assembly != gameAssembly)
							{
								addRequiredMod(ModLoader.GetModMetaFromAssembly(assembly));
							}
						}
					}
				}
			}
			Assembly assembly2 = comp.Type.Assembly;
			if (assembly2 != gameAssembly)
			{
				addRequiredMod(ModLoader.GetModMetaFromAssembly(assembly2));
			}
		}
	}

	public static Contraption LoadContraption(ContraptionMetaData metadata)
	{
		if (metadata.Version != "1.27.5")
		{
			NotificationControllerBehaviour.Show("Game version mismatch. This might cause problems.");
			Debug.LogWarning("Attempt to load save from " + metadata.Version + " into 1.27.5");
		}
		Contraption value = new Contraption(new ObjectState[0]);
		byte[] buffer = File.ReadAllBytes(metadata.PathToDataFile);
		try
		{
			cerasSerializer.Deserialize(ref value, buffer);
		}
		catch (Exception exception)
		{
			Debug.LogWarning("Error deserialising " + metadata.Name);
			Debug.LogException(exception);
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification("\"" + metadata.Name + "\" is corrupted");
			Initialise();
			return new Contraption(new ObjectState[0]);
		}
		if (string.IsNullOrWhiteSpace(metadata.DisplayName))
		{
			metadata.DisplayName = metadata.Name;
		}
		ObjectState[] objectStates = value.ObjectStates;
		foreach (ObjectState objectState in objectStates)
		{
			if (string.IsNullOrWhiteSpace(objectState.SpawnableAssetName))
			{
				continue;
			}
			SpawnableAsset spawnableAsset = ModAPI.FindSpawnable(objectState.SpawnableAssetName);
			if (spawnableAsset == null || spawnableAsset.MigrationEvents == null)
			{
				continue;
			}
			MigrationEvent[] migrationEvents = spawnableAsset.MigrationEvents;
			foreach (MigrationEvent migrationEvent in migrationEvents)
			{
				if (migrationEvent != null && migrationEvent.IsApplicable(metadata.Version))
				{
					objectState.SpawnableAssetName = migrationEvent.ToSpawnInstead.name;
				}
			}
		}
		return value;
	}

	public static Texture2D LoadThumbnail(ContraptionMetaData meta)
	{
		string pathToThumbnail = meta.PathToThumbnail;
		Texture2D texture2D = new Texture2D(0, 0);
		try
		{
			byte[] data = File.ReadAllBytes(pathToThumbnail);
			if (!texture2D.LoadImage(data))
			{
				throw new Exception("Thumbnail data for " + meta.Name + " is unreadable");
			}
			texture2D.Apply(updateMipmaps: true, makeNoLongerReadable: true);
			return texture2D;
		}
		catch (Exception exception)
		{
			Debug.LogWarning("Error while loading thumbnail for " + meta.Name);
			Debug.LogException(exception);
			if ((bool)texture2D)
			{
				UnityEngine.Object.Destroy(texture2D);
			}
			return Resources.Load<Texture2D>("Sprites/ErrorWhileLoadingThumbnail");
		}
	}

	public static void SaveThumbnail(ObjectState[] states, string name)
	{
		string path = "Contraptions/" + name + "/";
		if (!Directory.Exists("Contraptions/"))
		{
			Directory.CreateDirectory("Contraptions/");
		}
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		Texture2D texture2D = ThumbnailCreator.Main.CreateThumbnail(states, 512);
		File.WriteAllBytes("Contraptions/" + name + "/" + name + ".png", texture2D.EncodeToPNG());
		UnityEngine.Object.Destroy(texture2D);
	}

	public static void ConvertToDirectoryBased(ContraptionMetaData contraption)
	{
		if (contraption.IsLegacyStructure())
		{
			string text = "Contraptions/" + contraption.Name;
			string text2 = "Contraptions/" + contraption.Name + "/" + contraption.Name;
			Directory.CreateDirectory("Contraptions/" + contraption.Name);
			File.Move(text + ".json", text2 + ".json");
			File.Move(text + ".png", text2 + ".png");
			File.Move(text + ".jaap", text2 + ".jaap");
		}
	}
}

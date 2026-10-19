using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ModAPI
{
	public const float PixelSize = 0.028571429f;

	internal static bool metaDataIsValid = false;

	private static readonly Dictionary<int, HashSet<string>> registeredInputsByMod = new Dictionary<int, HashSet<string>>();

	private static ModMetaData metadata;

	private static JsonSerializerSettings modJsonSerializerSettings = new JsonSerializerSettings
	{
		Formatting = Formatting.Indented,
		Culture = CultureInfo.InvariantCulture,
		Converters = UnityConverterInitializer.defaultUnityConvertersSettings.Converters,
		ContractResolver = new UnityTypeContractResolver()
	};

	public static ModMetaData Metadata
	{
		get
		{
			if (!metaDataIsValid)
			{
				throw new InvalidOperationException("Metadata can only be retrieved in the AfterSpawn, Main, or OnLoad function.");
			}
			return metadata;
		}
		internal set
		{
			metadata = value;
		}
	}

	public static ModDebugDrawer Draw { get; internal set; }

	public static event EventHandler<WireBehaviour> OnWireCreated;

	public static event EventHandler<PinBehaviour> OnPinCreated;

	public static event EventHandler<LinkDeviceBehaviour> OnLinkCreated;

	public static event EventHandler<WireBehaviour> OnWireDestroyed;

	public static event EventHandler<PinBehaviour> OnPinDestroyed;

	public static event EventHandler<LinkDeviceBehaviour> OnLinkDestroyed;

	public static event EventHandler<UserSpawnEventArgs> OnItemSpawned;

	public static event EventHandler<UserSpawnEventArgs> OnItemRemoved;

	public static event EventHandler<PhysicalBehaviour> OnItemSelected;

	public static event EventHandler<PhysicalBehaviour> OnItemDeselected;

	public static event EventHandler<PhysicalBehaviour> OnItemActivated;

	public static event EventHandler<FirearmBehaviour> OnGunShot;

	public static event EventHandler<PersonBehaviour> OnDeath;

	internal static void ClearEvents()
	{
		ModAPI.OnWireCreated = null;
		ModAPI.OnPinCreated = null;
		ModAPI.OnLinkCreated = null;
		ModAPI.OnWireDestroyed = null;
		ModAPI.OnPinDestroyed = null;
		ModAPI.OnLinkDestroyed = null;
		ModAPI.OnItemSpawned = null;
		ModAPI.OnItemRemoved = null;
		ModAPI.OnItemSelected = null;
		ModAPI.OnItemDeselected = null;
		ModAPI.OnItemActivated = null;
		ModAPI.OnGunShot = null;
		ModAPI.OnDeath = null;
	}

	internal static void InvokeWireCreated(object sender, WireBehaviour args)
	{
		ModAPI.OnWireCreated?.Invoke(sender, args);
	}

	internal static void InvokePinCreated(object sender, PinBehaviour args)
	{
		ModAPI.OnPinCreated?.Invoke(sender, args);
	}

	internal static void InvokeLinkCreated(object sender, LinkDeviceBehaviour args)
	{
		ModAPI.OnLinkCreated?.Invoke(sender, args);
	}

	internal static void InvokeWireDestroyed(object sender, WireBehaviour args)
	{
		ModAPI.OnWireDestroyed?.Invoke(sender, args);
	}

	internal static void InvokePinDestroyed(object sender, PinBehaviour args)
	{
		ModAPI.OnPinDestroyed?.Invoke(sender, args);
	}

	internal static void InvokeLinkDestroyed(object sender, LinkDeviceBehaviour args)
	{
		ModAPI.OnLinkDestroyed?.Invoke(sender, args);
	}

	internal static void InvokeItemSpawned(object sender, UserSpawnEventArgs args)
	{
		ModAPI.OnItemSpawned?.Invoke(sender, args);
	}

	internal static void InvokeItemRemoved(object sender, UserSpawnEventArgs args)
	{
		ModAPI.OnItemRemoved?.Invoke(sender, args);
	}

	internal static void InvokeItemSelected(object sender, PhysicalBehaviour args)
	{
		ModAPI.OnItemSelected?.Invoke(sender, args);
	}

	internal static void InvokeItemDeselected(object sender, PhysicalBehaviour args)
	{
		ModAPI.OnItemDeselected?.Invoke(sender, args);
	}

	internal static void InvokeItemActivated(object sender, PhysicalBehaviour args)
	{
		ModAPI.OnItemActivated?.Invoke(sender, args);
	}

	internal static void InvokeGunShot(object sender, FirearmBehaviour args)
	{
		ModAPI.OnGunShot?.Invoke(sender, args);
	}

	internal static void InvokeDeath(object sender, PersonBehaviour args)
	{
		ModAPI.OnDeath?.Invoke(sender, args);
	}

	public static SpawnableAsset FindSpawnable(string name)
	{
		SpawnableAsset spawnableAsset = find(name);
		if (spawnableAsset == null)
		{
			return null;
		}
		if (metaDataIsValid && spawnableAsset.MigrationEvents != null && spawnableAsset.MigrationEvents.Length != 0)
		{
			MigrationEvent[] migrationEvents = spawnableAsset.MigrationEvents;
			foreach (MigrationEvent migrationEvent in migrationEvents)
			{
				if (migrationEvent != null && migrationEvent.IsApplicable(Metadata.GameVersion))
				{
					return migrationEvent.ToSpawnInstead;
				}
			}
		}
		return spawnableAsset;
		SpawnableAsset find(string name)
		{
			if ((bool)CatalogBehaviour.Main)
			{
				return CatalogBehaviour.Main.GetSpawnable(name);
			}
			return Resources.LoadAll<SpawnableAsset>("SpawnableAssets").FirstOrDefault((SpawnableAsset i) => i.name == name);
		}
	}

	public static Category FindCategory(string name)
	{
		if ((bool)CatalogBehaviour.Main)
		{
			return ModGlobals.MainCatalog.Categories.Concat(CatalogBehaviour.Main.ModdedCatalog).FirstOrDefault((Category c) => c.name == name || (c.HadPriorName && c.PriorName == name));
		}
		return ModGlobals.MainCatalog.Categories.FirstOrDefault((Category c) => c.name == name || (c.HadPriorName && c.PriorName == name));
	}

	public static Sprite LoadSprite(string path, float scale = 1f, bool pixelated = true)
	{
		if (!metaDataIsValid)
		{
			throw new InvalidOperationException("Invalid use of path-dependent function! LoadSprite must be called in the AfterSpawn, Main, or OnLoad function.");
		}
		string text = Path.Combine(Metadata.MetaLocation, path);
		if (ModResourceCache.TryGet<Sprite>(text, out var obj))
		{
			return obj;
		}
		if (!File.Exists(text))
		{
			Debug.LogErrorFormat("{0}: Failed to load sprite at \"{1}\"", Metadata.Name, path);
			return null;
		}
		Sprite sprite = Utils.LoadSprite(text, (!pixelated) ? FilterMode.Bilinear : FilterMode.Point, scale * 35f, markNonReadable: false);
		ModResourceCache.Cache(text, sprite);
		return sprite;
	}

	public static Texture2D LoadTexture(string path, bool pixelated = true)
	{
		if (!metaDataIsValid)
		{
			throw new InvalidOperationException("Invalid use of path-dependent function! LoadTexture must be called in the AfterSpawn, Main, or OnLoad function.");
		}
		string text = Path.Combine(Metadata.MetaLocation, path);
		if (ModResourceCache.TryGet<Texture2D>(text, out var obj))
		{
			return obj;
		}
		if (!File.Exists(text))
		{
			Debug.LogErrorFormat("{0}: Failed to load texture at \"{1}\"", Metadata.Name, path);
			return null;
		}
		Texture2D texture2D = Utils.LoadTexture(text, (!pixelated) ? FilterMode.Bilinear : FilterMode.Point, markNonReadable: false);
		ModResourceCache.Cache(text, texture2D);
		return texture2D;
	}

	public static AudioClip LoadSound(string path)
	{
		if (!metaDataIsValid)
		{
			throw new InvalidOperationException("Invalid use of path-dependent function! LoadSound must be called in the AfterSpawn, Main, or OnLoad function.");
		}
		string text = Path.Combine(Metadata.MetaLocation, path);
		if (ModResourceCache.TryGet<AudioClip>(text, out var obj))
		{
			return obj;
		}
		AudioClip audioClip = Utils.FileToAudioClip(text);
		ModResourceCache.Cache(text, audioClip);
		return audioClip;
	}

	public static Cartridge FindCartridge(string name)
	{
		if (name == "30-06 Springfield")
		{
			name = "30-06";
		}
		Cartridge cartridge = Resources.Load<Cartridge>("Calibers/" + name);
		if (cartridge == null)
		{
			return null;
		}
		return UnityEngine.Object.Instantiate(cartridge);
	}

	public static PhysicalProperties FindPhysicalProperties(string name)
	{
		PhysicalProperties physicalProperties = Resources.Load<PhysicalProperties>("PhysicalProperties/" + name);
		if (physicalProperties == null)
		{
			return null;
		}
		return UnityEngine.Object.Instantiate(physicalProperties);
	}

	public static Material FindMaterial(string name)
	{
		if (ModGlobals.LoadedMaterials.TryGetValue(name, out var value))
		{
			return value;
		}
		return null;
	}

	public static GameObject CreateParticleEffect(string name, Vector2 position)
	{
		if (ModGlobals.LoadedParticleEffects.TryGetValue(name, out var value))
		{
			if (value.CompareTag("Poolable"))
			{
				return PoolGenerator.Instance.RequestPrefab(value, position);
			}
			return UnityEngine.Object.Instantiate(value, position, Quaternion.identity);
		}
		return null;
	}

	public static void Register(Modification modification)
	{
		if (!metaDataIsValid)
		{
			throw new InvalidOperationException("Item modifications can only be registered in the AfterSpawn, Main, or OnLoad function.");
		}
		ModificationManager.Modifications.Add(modification, Metadata);
		Debug.Log(modification.NameOverride + " registered as item modification");
	}

	public static void Register<T>() where T : MonoBehaviour
	{
		ModificationManager.BackgroundScripts.Add(typeof(T));
		Debug.Log(typeof(T).Name + " registered as background behaviour");
	}

	public static void RegisterTool<T>(string name, string description, Sprite icon, string parent = null) where T : ToolBehaviour
	{
		RegisterCustomTool<T>(name, description, icon, parent, ToolLibrary.Instance.Tools);
		Debug.Log(typeof(T).Name + " registered as tool");
	}

	public static void RegisterPower<T>(string name, string description, Sprite icon, string parent = null) where T : ToolBehaviour
	{
		RegisterCustomTool<T>(name, description, icon, parent, ToolLibrary.Instance.Powers);
		Debug.Log(typeof(T).Name + " registered as power");
	}

	public static void RegisterInput(string name, string uniqueId, KeyCode defaultKey)
	{
		if (!metaDataIsValid)
		{
			throw new InvalidOperationException("Inputs can only be registered in the AfterSpawn, Main, or OnLoad function.");
		}
		if (!InputSystem.Modded.ContainsKey(uniqueId))
		{
			int hashCode = metadata.GetHashCode();
			if (registeredInputsByMod.TryGetValue(hashCode, out var value))
			{
				value.Add(uniqueId);
			}
			else
			{
				registeredInputsByMod.Add(hashCode, new HashSet<string> { uniqueId });
			}
			InputSystem.Modded.Add(uniqueId, new InputAction
			{
				Name = name,
				Category = ActionCategory.Modded,
				Key = defaultKey,
				Universe = ActionUniverse.Game
			});
			Debug.Log("Registered input for " + name + ":" + uniqueId);
			ControlSchemeEditorBehaviour.ShouldReinitialise = true;
		}
		else
		{
			Debug.LogError("Could not register input for " + name + ":" + uniqueId + ", it is a duplicate");
		}
	}

	private static void RegisterCustomTool<T>(string name, string description, Sprite icon, string parent, List<ToolLibrary.Tool> coll) where T : ToolBehaviour
	{
		if (!ToolLibrary.Instance)
		{
			Debug.LogError("You can't register custom tools/powers before a map is loaded");
			return;
		}
		coll.Add(new ToolLibrary.Tool(name, description, icon, typeof(T).Name, parent));
		ToolLibrary.ModdedTypes.Add(typeof(T));
		ToolLibrary.Instance.BroadcastCollectionChange();
	}

	public static void RegisterCategory(string name, string description, Sprite icon)
	{
		if (!CatalogBehaviour.Main)
		{
			Debug.LogError("You can't register custom categories before a map is loaded");
			return;
		}
		if (CatalogBehaviour.Main.ModdedCatalog.Any((Category u) => u.name == name))
		{
			Debug.LogError("You can't regiser custom categories with the same name");
			return;
		}
		Category category = ScriptableObject.CreateInstance<Category>();
		category.name = name;
		category.Description = description;
		category.Icon = icon;
		CatalogBehaviour.Main.ModdedCatalog.Add(category);
	}

	public static void Notify(object message)
	{
		NotificationControllerBehaviour.Show(message.ToString());
	}

	public static GameObject CreatePhysicalObject(string name, Sprite sprite)
	{
		GameObject gameObject = new GameObject(name, typeof(SpriteRenderer), typeof(AudioSourceTimeScaleBehaviour), typeof(Optout));
		gameObject.layer = LayerMask.NameToLayer("Objects");
		gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
		gameObject.AddComponent<BoxCollider2D>();
		gameObject.AddComponent<Rigidbody2D>();
		PhysicalBehaviour physicalBehaviour = gameObject.AddComponent<PhysicalBehaviour>();
		physicalBehaviour.Properties = FindPhysicalProperties("Metal");
		physicalBehaviour.SpawnSpawnParticles = false;
		gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
		physicalBehaviour.OverrideShotSounds = Array.Empty<AudioClip>();
		physicalBehaviour.OverrideImpactSounds = Array.Empty<AudioClip>();
		return gameObject;
	}

	public static LightSprite CreateLight(Transform parent, Color color, float radius = 5f, float brightness = 1.5f)
	{
		LightSprite component = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/ModLightPrefab"), parent).GetComponent<LightSprite>();
		component.transform.localPosition = Vector3.zero;
		component.Color = color;
		component.Radius = radius;
		component.Brightness = brightness;
		return component;
	}

	public static void KeepExtraObjects()
	{
		if (metaDataIsValid)
		{
			CatalogBehaviour.ShouldRemoveToRemoveWhenModded = false;
			return;
		}
		Debug.LogWarningFormat("You cannot call {0} outside the AfterSpawn delegate.", "KeepExtraObjects");
	}

	public static void RegisterLiquid(string id, Liquid liquidInstance)
	{
		if (Global.allowedToRegisterExternalLiquids)
		{
			Liquid.Register(id, liquidInstance);
			return;
		}
		throw new InvalidOperationException("You can only register a liquid after or during map load.");
	}

	public static void DeleteJSON(in string path)
	{
		if (!path.EndsWith(".json"))
		{
			throw new ArgumentException("path argument has to end with .json");
		}
		string fullPath = Path.GetFullPath(Path.Combine(Path.GetFullPath("Mods"), path));
		if (!fullPath.StartsWith(Path.GetFullPath("Mods")))
		{
			throw new ArgumentException("given path '" + fullPath + "' is not inside the game mod directory");
		}
		File.Delete(fullPath);
	}

	public static void SerialiseJSON(in object obj, in string path)
	{
		if (!path.EndsWith(".json"))
		{
			throw new ArgumentException("path argument has to end with .json");
		}
		if (obj == null)
		{
			throw new ArgumentNullException("obj cannot be null");
		}
		string fullPath = Path.GetFullPath(Path.Combine(Path.GetFullPath("Mods"), path));
		if (!fullPath.StartsWith(Path.GetFullPath("Mods")))
		{
			throw new ArgumentException("given path '" + fullPath + "' is not inside the game mod directory");
		}
		string contents = JsonConvert.SerializeObject(obj, modJsonSerializerSettings);
		File.WriteAllText(fullPath, contents);
	}

	public static T DeserialiseJSON<T>(in string path)
	{
		if (!path.EndsWith(".json"))
		{
			throw new ArgumentException("path argument has to end with .json");
		}
		string fullPath = Path.GetFullPath(Path.Combine(Path.GetFullPath("Mods"), path));
		if (!fullPath.StartsWith(Path.GetFullPath("Mods")))
		{
			throw new ArgumentException("given path '" + fullPath + "' is not inside the game mod directory");
		}
		return JsonConvert.DeserializeObject<T>(File.ReadAllText(fullPath));
	}
}

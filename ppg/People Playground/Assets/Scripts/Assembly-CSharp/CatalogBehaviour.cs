using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatalogBehaviour : MonoBehaviour
{
	public static List<GameObject> SpawnedGameObjects = new List<GameObject>();

	public TextMeshProUGUI TooltipText;

	public CatalogData Catalog;

	public Category LocalCategory;

	public List<Category> ModdedCatalog = new List<Category>();

	public bool AutomaticallyPopulateCatalog;

	internal Dictionary<SpawnableAsset, Action<GameObject>> modActionByAsset = new Dictionary<SpawnableAsset, Action<GameObject>>();

	private HashSet<SpawnableAsset> spawnableSet = new HashSet<SpawnableAsset>();

	private Dictionary<string, SpawnableAsset> spawnableByName = new Dictionary<string, SpawnableAsset>();

	private Dictionary<string, SpawnableAsset> spawnableByPriorName = new Dictionary<string, SpawnableAsset>();

	internal Dictionary<SpawnableAsset, ModMetaData> spawnableByMod = new Dictionary<SpawnableAsset, ModMetaData>();

	[Space]
	public Transform ItemContainer;

	public Transform CategoryContainer;

	public GameObject ItemPanel;

	public ResizableUIElementHandle ResizableUIElementHandle;

	public static CatalogBehaviour Main;

	[Space]
	public GameObject CategoryButtonPrefab;

	public GameObject CustomDropDownPrefab;

	public GameObject CategoryContainerPrefab;

	public GameObject ItemButtonPrefab;

	public SpawnableAsset SelectedItem;

	[NonSerialized]
	public Contraption SelectedContraption;

	[NonSerialized]
	public ContraptionMetaData SelectedContraptionMetaData;

	public SpawnableOutlineBehaviour SpawnableOutline;

	private readonly List<ItemButtonBehaviour> items = new List<ItemButtonBehaviour>();

	private readonly List<ItemButtonBehaviour> localItems = new List<ItemButtonBehaviour>();

	private Category selectedCategory;

	private bool backgroundScriptsInstantiated;

	internal static bool ShouldRemoveToRemoveWhenModded = true;

	private float secondsSpawnKeyHeld;

	private const float secondsSpawnKeyHeldThreshold = 0.1f;

	private int prevCategoryCount = 9;

	public Category SelectedCategory
	{
		get
		{
			return selectedCategory;
		}
		private set
		{
			selectedCategory = value;
			foreach (ItemButtonBehaviour item in items)
			{
				if ((bool)item.Item)
				{
					item.gameObject.SetActive(item.Item.Category == selectedCategory);
				}
				else
				{
					item.gameObject.SetActive(selectedCategory == LocalCategory);
				}
			}
		}
	}

	internal int ExtraCategoryCount => Catalog.Categories.Length - prevCategoryCount + ModdedCatalog.Count;

	public event EventHandler SelectionChanged;

	private void Awake()
	{
		Main = this;
		ResizableUIElementHandle = GetComponentInChildren<ResizableUIElementHandle>();
	}

	private void Start()
	{
		LoadToyboxElementSize();
		ItemPersistence.Deserialise();
		if (AutomaticallyPopulateCatalog)
		{
			Populate();
		}
		CreateCategoryButtons();
		CreateItemButtons();
		SelectedCategory = Catalog.Categories.First();
	}

	private void Populate()
	{
		Catalog.Items = Resources.LoadAll<SpawnableAsset>("SpawnableAssets");
		spawnableByName.Clear();
		spawnableSet.Clear();
		spawnableByPriorName.Clear();
		for (int i = 0; i < Catalog.Items.Length; i++)
		{
			SpawnableAsset spawnableAsset = Catalog.Items[i];
			spawnableByName.Add(spawnableAsset.name, spawnableAsset);
			spawnableSet.Add(spawnableAsset);
			if (spawnableAsset.HasPriorName)
			{
				if (string.IsNullOrWhiteSpace(spawnableAsset.PriorName))
				{
					throw new Exception(spawnableAsset.name + " has no prior name, but it indicates that it should have one");
				}
				spawnableByPriorName.Add(spawnableAsset.PriorName, spawnableAsset);
			}
		}
		ToolLibrary.ModdedTypes.Clear();
		modActionByAsset.Clear();
		ModificationManager.InvokeMain();
		ApplyModifications();
		InstantiateBackgroundScripts();
	}

	private void InstantiateBackgroundScripts()
	{
		if (backgroundScriptsInstantiated)
		{
			return;
		}
		foreach (Type backgroundScript in ModificationManager.BackgroundScripts)
		{
			new GameObject("Background script container for " + backgroundScript.Name, backgroundScript);
		}
		backgroundScriptsInstantiated = true;
	}

	internal void RegisterBackgroundScript<T>() where T : MonoBehaviour
	{
		ModificationManager.BackgroundScripts.Add(typeof(T));
	}

	private void ApplyModifications()
	{
		spawnableByMod.Clear();
		foreach (KeyValuePair<Modification, ModMetaData> modification in ModificationManager.Modifications)
		{
			Modification key = modification.Key;
			ModMetaData value = modification.Value;
			if (key == null || value == null)
			{
				continue;
			}
			if (key.AfterSpawn == null)
			{
				Debug.LogWarning((key.NameOverride ?? "Unknown custom item") + " has no AfterSpawn set");
				continue;
			}
			if (string.IsNullOrWhiteSpace(key.NameOverride))
			{
				Debug.LogWarning("Unknown custom item has no name set");
				continue;
			}
			SpawnableAsset originalItem = key.OriginalItem;
			if (!originalItem)
			{
				Debug.LogWarning((key.NameOverride ?? "Unknown custom item") + " has no original item set");
				continue;
			}
			try
			{
				ApplyModification(key, value, originalItem);
			}
			catch (Exception ex)
			{
				Debug.LogError("Error applying mod " + value.Name + "\n" + ex);
			}
		}
	}

	private void ApplyModification(Modification mod, ModMetaData meta, SpawnableAsset spawnable)
	{
		SpawnableAsset spawnableAsset = ScriptableObject.CreateInstance<SpawnableAsset>();
		spawnableAsset.RelevantModMetadata = meta;
		spawnableAsset.ViewSprite = (mod.ThumbnailOverride ? mod.ThumbnailOverride : mod.OriginalItem.ViewSprite);
		spawnableAsset.Category = mod.CategoryOverride;
		spawnableAsset.Description = mod.DescriptionOverride ?? spawnableAsset.Description;
		spawnableAsset.name = mod.NameOverride;
		spawnableAsset.NameToOrderBy = mod.NameToOrderByOverride ?? string.Empty;
		spawnableAsset.Prefab = spawnable.Prefab;
		if (spawnableByName.ContainsKey(spawnableAsset.name))
		{
			Debug.LogErrorFormat("Spawnable name conflict! There is already a spawnable with the name \"{0}\"", spawnableAsset.name);
		}
		spawnableByName.Add(spawnableAsset.name, spawnableAsset);
		spawnableSet.Add(spawnableAsset);
		modActionByAsset.Add(spawnableAsset, mod.AfterSpawn);
		spawnableByMod.Add(spawnableAsset, meta);
	}

	internal ModMetaData GetModOfSpawnable(SpawnableAsset spawnable)
	{
		if (spawnableByMod.TryGetValue(spawnable, out var value))
		{
			return value;
		}
		return null;
	}

	private void CreateCategoryButtons()
	{
		int num = 0;
		for (int i = 0; i < prevCategoryCount; i++)
		{
			Category category = Catalog.Categories[i];
			if (!category || !category.Icon)
			{
				Debug.LogErrorFormat("Attempt to create a non-existent category...");
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(CategoryButtonPrefab, CategoryContainer);
			gameObject.GetComponent<Image>().sprite = category.Icon;
			HasTooltipBehaviour hasTooltipBehaviour = gameObject.AddComponent<HasTooltipBehaviour>();
			hasTooltipBehaviour.TooltipText = TooltipText;
			hasTooltipBehaviour.Text = "<b>" + category.name + "</b>" + Environment.NewLine + category.Description;
			CategoryButtonBehaviour componentInChildren = gameObject.GetComponentInChildren<CategoryButtonBehaviour>();
			componentInChildren.CatalogBehaviour = this;
			componentInChildren.Category = category;
		}
		if (ModdedCatalog.Count <= 0 && Catalog.Categories.Length <= prevCategoryCount)
		{
			return;
		}
		CatalogDropdownBehaviour component = UnityEngine.Object.Instantiate(CustomDropDownPrefab, CategoryContainer).GetComponent<CatalogDropdownBehaviour>();
		Transform parent = CreateCategoryContainer(component);
		component.gameObject.GetComponent<HasTooltipBehaviour>().TooltipText = TooltipText;
		for (int j = prevCategoryCount; j < Catalog.Categories.Length; j++)
		{
			Category category2 = Catalog.Categories[j];
			if (!category2 || !category2.Icon)
			{
				Debug.LogErrorFormat("Attempt to create a non-existent category...");
				continue;
			}
			num++;
			if (num % 11 == 0)
			{
				parent = CreateCategoryContainer(component);
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(CategoryButtonPrefab, parent);
			gameObject2.GetComponent<Image>().sprite = category2.Icon;
			HasTooltipBehaviour hasTooltipBehaviour2 = gameObject2.AddComponent<HasTooltipBehaviour>();
			hasTooltipBehaviour2.TooltipText = TooltipText;
			hasTooltipBehaviour2.Text = "<b>" + category2.name + "</b>" + Environment.NewLine + category2.Description;
			CategoryButtonBehaviour componentInChildren2 = gameObject2.GetComponentInChildren<CategoryButtonBehaviour>();
			componentInChildren2.CatalogBehaviour = this;
			componentInChildren2.Category = category2;
		}
		foreach (Category item in ModdedCatalog)
		{
			if (!item || !item.Icon)
			{
				Debug.LogErrorFormat("Attempt to create a non-existent category...");
				continue;
			}
			num++;
			if (num % 11 == 0)
			{
				parent = CreateCategoryContainer(component);
			}
			GameObject gameObject3 = UnityEngine.Object.Instantiate(CategoryButtonPrefab, parent);
			gameObject3.GetComponent<Image>().sprite = item.Icon;
			HasTooltipBehaviour hasTooltipBehaviour3 = gameObject3.AddComponent<HasTooltipBehaviour>();
			hasTooltipBehaviour3.TooltipText = TooltipText;
			hasTooltipBehaviour3.Text = "<b>" + item.name + "</b>" + Environment.NewLine + item.Description;
			CategoryButtonBehaviour componentInChildren3 = gameObject3.GetComponentInChildren<CategoryButtonBehaviour>();
			componentInChildren3.CatalogBehaviour = this;
			componentInChildren3.Category = item;
		}
	}

	private Transform CreateCategoryContainer(CatalogDropdownBehaviour dropDown)
	{
		Transform transform = UnityEngine.Object.Instantiate(CategoryContainerPrefab, CategoryContainer.parent).transform;
		transform.SetSiblingIndex(transform.parent.childCount - 2);
		transform.gameObject.SetActive(value: false);
		dropDown.CategoryContainers.Add(transform.gameObject);
		return transform;
	}

	public void CreateItemButtons()
	{
		foreach (ItemButtonBehaviour item in items)
		{
			if ((bool)item)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		items.Clear();
		foreach (SpawnableAsset item2 in spawnableSet.OrderBy((SpawnableAsset i) => (i.NameToOrderBy.Length != 0) ? i.NameToOrderBy : i.name))
		{
			if (item2.VisibleInCatalog && (!item2.IsLocked || ItemPersistence.Has(item2.name)))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(ItemButtonPrefab, ItemContainer);
				ItemButtonBehaviour componentInChildren = gameObject.GetComponentInChildren<ItemButtonBehaviour>();
				componentInChildren.CatalogBehaviour = this;
				componentInChildren.Item = item2;
				HasTooltipBehaviour[] componentsInChildren = componentInChildren.GetComponentsInChildren<HasTooltipBehaviour>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].TooltipText = TooltipText;
				}
				gameObject.GetComponentInChildren<TextMeshProUGUI>().text = item2.name;
				items.Add(componentInChildren);
			}
		}
		IEnumerable<ContraptionMetaData> allContraptions = ContraptionSerialiser.GetAllContraptions();
		localItems.Clear();
		foreach (ContraptionMetaData item3 in allContraptions)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(ItemButtonPrefab, ItemContainer);
			ItemButtonBehaviour componentInChildren2 = gameObject2.GetComponentInChildren<ItemButtonBehaviour>();
			componentInChildren2.CatalogBehaviour = this;
			componentInChildren2.Local = item3;
			componentInChildren2.PublishedFileId = item3.PublishedFileID;
			HasTooltipBehaviour[] componentsInChildren = componentInChildren2.GetComponentsInChildren<HasTooltipBehaviour>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].TooltipText = TooltipText;
			}
			gameObject2.GetComponentInChildren<Image>().sprite = ContraptionSpriteStorage.GetFor(item3);
			gameObject2.GetComponentInChildren<TextMeshProUGUI>().text = item3.DisplayName;
			localItems.Add(componentInChildren2);
			items.Add(componentInChildren2);
		}
		SetCategory(SelectedCategory);
		if (!SelectedItem)
		{
			SetItem(spawnableSet.FirstOrDefault());
		}
		UISoundBehaviour.Refresh();
	}

	public void SetFilter(string filter)
	{
		filter = filter.ToLower();
		foreach (ItemButtonBehaviour item in items)
		{
			if ((bool)item && (bool)item.Item && item.Item.VisibleInCatalog)
			{
				item.gameObject.SetActive((item.Item.name + item.Item.Category).ToLower().Contains(filter));
			}
		}
		foreach (ItemButtonBehaviour localItem in localItems)
		{
			if ((bool)localItem && localItem.Local != null)
			{
				localItem.gameObject.SetActive(localItem.Local.DisplayName.ToLower().Contains(filter));
			}
		}
	}

	private void Update()
	{
		if (Global.main.GetPausedMenu() || DialogBox.IsAnyDialogboxOpen || Global.ActiveUiBlock || (!SelectedItem && SelectedContraption == null))
		{
			return;
		}
		if (SpawnableOutline.gameObject.activeInHierarchy)
		{
			SpawnableOutline.transform.position = Global.main.MousePosition;
		}
		if (InputSystem.Down("spawnRight") || InputSystem.Down("spawnLeft"))
		{
			secondsSpawnKeyHeld = 0f;
		}
		if (secondsSpawnKeyHeld < 0.1f && SelectedContraption != null)
		{
			if (InputSystem.Held("spawnRight"))
			{
				secondsSpawnKeyHeld += Time.unscaledDeltaTime;
				if (secondsSpawnKeyHeld >= 0.1f)
				{
					ShowContraptionOutline(flipped: false);
				}
			}
			else if (InputSystem.Held("spawnLeft"))
			{
				secondsSpawnKeyHeld += Time.unscaledDeltaTime;
				if (secondsSpawnKeyHeld >= 0.1f)
				{
					ShowContraptionOutline(flipped: true);
				}
			}
		}
		if (InputSystem.Up("spawnRight"))
		{
			HideContraptionOutline();
			if (SelectedContraption != null)
			{
				Spawn(SelectedContraption, flipped: false);
			}
			else
			{
				Spawn(SelectedItem, flipped: false);
			}
		}
		else if (InputSystem.Up("spawnLeft"))
		{
			HideContraptionOutline();
			if (SelectedContraption != null)
			{
				Spawn(SelectedContraption, flipped: true);
			}
			else
			{
				Spawn(SelectedItem, flipped: true);
			}
		}
	}

	private void HideContraptionOutline()
	{
		secondsSpawnKeyHeld = 0f;
		SpawnableOutline.gameObject.SetActive(value: false);
	}

	private void ShowContraptionOutline(bool flipped)
	{
		if (SelectedContraption == null)
		{
			return;
		}
		if (string.IsNullOrWhiteSpace(SelectedContraptionMetaData.PathToOutlineFile) || !File.Exists(SelectedContraptionMetaData.PathToOutlineFile))
		{
			Debug.LogWarningFormat("Path to outline file does not exist: {0}", SelectedContraptionMetaData.PathToOutlineFile);
			return;
		}
		try
		{
			ContraptionOutline outline = ContraptionOutlineSerialiser.LoadOutline(SelectedContraptionMetaData.PathToOutlineFile);
			SpawnableOutline.SetOutline(outline);
			SpawnableOutline.transform.localScale = new Vector3((!flipped) ? 1 : (-1), 1f, 1f);
			SpawnableOutline.gameObject.SetActive(value: true);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Failed to load contraption outline: {0}", ex);
		}
	}

	private void Spawn(SpawnableAsset e, bool flipped)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(SelectedItem.Prefab, Global.main.MousePosition, Quaternion.identity);
		gameObject.AddComponent<DeregisterBehaviour>();
		if (flipped)
		{
			Vector3 localScale = gameObject.transform.localScale;
			localScale.x *= -1f;
			gameObject.transform.localScale = localScale;
		}
		gameObject.AddComponent<TexturePackApplier>();
		gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
		gameObject.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = e;
		gameObject.name = e.name;
		SpawnedGameObjects.Add(gameObject);
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(gameObject));
		StatManager.IncrementInteger(StatManager.Stat.TOTAL_SPAWNED_ITEMS);
		NonSteamStatManager.Stats.Increment("TOTAL_SPAWNED_ITEMS");
		PerformMod(e, gameObject);
	}

	private void Spawn(Contraption e, bool flipped)
	{
		UndoControllerBehaviour.RegisterAction(new PasteLoadAction(ObjectStateConverter.Convert(e.ObjectStates, Global.main.MousePosition, flipped), SelectedContraptionMetaData.DisplayName));
	}

	public void SetCategory(Category category)
	{
		if (Catalog.Categories.Contains(category) || ModdedCatalog.Contains(category))
		{
			SelectedCategory = category;
			StartCoroutine(rebuildUi());
		}
		IEnumerator rebuildUi()
		{
			ScrollRect scroll = ItemContainer.GetComponentInParent<ScrollRect>();
			scroll.verticalNormalizedPosition = 1f;
			yield return new WaitForEndOfFrame();
			RectTransform rectTransform = scroll.GetComponent<RectTransform>();
			rectTransform.ForceUpdateRectTransforms();
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
			scroll.verticalNormalizedPosition = 1f;
			yield return new WaitForEndOfFrame();
			rectTransform.ForceUpdateRectTransforms();
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
			scroll.verticalNormalizedPosition = 1f;
		}
	}

	public void SetItem(SpawnableAsset item)
	{
		if (spawnableSet.Contains(item))
		{
			SelectedContraption = null;
			SelectedItem = item;
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, EventArgs.Empty);
			}
		}
	}

	public void SetContraption(Contraption contraption)
	{
		SelectedContraption = contraption;
		SelectedItem = null;
		if (this.SelectionChanged != null)
		{
			this.SelectionChanged(this, EventArgs.Empty);
		}
	}

	public SpawnableAsset GetSpawnable(string name)
	{
		if (spawnableByName.TryGetValue(name, out var value))
		{
			return value;
		}
		if (spawnableByPriorName.TryGetValue(name, out value))
		{
			return value;
		}
		return null;
	}

	public static void PerformMod(SpawnableAsset asset, GameObject instance)
	{
		ShouldRemoveToRemoveWhenModded = true;
		if (Main.modActionByAsset.TryGetValue(asset, out var value) && asset.RelevantModMetadata.Active)
		{
			ModAPI.Metadata = asset.RelevantModMetadata;
			ModAPI.metaDataIsValid = true;
			try
			{
				value(instance);
			}
			catch (Exception ex)
			{
				Debug.LogError(asset.RelevantModMetadata.Name + " caused an error\n" + ex);
			}
			ModAPI.metaDataIsValid = false;
			if (ShouldRemoveToRemoveWhenModded)
			{
				RemoveWhenModded[] componentsInChildren = instance.GetComponentsInChildren<RemoveWhenModded>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
				}
			}
		}
		ModAPI.InvokeItemSpawned(Main, new UserSpawnEventArgs(instance, asset));
	}

	public static void PerformMod(string assetName, GameObject instance)
	{
		SpawnableAsset spawnable = Main.GetSpawnable(assetName);
		if (!(spawnable == null))
		{
			PerformMod(spawnable, instance);
		}
	}

	public void SaveToyboxElementSize()
	{
		float x = ResizableUIElementHandle.ToResize.sizeDelta.x;
		if (Mathf.Abs(UserPreferenceManager.Current.ToyboxSizeOffset - x) > 1f)
		{
			UserPreferenceManager.Current.ToyboxSizeOffset = x;
			UserPreferenceManager.Save();
		}
	}

	public void LoadToyboxElementSize()
	{
		Vector2 sizeDelta = ResizableUIElementHandle.ToResize.sizeDelta;
		sizeDelta.x = UserPreferenceManager.Current.ToyboxSizeOffset;
		ResizableUIElementHandle.ToResize.sizeDelta = sizeDelta;
	}
}

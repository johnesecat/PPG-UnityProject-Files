using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModListBehaviour : MonoBehaviour
{
	public Transform Container;

	public GameObject EntryPrefab;

	public TextMeshProUGUI NoModsFound;

	public TextMeshProUGUI ModDescription;

	public string FallbackModDescription = "<alpha=#50><align=center>No mod selected";

	public Scrollbar Scrollbar;

	public ScrollRect ScrollRect;

	private readonly List<ModEntryBehaviour> modEntries = new List<ModEntryBehaviour>();

	public TMP_InputField Filter;

	private void Awake()
	{
		ModLoader.ModListChanged += ModLoader_ModListChanged;
		Generate();
		Filter.onValueChanged.AddListener(ApplySearchFilter);
	}

	private void ApplySearchFilter(string arg0)
	{
		string value = arg0.ToLower().Normalize().Trim();
		bool flag = string.IsNullOrWhiteSpace(value);
		foreach (ModEntryBehaviour modEntry in modEntries)
		{
			if (flag)
			{
				modEntry.gameObject.SetActive(value: true);
			}
			else if (modEntry.ModMeta == null)
			{
				modEntry.gameObject.SetActive(value: false);
			}
			else
			{
				modEntry.gameObject.SetActive(modEntry.ModMeta.Name.ToLower().Normalize().Contains(value) || modEntry.ModMeta.Author.ToLower().Normalize().Contains(value));
			}
		}
	}

	private void ModLoader_ModListChanged(object sender, EventArgs e)
	{
		Generate();
	}

	private void Start()
	{
		if (!Container)
		{
			Container = base.transform;
		}
		ResetView();
	}

	public void Generate()
	{
		List<ModMetaData> loadedMods = ModLoader.LoadedMods;
		modEntries.Clear();
		foreach (Transform item in Container)
		{
			if ((bool)item.GetComponent<ModEntryBehaviour>())
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		foreach (ModMetaData item2 in loadedMods)
		{
			if (item2 != null)
			{
				ModEntryBehaviour component = UnityEngine.Object.Instantiate(EntryPrefab, Container).GetComponent<ModEntryBehaviour>();
				component.ModMeta = item2;
				component.ModListBehaviour = this;
				component.UpdateUi();
				modEntries.Add(component);
			}
		}
		NoModsFound.enabled = loadedMods.Count == 0;
		ResetView();
	}

	internal void ResetView()
	{
		StartCoroutine(OnNextFrame(delegate
		{
			ScrollRect.verticalNormalizedPosition = 1f;
			Scrollbar.value = 1f;
			Container.localPosition = new Vector3(Container.localPosition.x, 0f);
			Container.GetComponent<RectTransform>().localPosition = new Vector3(Container.localPosition.x, 0f);
			Scrollbar.Rebuild(CanvasUpdate.Prelayout);
			Scrollbar.Rebuild(CanvasUpdate.Layout);
			Scrollbar.Rebuild(CanvasUpdate.PostLayout);
			ScrollRect.Rebuild(CanvasUpdate.Prelayout);
			ScrollRect.Rebuild(CanvasUpdate.Layout);
			ScrollRect.Rebuild(CanvasUpdate.PostLayout);
		}));
	}

	private void OnPageSelected()
	{
		ResetView();
	}

	private IEnumerator OnNextFrame(Action a)
	{
		yield return new WaitForEndOfFrame();
		a();
	}

	private void OnDestroy()
	{
		ModLoader.ModListChanged -= ModLoader_ModListChanged;
		Filter.onValueChanged.RemoveListener(ApplySearchFilter);
	}

	public void SetAllModStatus(bool active)
	{
		foreach (ModEntryBehaviour modEntry in modEntries)
		{
			if (!(modEntry == null))
			{
				ModLoader.SetModActive(modEntry.ModMeta, active);
				modEntry.UpdateUi();
			}
		}
	}
}

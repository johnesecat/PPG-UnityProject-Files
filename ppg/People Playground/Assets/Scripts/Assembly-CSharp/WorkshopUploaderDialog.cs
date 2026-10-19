using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopUploaderDialog : MonoBehaviour
{
	public ContraptionMetaData Contraption;

	[ReorderableList]
	public string[] AvailableTags;

	[Space]
	public Transform TagEntryContainer;

	public GameObject TagEntryPrefab;

	private void Start()
	{
		for (int i = 0; i < AvailableTags.Length; i++)
		{
			string tag = AvailableTags[i];
			GameObject obj = UnityEngine.Object.Instantiate(TagEntryPrefab, TagEntryContainer);
			obj.GetComponentInChildren<TextMeshProUGUI>().text = tag;
			obj.GetComponentInChildren<UnityEngine.UI.Toggle>().onValueChanged.AddListener(delegate(bool b)
			{
				if (b)
				{
					Select(tag);
				}
				else
				{
					Deselect(tag);
				}
			});
		}
	}

	public void Reset()
	{
		AllEntriesDo(delegate(UnityEngine.UI.Toggle item)
		{
			item.isOn = false;
		});
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		Global.main.AddUiBlocker();
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
		Global.main.RemoveUiBlocker();
	}

	public void Upload()
	{
		if (Contraption == null)
		{
			DialogBoxManager.Notification("Error\nAttempt to upload literally nothing: null contraption");
			Hide();
			return;
		}
		try
		{
			SteamWorkshopController.Main.PublishContraption(Contraption);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Failed to upload contraption: {0}", ex);
		}
		Hide();
	}

	public void Select(string tag)
	{
		if (Contraption == null)
		{
			DialogBoxManager.Notification("Error\nAttempt to manipulate nothing: null contraption");
			Hide();
		}
		else
		{
			Contraption.Tags.Add(tag);
		}
	}

	public void Deselect(string tag)
	{
		if (Contraption == null)
		{
			DialogBoxManager.Notification("Error\nAttempt to manipulate nothing: null contraption");
			Hide();
		}
		else
		{
			Contraption.Tags.RemoveAll((string c) => c == tag);
		}
	}

	private void OnDestroy()
	{
		AllEntriesDo(delegate(UnityEngine.UI.Toggle item)
		{
			item.onValueChanged.RemoveAllListeners();
		});
	}

	private void AllEntriesDo(Action<UnityEngine.UI.Toggle> action)
	{
		UnityEngine.UI.Toggle[] componentsInChildren = TagEntryContainer.GetComponentsInChildren<UnityEngine.UI.Toggle>();
		foreach (UnityEngine.UI.Toggle obj in componentsInChildren)
		{
			action(obj);
		}
	}
}

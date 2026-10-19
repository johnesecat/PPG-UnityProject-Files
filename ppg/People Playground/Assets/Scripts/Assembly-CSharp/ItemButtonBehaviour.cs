using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButtonBehaviour : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public SpawnableAsset Item;

	public ContraptionMetaData Local;

	public ulong PublishedFileId;

	public CatalogBehaviour CatalogBehaviour;

	public HasTooltipBehaviour tooltipBehaviour;

	public Transform Incompatible;

	public DeleteContraptionButtonBehaviour RemoveButton;

	public PublishButtonBehaviour PublishButton;

	public GameObject WorkshopButton;

	public GameObject InfoButton;

	private void Start()
	{
		Incompatible.gameObject.SetActive(value: false);
		tooltipBehaviour = GetComponent<HasTooltipBehaviour>();
		if ((bool)Item)
		{
			GetComponent<Image>().sprite = Item.ViewSprite;
			tooltipBehaviour.Text = "<b>" + Item.name + "</b>" + Environment.NewLine + Item.Description;
			RemoveButton.gameObject.SetActive(value: false);
			PublishButton.gameObject.SetActive(value: false);
			WorkshopButton.gameObject.SetActive(value: false);
			InfoButton.gameObject.SetActive(value: false);
		}
		else if (Local != null)
		{
			string text = "<b>" + Local.DisplayName + "</b>" + Environment.NewLine + "Saved in version " + Local.Version;
			if (!Local.IsCurrentVersion)
			{
				Incompatible.gameObject.SetActive(value: true);
				text += "\n<color=\"yellow\">This save is from a different version. This might cause issues.";
			}
			RemoveButton.gameObject.SetActive(value: true);
			PublishButton.gameObject.SetActive(PublishedFileId == 0);
			PublishButton.ContraptionMetaData = Local;
			WorkshopButton.SetActive(PublishedFileId != 0);
			RemoveButton.ContraptionMetaData = Local;
			RemoveButton.PublishedFileId = PublishedFileId;
			tooltipBehaviour.Text = text;
			InfoButton.gameObject.SetActive(value: true);
		}
	}

	[Obsolete]
	public void SetDragItem()
	{
		if ((bool)Item)
		{
			CatalogBehaviour.SetItem(Item);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (DialogBox.IsAnyDialogboxOpen)
		{
			return;
		}
		if ((bool)Item)
		{
			CatalogBehaviour.SetItem(Item);
		}
		else
		{
			if (Local == null)
			{
				return;
			}
			if (Local.RequiresMods)
			{
				List<string> list = new List<string>();
				for (int i = 0; i < Local.RequiredMods.Count; i++)
				{
					RequiredMod requiredMod = Local.RequiredMods[i];
					if (!ModLoader.LoadedMods.Any((ModMetaData m) => m.Active && m.GetUniqueName() == requiredMod.UniqueIdentity))
					{
						list.Add(requiredMod.Name);
					}
				}
				if (list.Any())
				{
					UISoundBehaviour.Main.Warning();
					DialogBoxManager.Dialog("Unfulfilled mod requirements:\n" + string.Join("\n", list), new DialogButton("Cancel", true), new DialogButton("Select anyway", true, load)).SetHeight(list.Count * 41 + 200);
					return;
				}
			}
			load();
		}
		void load()
		{
			Contraption contraption = null;
			try
			{
				contraption = ContraptionSerialiser.LoadContraption(Local);
			}
			catch (Exception message)
			{
				UnityEngine.Debug.LogError(message);
				UISoundBehaviour.Main.Error();
				DialogBoxManager.Notification(Local.IsCurrentVersion ? "This contraption is corrupted" : ("This contraption is incompatible with your current version\nTarget " + Local.Version + ", Current 1.27.5"));
				return;
			}
			CatalogBehaviour.SelectedContraptionMetaData = Local;
			CatalogBehaviour.SetContraption(contraption);
		}
	}

	public void ShowInfoDialog()
	{
		if (Local == null)
		{
			UnityEngine.Debug.LogError("Attempt to view information of a catalog item that isn't a contraption.");
			UISoundBehaviour.Main.Error();
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		string formattedByteString = Utils.GetFormattedByteString((ulong)Local.GetSizeOnDisk());
		stringBuilder.AppendLine("<b><size=110%>" + Local.DisplayName + "</size></b> <alpha=#5A><size=80%>for " + Local.Version + "\n" + formattedByteString + "<alpha=#FF><size=100%>");
		if (!Local.IsCurrentVersion)
		{
			stringBuilder.AppendLine("Outdated");
		}
		if (PublishedFileId == 0L)
		{
			stringBuilder.AppendLine("Local file");
		}
		else
		{
			stringBuilder.AppendLine("From the Steam Workshop");
		}
		if (Local.RequiresMods)
		{
			stringBuilder.AppendLine("<b>Required mods:</b>");
			foreach (RequiredMod item in Local.RequiredMods)
			{
				if (ModLoader.LoadedMods.Any((ModMetaData m) => m.Active && !m.HasErrors && m.GetUniqueName() == item.UniqueIdentity && (string.IsNullOrWhiteSpace(m.UGCIdentity) || m.UGCIdentity == item.WorkshopId)))
				{
					stringBuilder.Append("<color=#47e849>");
				}
				else
				{
					stringBuilder.Append("<color=#f42d2d>");
				}
				stringBuilder.AppendLine(" - " + item.Name);
				stringBuilder.Append("</color>");
			}
		}
		else
		{
			stringBuilder.AppendLine("No mods required");
		}
		string text = stringBuilder.ToString();
		DialogBox dialogBox = DialogBoxManager.Notification(text);
		dialogBox.Buttons = dialogBox.Buttons.Append(new DialogButton("Open in Explorer", false, delegate
		{
			Process.Start("explorer.exe", Path.GetDirectoryName(Local.PathToDataFile));
		})).ToArray();
		dialogBox.TextMesh.text = text;
		dialogBox.TextMesh.ForceMeshUpdate(ignoreActiveState: true, forceTextReparsing: true);
		Vector2 preferredValues = dialogBox.TextMesh.GetPreferredValues();
		dialogBox.SetWidth(Mathf.Clamp(preferredValues.x + 32f, 670f, Camera.main.pixelWidth - 150));
		dialogBox.SetHeight(Mathf.Clamp(preferredValues.y + 32f, 400f, Camera.main.pixelHeight - 50));
		dialogBox.TextMesh.alignment = TextAlignmentOptions.TopLeft;
	}

	public void OpenWorkshop()
	{
		Utils.OpenURL($"https://steamcommunity.com/sharedfiles/filedetails/?id={PublishedFileId}");
	}
}

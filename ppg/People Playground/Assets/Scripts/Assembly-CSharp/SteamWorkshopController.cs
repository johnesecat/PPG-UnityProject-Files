using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steamworks;
using Steamworks.Data;
using Steamworks.Ugc;
using UnityEngine;

public class SteamWorkshopController : MonoBehaviour, IProgress<float>
{
	public static SteamWorkshopController Main;

	private ContraptionMetaData currentlyUploading;

	private bool isBusy;

	private DialogBox currentProgressBox;

	private float currentProgress;

	private void Awake()
	{
		Main = this;
	}

	public void PublishContraption(ContraptionMetaData contraptionMetadata)
	{
		if (isBusy)
		{
			return;
		}
		ContraptionSerialiser.ConvertToDirectoryBased(contraptionMetadata);
		if (!IsContraptionValid(contraptionMetadata))
		{
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Dialog("Contraption directory malformed\nCan't upload to Workshop", new DialogButton("Close", true));
			return;
		}
		currentlyUploading = contraptionMetadata;
		if (string.IsNullOrEmpty(currentlyUploading.CreatorUGCIdentity))
		{
			StartCoroutine(StartUploadingRoutine());
			return;
		}
		UISoundBehaviour.Main.Warning();
		DialogBoxManager.Dialog("This item is marked as once uploaded. How do you want to continue?", new DialogButton("Update existing", true, delegate
		{
			StartCoroutine(StartUploadingRoutine(ulong.Parse(currentlyUploading.CreatorUGCIdentity)));
		}), new DialogButton("Upload as new", true, delegate
		{
			StartCoroutine(StartUploadingRoutine());
		}), new DialogButton("Cancel", true));
	}

	private IEnumerator StartUploadingRoutine(PublishedFileId? existingId = null)
	{
		isBusy = true;
		string absolutePath = Path.GetFullPath("Contraptions/");
		string previewFilePath = absolutePath + currentlyUploading.Name + "\\" + currentlyUploading.Name + ".png";
		if (Utils.IsFamilyShared())
		{
			UISoundBehaviour.Main.Error();
			Debug.LogWarning("Invalid Steam user for contraption upload");
			DialogBoxManager.Notification("Contraptions cannot be uploaded from family shared games.");
			isBusy = false;
			yield break;
		}
		try
		{
			Utils.AssertValidThumbnail(previewFilePath);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Invalid thumbnail: " + ex);
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification(ex.Message);
			isBusy = false;
			yield break;
		}
		string text = currentlyUploading.DisplayName;
		if (currentlyUploading.RequiresMods)
		{
			StringBuilder stringBuilder = new StringBuilder(text);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("[b]This contraption requires the following mods:[/b]");
			foreach (RequiredMod requiredMod in currentlyUploading.RequiredMods)
			{
				if (requiredMod.HasWorkshopId)
				{
					stringBuilder.AppendFormat("- [url=https://steamcommunity.com/sharedfiles/filedetails/?id={0}] {1} [/url]\n", requiredMod.WorkshopId, requiredMod.Name);
				}
				else
				{
					stringBuilder.AppendFormat("- {0}\n", requiredMod.Name);
				}
			}
			text = stringBuilder.ToString();
		}
		PublishResult? publishResult = null;
		Editor editor;
		if (existingId.HasValue)
		{
			Item? item = null;
			Task<Item?> getItemTask = Task.Run(async () => item = await Item.GetAsync(existingId.Value));
			yield return new WaitUntil(() => getItemTask.IsCompleted);
			if (!item.HasValue || !item.Value.Result.HasFlag(Result.OK))
			{
				Debug.LogWarning("Failed to retrieve original Workshop item... creating a new one");
				StartCoroutine(StartUploadingRoutine());
				yield break;
			}
			editor = item.Value.Edit();
			editor = editor.WithContent(absolutePath + currentlyUploading.Name).WithPreviewFile(previewFilePath);
		}
		else
		{
			editor = Editor.NewCommunityFile.WithTitle(currentlyUploading.DisplayName).WithDescription(text).WithPreviewFile(previewFilePath)
				.WithContent(absolutePath + currentlyUploading.Name)
				.WithPublicVisibility();
		}
		foreach (string item2 in currentlyUploading.Tags.Intersect(SteamTags.Tags).Append("Contraptions"))
		{
			editor = editor.WithTag(item2);
		}
		Task task = Task.Run(async delegate
		{
			publishResult = await editor.SubmitAsync(this);
		});
		if ((bool)currentProgressBox)
		{
			currentProgressBox.Close();
		}
		currentProgressBox = DialogBoxManager.Dialog(currentlyUploading.DisplayName);
		yield return new WaitUntil(() => task.IsCompleted);
		if (!publishResult.HasValue)
		{
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Dialog("Could not create workshop item\n<i>" + (task.IsFaulted ? task.Exception.InnerException.Message : "Unknown reason") + "</i>", new DialogButton("Close", true));
			isBusy = false;
			yield break;
		}
		if (!publishResult.Value.Result.HasFlag(Result.OK))
		{
			UISoundBehaviour.Main.Error();
			Debug.LogWarning("Failed to create Steam Workshop item: " + publishResult.Value.Result);
			DialogBoxManager.Dialog($"Could not create workshop item\n<i>{publishResult.Value.Result}</i>", new DialogButton("Close", true));
			isBusy = false;
			yield break;
		}
		if (publishResult.Value.NeedsWorkshopAgreement)
		{
			DialogBoxManager.Dialog("By submitting to the workshop, you must agree to the terms of service.", new DialogButton("View terms of service", true, delegate
			{
				OpenURL("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
			}), new DialogButton("Cancel", true));
			isBusy = false;
			yield break;
		}
		currentlyUploading.CreatorUGCIdentity = publishResult.Value.FileId.ToString();
		OpenURL($"steam://url/CommunityFilePage/{publishResult.Value.FileId}");
		File.WriteAllText(currentlyUploading.PathToMetadata, JsonConvert.SerializeObject(currentlyUploading));
		if ((bool)currentProgressBox)
		{
			currentProgressBox.Close();
		}
		isBusy = false;
	}

	private void OpenURL(string url)
	{
		if (SteamUtils.IsOverlayEnabled)
		{
			SteamFriends.OpenWebOverlay(url);
		}
		else
		{
			Application.OpenURL(url);
		}
	}

	private bool IsContraptionValid(ContraptionMetaData data)
	{
		string pathToMetadata = data.PathToMetadata;
		string pathToDataFile = data.PathToDataFile;
		string pathToThumbnail = data.PathToThumbnail;
		if (File.Exists(pathToMetadata) && File.Exists(pathToDataFile))
		{
			return File.Exists(pathToThumbnail);
		}
		return false;
	}

	public void Report(float value)
	{
		currentProgress = value;
	}

	private void Update()
	{
		if ((bool)currentProgressBox && currentlyUploading != null)
		{
			currentProgressBox.SetTitle($"{currentlyUploading.Name}\n\n{Mathf.RoundToInt(currentProgress * 100f)}% uploaded...");
		}
	}
}

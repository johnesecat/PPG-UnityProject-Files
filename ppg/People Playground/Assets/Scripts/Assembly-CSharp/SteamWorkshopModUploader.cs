using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using Steamworks.Ugc;
using UnityEngine;

public class SteamWorkshopModUploader : MonoBehaviour, IProgress<float>
{
	public static SteamWorkshopModUploader Main;

	private ModMetaData currentlyUploading;

	private bool isBusy;

	private DialogBox currentProgressBox;

	private float currentProgress;

	private string TemporaryDir = "tmp";

	private void Awake()
	{
		Main = this;
		try
		{
			if (Directory.Exists(TemporaryDir))
			{
				Directory.Delete(TemporaryDir, recursive: true);
			}
			Directory.CreateDirectory(TemporaryDir);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to create or delete temp folder: \"" + TemporaryDir + "\": " + ex);
		}
	}

	public void PublishMod(ModMetaData modMeta)
	{
		if (isBusy)
		{
			return;
		}
		if (modMeta.HasErrors)
		{
			throw new ArgumentException("Mod cannot compile, cannot upload");
		}
		if (!string.IsNullOrEmpty(modMeta.UGCIdentity))
		{
			throw new ArgumentException("This mod originates from the workshop already");
		}
		currentlyUploading = modMeta;
		if (string.IsNullOrEmpty(currentlyUploading.CreatorUGCIdentity))
		{
			StartCoroutine(PublishRoutine());
			return;
		}
		UISoundBehaviour.Main.Warning();
		DialogBoxManager.Dialog("This item is marked as once uploaded. How do you want to continue?", new DialogButton("Update existing", true, delegate
		{
			StartCoroutine(PublishRoutine(ulong.Parse(modMeta.CreatorUGCIdentity)));
		}), new DialogButton("Upload as new", true, delegate
		{
			StartCoroutine(PublishRoutine());
		}), new DialogButton("Cancel", true));
	}

	private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
	{
		Directory.CreateDirectory(target.FullName, null);
		foreach (FileInfo item in source.EnumerateFiles())
		{
			Console.WriteLine("Copying {0}\\{1}", target.FullName, item.Name);
			item.CopyTo(Path.Combine(target.FullName, item.Name), overwrite: true);
		}
		foreach (DirectoryInfo item2 in source.EnumerateDirectories())
		{
			DirectoryInfo target2 = target.CreateSubdirectory(item2.Name);
			CopyAll(item2, target2);
		}
	}

	private DirectoryInfo CopyModToTmpFolder(ModMetaData modMeta)
	{
		string path = TemporaryDir + Path.DirectorySeparatorChar + Utils.GetMD5AsString(modMeta.GetUniqueName()).Substring(0, 8);
		if (Directory.Exists(path))
		{
			Directory.Delete(path, recursive: true);
		}
		DirectoryInfo source = new DirectoryInfo(modMeta.MetaLocation);
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		Debug.Log($"Created tmp directory \"{directoryInfo}\" for workshop mod copy");
		CopyAll(source, directoryInfo);
		return directoryInfo;
	}

	private IEnumerator PublishRoutine(PublishedFileId? existingId = null)
	{
		isBusy = true;
		DirectoryInfo newDirectory = CopyModToTmpFolder(currentlyUploading);
		ModifyModInDirectoryForUpload(newDirectory, currentlyUploading.Author ?? SteamClient.Name);
		string contentLocation = newDirectory.FullName;
		string thumbnailLocation = ToAbsolutePath(Path.Combine(contentLocation, currentlyUploading.ThumbnailPath));
		PublishResult? publishResult = null;
		if (new FileInfo(thumbnailLocation).Length > 1000000)
		{
			UISoundBehaviour.Main.Error();
			Debug.LogWarning("Invalid thumbnail");
			DialogBoxManager.Dialog("Mod thumbnail exceeds 1 MB. Please compress or resize the image.", new DialogButton("Close", true));
			isBusy = false;
			yield break;
		}
		Editor editor;
		if (existingId.HasValue)
		{
			Item? item = null;
			Task<Item?> getItemTask = Task.Run(async () => item = await Item.GetAsync(existingId.Value));
			yield return new WaitUntil(() => getItemTask.IsCompleted);
			if (!item.HasValue || !item.Value.Result.HasFlag(Result.OK))
			{
				Debug.LogWarning("Failed to retrieve original Workshop item... creating a new one");
				StartCoroutine(PublishRoutine());
				yield break;
			}
			editor = item.Value.Edit();
			editor = editor.WithContent(contentLocation).WithPreviewFile(thumbnailLocation);
		}
		else
		{
			editor = Editor.NewCommunityFile.WithTitle(currentlyUploading.Name).WithDescription(currentlyUploading.Description).WithPreviewFile(thumbnailLocation)
				.WithContent(contentLocation)
				.WithPublicVisibility();
			foreach (string item2 in currentlyUploading.GetTagsForSteam())
			{
				editor = editor.WithTag(item2);
			}
		}
		Task<PublishResult?> task = Task.Run(async () => publishResult = await editor.SubmitAsync(this));
		if ((bool)currentProgressBox)
		{
			currentProgressBox.Close();
		}
		currentProgressBox = DialogBoxManager.Dialog(currentlyUploading.Name);
		yield return new WaitUntil(() => task.IsCompleted);
		if (!publishResult.HasValue)
		{
			if (task.IsFaulted)
			{
				Debug.LogError(task.Exception.InnerException);
			}
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Dialog("Could not create workshop item\n<i>" + (task.IsFaulted ? task.Exception.InnerException.Message : "Unknown reason") + "</i>", new DialogButton("Close", true));
			isBusy = false;
			task.Dispose();
			yield break;
		}
		task.Dispose();
		int num = 4;
		while (num > 0)
		{
			try
			{
				newDirectory.Delete(recursive: true);
			}
			catch (Exception)
			{
				num--;
				Debug.LogWarning("Failed to delete directory " + newDirectory.FullName + " while uploading mod... " + ((num > 0) ? "retrying..." : "giving up"));
				if (num > 0)
				{
					Thread.Sleep(1500);
				}
				continue;
			}
			break;
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
			DialogBoxManager.Dialog("By submitting to the workshop, you must agree to the terms of service.", new DialogButton("View terms of service", false, delegate
			{
				OpenURL("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
			}), new DialogButton("Cancel", true));
			isBusy = false;
			yield break;
		}
		currentlyUploading.CreatorUGCIdentity = publishResult.Value.FileId.ToString();
		ModLoader.UpdateJSON(currentlyUploading);
		OpenURL($"steam://url/CommunityFilePage/{publishResult.Value.FileId}");
		if ((bool)currentProgressBox)
		{
			currentProgressBox.Close();
		}
		isBusy = false;
	}

	private void ModifyModInDirectoryForUpload(DirectoryInfo dir, string author)
	{
		author = author.Normalize();
		string value = Environment.NewLine + "// Originally uploaded by '" + author + "'. Do not reupload without their explicit permission.";
		foreach (FileInfo item in dir.EnumerateFiles("*.cs"))
		{
			using (StreamWriter streamWriter = File.AppendText(item.FullName))
			{
				streamWriter.WriteLine(value);
				streamWriter.Close();
			}
		}
		DoNotReuploadSign.Mark(ToAbsolutePath(Path.Combine(dir.FullName, currentlyUploading.ThumbnailPath)));
	}

	private static string ToAbsolutePath(string relativePath)
	{
		return Path.Combine(Environment.CurrentDirectory, relativePath);
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

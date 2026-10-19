using System;
using System.IO;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Ugc;
using UnityEngine;

public class DeleteContraptionButtonBehaviour : MonoBehaviour
{
	public ContraptionMetaData ContraptionMetaData;

	public ulong PublishedFileId;

	public void CreateDialogBoxAndRemove()
	{
		DialogBoxManager.Dialog(((PublishedFileId == 0L) ? "Are you sure you want to permanently remove" : "Are you sure you want to unsubscribe from") + " " + ContraptionMetaData.DisplayName + "?", new DialogButton("Yes", true, RemoveContraption), new DialogButton("No", true));
	}

	public void RemoveContraption()
	{
		if (PublishedFileId != 0L)
		{
			Task.Run(async delegate
			{
				Item? item = await Item.GetAsync(PublishedFileId);
				if (item.HasValue && item.Value.Result == Result.OK && item.Value.IsInstalled)
				{
					await item.Value.Unsubscribe();
				}
			});
		}
		string pathToMetadata = ContraptionMetaData.PathToMetadata;
		string pathToDataFile = ContraptionMetaData.PathToDataFile;
		string pathToThumbnail = ContraptionMetaData.PathToThumbnail;
		string pathToOutlineFile = ContraptionMetaData.PathToOutlineFile;
		if (!File.Exists(pathToMetadata) || !File.Exists(pathToDataFile))
		{
			Debug.LogWarning("Attempt to delete contraption that doesn't exist: " + ContraptionMetaData.Name);
		}
		attemptDelete(pathToMetadata);
		attemptDelete(pathToDataFile);
		attemptDelete(pathToThumbnail);
		attemptDelete(pathToOutlineFile);
		CatalogBehaviour.Main.CreateItemButtons();
		void attemptDelete(string path)
		{
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
					DialogBoxManager.Notification("Unable to delete\n" + path);
					return;
				}
				Debug.Log("Deleted " + path);
			}
		}
	}
}

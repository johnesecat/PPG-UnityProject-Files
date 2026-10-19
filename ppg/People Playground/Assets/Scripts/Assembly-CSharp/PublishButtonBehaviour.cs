using UnityEngine;

public class PublishButtonBehaviour : MonoBehaviour
{
	public ContraptionMetaData ContraptionMetaData;

	public void Publish()
	{
		if (!SteamworksInitialiser.IsInitialised)
		{
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification("<b>Steam is not initialised</b>\nAre you sure Steam is open? Try restarting the game and the Steam client.");
			return;
		}
		DialogBoxManager.Dialog("Do you want to upload\n<b>" + ContraptionMetaData.DisplayName + "</b>\nto the Workshop?", new DialogButton("Yes", true, delegate
		{
			WorkshopUploaderDialog workshopUploaderDialog = Global.main.WorkshopUploaderDialog;
			workshopUploaderDialog.Reset();
			workshopUploaderDialog.Contraption = ContraptionMetaData;
			workshopUploaderDialog.Show();
		}), new DialogButton("No", true));
	}
}

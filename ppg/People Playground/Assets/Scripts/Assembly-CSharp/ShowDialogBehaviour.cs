using NaughtyAttributes;
using UnityEngine;

public class ShowDialogBehaviour : MonoBehaviour
{
	public bool ShouldShowOnStart;

	public OptionalFloat Width;

	public OptionalFloat Height;

	public string Title = "Untitled";

	[ResizableTextArea]
	public string Message = "Allan please add details";

	public bool PlayErrorSound;

	public bool PlayWarningSound;

	private void Start()
	{
		if (ShouldShowOnStart)
		{
			ShowDialog();
		}
	}

	public void ShowDialog()
	{
		if (base.gameObject.activeInHierarchy)
		{
			string text = string.Empty;
			if (!string.IsNullOrWhiteSpace(Title))
			{
				text = "<b>" + Title + "</b>\n\n";
			}
			if (!string.IsNullOrWhiteSpace(Message))
			{
				text += Message;
			}
			DialogBox dialogBox = DialogBoxManager.Notification(text);
			if (Width.Active)
			{
				dialogBox.SetWidth(Width.Value);
			}
			if (Height.Active)
			{
				dialogBox.SetHeight(Height.Value);
			}
			if (PlayErrorSound)
			{
				UISoundBehaviour.Main.Error();
			}
			if (PlayWarningSound)
			{
				UISoundBehaviour.Main.Warning();
			}
		}
	}
}

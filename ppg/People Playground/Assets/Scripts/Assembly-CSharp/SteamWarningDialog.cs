using NaughtyAttributes;
using UnityEngine;

public class SteamWarningDialog : MonoBehaviour
{
	private static bool alreadyShown;

	public float Delay = 1f;

	public OptionalFloat Width;

	public OptionalFloat Height;

	public string Title = "Untitled";

	[ResizableTextArea]
	public string Message = "Allan please add details";

	private float t;

	private void Start()
	{
		t = 0f;
		if (alreadyShown)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		t += Time.unscaledDeltaTime;
		if (!(t > Delay))
		{
			return;
		}
		if (!SteamworksInitialiser.IsInitialised)
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
			UISoundBehaviour.Main.Warning();
			DialogBox dialogBox = DialogBoxManager.Notification(text);
			if (Width.Active)
			{
				dialogBox.SetWidth(Width.Value);
			}
			if (Height.Active)
			{
				dialogBox.SetHeight(Height.Value);
			}
			alreadyShown = true;
		}
		base.enabled = false;
	}
}

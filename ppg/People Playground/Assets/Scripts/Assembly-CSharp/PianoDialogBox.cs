using System;
using UnityEngine;

public class PianoDialogBox : MonoBehaviour
{
	public static BellClip SelectedBellClip;

	private static Action<BellClip> currentCallback;

	public static PianoDialogBox Instance { get; private set; }

	private void Start()
	{
		Instance = this;
		Hide();
	}

	public static void Show(BellClip startClip, Action<BellClip> onApply)
	{
		Instance.gameObject.SetActive(value: true);
		currentCallback = onApply;
		SelectedBellClip = startClip;
	}

	public static void Hide()
	{
		Instance.gameObject.SetActive(value: false);
	}

	public void ApplyClicked()
	{
		currentCallback?.Invoke(SelectedBellClip);
		Hide();
	}
}

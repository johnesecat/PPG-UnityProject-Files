using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiPianoKeyBehaviour : MonoBehaviour
{
	public AudioSource Source;

	public BellClip BellClip;

	private Button button;

	private void Start()
	{
		button = GetComponentInChildren<Button>();
		button.onClick.AddListener(delegate
		{
			Source.PlayOneShot(BellClip.Clip);
			PianoDialogBox.SelectedBellClip = BellClip;
		});
		GetComponentInChildren<TextMeshProUGUI>().text = BellClip.GetDisplayName();
	}

	private void Update()
	{
		if ((bool)button)
		{
			button.targetGraphic.color = ((PianoDialogBox.SelectedBellClip == BellClip) ? Color.green : Color.white);
		}
	}
}

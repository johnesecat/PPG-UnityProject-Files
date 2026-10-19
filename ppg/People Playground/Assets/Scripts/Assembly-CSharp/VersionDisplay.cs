using System;
using TMPro;
using UnityEngine;

public class VersionDisplay : MonoBehaviour
{
	public TextMeshProUGUI Text;

	private void Start()
	{
		string text = ((IntPtr.Size == 8) ? "64" : "32");
		Text.text = "running game version <b>1.27.5 (" + text + " bit)</b>\nusing Unity version <b>" + Application.unityVersion + "</b>";
	}
}

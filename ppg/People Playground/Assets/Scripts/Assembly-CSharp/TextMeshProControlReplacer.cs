using System;
using TMPro;
using UnityEngine;

public class TextMeshProControlReplacer : MonoBehaviour
{
	[Serializable]
	public struct ControlSubstituteInstruction
	{
		public string ToReplace;

		public string ControlCodeName;
	}

	public TMP_Text Text;

	public ControlSubstituteInstruction[] ToReplace;

	private void Start()
	{
		string text = Text.text;
		for (int i = 0; i < ToReplace.Length; i++)
		{
			ControlSubstituteInstruction controlSubstituteInstruction = ToReplace[i];
			text = text.Replace(controlSubstituteInstruction.ToReplace, InputSystem.GetDisplayText(controlSubstituteInstruction.ControlCodeName));
		}
		Text.text = text;
	}
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingButtonBehaviour : MonoBehaviour
{
	public Button Button;

	public TextMeshProUGUI Label;

	[NonSerialized]
	public object Value;

	[NonSerialized]
	public string Key;

	public void SetChecked()
	{
		Label.color = (UserPreferenceManager.Current.GetByName(Key).Equals(Value) ? Color.green : Color.white);
	}
}

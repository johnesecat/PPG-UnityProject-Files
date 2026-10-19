using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderUserPreferenceBehaviour : MonoBehaviour
{
	public Slider Slider;

	public TextMeshProUGUI Display;

	[ShowIf("HasDisplay")]
	public string Formatting = "{x}";

	public bool MultiplyDisplayByHundred;

	public string Key;

	public bool ShowInteger;

	[HideIf("ShowInteger")]
	public int MaxDisplayLength = 4;

	public bool IsInteger;

	private void Awake()
	{
		if (!Slider)
		{
			Slider = GetComponent<Slider>();
		}
		Slider.onValueChanged.AddListener(Save);
		SetDisplayText();
	}

	private void Save(float value)
	{
		if (IsInteger)
		{
			UserPreferenceManager.Current.SetByName(Key, Mathf.RoundToInt(value));
		}
		else
		{
			UserPreferenceManager.Current.SetByName(Key, value);
		}
		SetDisplayText();
		UnityEngine.Object.FindObjectOfType<MixerControllerBehaviour>().Sync();
	}

	private void SetDisplayText()
	{
		if (HasDisplay())
		{
			float num = GetValueFromConfig();
			if (MultiplyDisplayByHundred)
			{
				num *= 100f;
			}
			if (ShowInteger)
			{
				num = Mathf.RoundToInt(num);
			}
			string text = num.ToString();
			Display.text = Formatting.Replace("{x}", text.Substring(0, Math.Min(MaxDisplayLength, text.Length)));
		}
	}

	private float GetValueFromConfig()
	{
		if (!IsInteger)
		{
			return UserPreferenceManager.Current.GetByName<float>(Key);
		}
		return UserPreferenceManager.Current.GetByName<int>(Key);
	}

	public bool HasDisplay()
	{
		return Display;
	}

	private void Start()
	{
		SetFromRemote();
	}

	private void SetFromRemote()
	{
		Slider.value = GetValueFromConfig();
		SetDisplayText();
	}
}

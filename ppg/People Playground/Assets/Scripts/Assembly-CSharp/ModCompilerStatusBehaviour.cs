using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Obsolete]
public class ModCompilerStatusBehaviour : MonoBehaviour
{
	public enum State
	{
		Compiling,
		Reading,
		Loading
	}

	public Sprite DefaultModSprite;

	public TextMeshProUGUI TextMeshPro;

	public Image Thumbnail;

	private string modName = string.Empty;

	private State state;

	public static ModCompilerStatusBehaviour Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		Hide();
	}

	public void SetMod(ModMetaData mod)
	{
		modName = mod.Name;
		TextMeshPro.text = modName;
		Sprite thumbnail = ModLoader.GetThumbnail(mod);
		if ((bool)thumbnail && thumbnail != null)
		{
			Thumbnail.sprite = thumbnail;
		}
		else
		{
			Thumbnail.sprite = DefaultModSprite;
		}
	}

	private void Update()
	{
		TextMeshPro.text = $"<color=#ccc>{state}</color>\n{modName}";
	}

	public void SetDisplayState(State state)
	{
		this.state = state;
	}

	public static void Show(ModMetaData mod)
	{
		if ((bool)Instance)
		{
			Instance.SetMod(mod);
			Instance.gameObject.SetActive(value: true);
		}
	}

	public static void Hide()
	{
		if ((bool)Instance)
		{
			Instance.gameObject.SetActive(value: false);
		}
	}
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundItemLoaderStatusBehaviour : MonoBehaviour
{
	public Sprite DefaultThumbnailSprite;

	[Space]
	public TextMeshProUGUI TitleMesh;

	public TextMeshProUGUI StateMesh;

	public Image ThumbnailImage;

	private volatile string displayState;

	private volatile string displayName;

	public static BackgroundItemLoaderStatusBehaviour Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		Hide();
	}

	public void SetDisplayData(string name, Sprite thumbnail)
	{
		displayName = (string.IsNullOrWhiteSpace(name) ? "<color=magenta>null</color>" : name);
		if ((bool)thumbnail && thumbnail != null)
		{
			ThumbnailImage.sprite = thumbnail;
		}
		else
		{
			ThumbnailImage.sprite = DefaultThumbnailSprite;
		}
	}

	private void Update()
	{
		TitleMesh.text = displayName ?? "";
		StateMesh.text = "<color=#ccc>" + displayState + "</color>";
	}

	public static void SetDisplayState(string state)
	{
		if ((bool)Instance)
		{
			Instance.displayState = state;
		}
	}

	public static void Show(string name, Sprite thumbnail)
	{
		if ((bool)Instance)
		{
			Instance.SetDisplayData(name, thumbnail);
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

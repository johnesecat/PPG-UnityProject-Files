using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionBoxBehaviour : MonoBehaviour
{
	public static SelectionBoxBehaviour Main;

	private Image image;

	private RectTransform rectTransform;

	public TextMeshProUGUI WidthDisplay;

	public TextMeshProUGUI HeightDisplay;

	private void OnEnable()
	{
		Main = this;
	}

	private void Awake()
	{
		image = GetComponent<Image>();
		rectTransform = GetComponent<RectTransform>();
	}

	private float Snap(float i)
	{
		return Mathf.Round(100f * Mathf.Abs(i)) / 100f;
	}

	private void Start()
	{
		Hide();
	}

	public void SetSize(float width, float height)
	{
		Vector2 sizeDelta = rectTransform.sizeDelta;
		sizeDelta.x = width;
		sizeDelta.y = height;
		rectTransform.sizeDelta = sizeDelta;
	}

	public void SetSizeDisplay(float x, float y)
	{
		WidthDisplay.text = Snap(x) + "m";
		HeightDisplay.text = Snap(y) + "m";
	}

	public void Show()
	{
		image.enabled = true;
		HeightDisplay.enabled = true;
		WidthDisplay.enabled = true;
	}

	public void Hide()
	{
		image.enabled = false;
		WidthDisplay.enabled = false;
		HeightDisplay.enabled = false;
	}
}

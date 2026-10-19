using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorpickerDialogBehaviour : MonoBehaviour
{
	public static ColorpickerDialogBehaviour Instance;

	public Color CurrentColor;

	[Space]
	public TextMeshProUGUI Title;

	public TextMeshProUGUI Description;

	[Space]
	public UnityEvent OnCancel = new UnityEvent();

	public UnityEvent<Color> OnApply = new UnityEvent<Color>();

	[Space]
	public RectTransform SaturationValueBox;

	public RectTransform SaturationValueBoxDrag;

	public Graphic SaturationValueImage;

	private UiEventEmitter saturationValueBoxEvents;

	[Space]
	public RectTransform HueBox;

	public RectTransform HueBoxDrag;

	private UiEventEmitter hueBoxEvents;

	[Space]
	public TMP_InputField HexCodeInput;

	[Space]
	public Image PreviewBox;

	private bool saturationValueBoxDragging;

	private bool hueBoxDragging;

	public static bool IsOpen;

	public void Apply()
	{
		OnApply.Invoke(CurrentColor);
		Close();
	}

	public void Cancel()
	{
		Close();
		OnCancel.Invoke();
	}

	public void Open(Color initialColor)
	{
		if (!IsOpen)
		{
			IsOpen = true;
			CurrentColor = initialColor;
			base.gameObject.SetActive(value: true);
			UpdateHexCodeInput();
			DialogBox.OpenDialogboxCount++;
			Global.main.AddUiBlocker();
		}
	}

	public void Open()
	{
		Open(Color.red);
	}

	public void Close()
	{
		if (IsOpen)
		{
			IsOpen = false;
			hueBoxDragging = false;
			saturationValueBoxDragging = false;
			base.gameObject.SetActive(value: false);
			DialogBox.OpenDialogboxCount--;
			Global.main.RemoveUiBlocker();
		}
	}

	private void Awake()
	{
		Instance = this;
		saturationValueBoxEvents = SaturationValueBox.GetComponent<UiEventEmitter>();
		hueBoxEvents = HueBox.GetComponent<UiEventEmitter>();
		saturationValueBoxEvents.OnMouseDown.AddListener(delegate
		{
			saturationValueBoxDragging = true;
		});
		saturationValueBoxEvents.OnMouseUp.AddListener(delegate
		{
			saturationValueBoxDragging = false;
		});
		hueBoxEvents.OnMouseDown.AddListener(delegate
		{
			hueBoxDragging = true;
		});
		hueBoxEvents.OnMouseUp.AddListener(delegate
		{
			hueBoxDragging = false;
		});
		HexCodeInput.onValueChanged.AddListener(delegate(string e)
		{
			if (!e.StartsWith("#"))
			{
				e = e.Insert(0, "#");
			}
			ColorUtility.TryParseHtmlString(e, out CurrentColor);
		});
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	private void UpdateHexCodeInput()
	{
		HexCodeInput.text = "#" + ColorUtility.ToHtmlStringRGB(CurrentColor);
	}

	private void Update()
	{
		Color.RGBToHSV(CurrentColor, out var H, out var S, out var V);
		SaturationValueImage.material.SetFloat(ShaderProperties.Get("_Hue"), H);
		if (saturationValueBoxDragging)
		{
			Bounds worldSpaceBounds = SaturationValueBox.GetWorldSpaceBounds();
			SaturationValueBoxDrag.position = worldSpaceBounds.ClosestPoint(Input.mousePosition);
			S = Mathf.Abs(SaturationValueBoxDrag.anchoredPosition.x / SaturationValueBox.sizeDelta.x);
			V = 1f - Mathf.Abs(SaturationValueBoxDrag.anchoredPosition.y / SaturationValueBox.sizeDelta.y);
			UpdateHexCodeInput();
		}
		else
		{
			SaturationValueBoxDrag.anchoredPosition = new Vector2(SaturationValueBox.sizeDelta.x * S, SaturationValueBox.sizeDelta.y * (0f - (1f - V)));
		}
		if (hueBoxDragging)
		{
			Bounds worldSpaceBounds2 = HueBox.GetWorldSpaceBounds();
			HueBoxDrag.position = new Vector3(HueBoxDrag.position.x, worldSpaceBounds2.ClosestPoint(Input.mousePosition).y);
			H = 1f - Mathf.Abs(HueBoxDrag.anchoredPosition.y / HueBox.sizeDelta.y);
			UpdateHexCodeInput();
		}
		else
		{
			HueBoxDrag.anchoredPosition = new Vector2(HueBoxDrag.anchoredPosition.x, HueBox.sizeDelta.y * (0f - (1f - H)));
		}
		CurrentColor = Color.HSVToRGB(H, S, V);
		PreviewBox.color = CurrentColor;
	}
}

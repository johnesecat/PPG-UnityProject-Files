using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapViewBehaviour : MonoBehaviour
{
	public Map Map;

	private TextMeshProUGUI text;

	private Image image;

	private float clickHeat;

	private bool alreadyLoaded;

	public UnityEvent OnSelect;

	private void Awake()
	{
		text = GetComponentInChildren<TextMeshProUGUI>();
		image = GetComponent<Image>();
	}

	private void Start()
	{
		text.text = Map.name;
		image.sprite = Map.Thumbnail;
	}

	private void Update()
	{
		clickHeat -= Time.deltaTime;
		clickHeat = Mathf.Clamp01(clickHeat);
	}

	public void Select()
	{
		MapLoaderBehaviour.CurrentMap = Map;
		OnSelect?.Invoke();
		if (clickHeat > 0.025f && !alreadyLoaded)
		{
			alreadyLoaded = true;
			GetComponent<SceneSwitchBehaviour>().Switch();
		}
		clickHeat = 0.2f;
	}
}

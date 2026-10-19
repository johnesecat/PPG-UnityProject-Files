using TMPro;
using UnityEngine;

public class ThermometerBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public SpriteRenderer Light;

	[SkipSerialisation]
	public TextMeshPro TextMesh;

	[SkipSerialisation]
	public PhysicalBehaviour Phys;

	public bool Activated = true;

	public int Min = -999;

	public int Max = 9999;

	private float lastShownTemp = float.MinValue;

	private const float tempCacheThreshold = 0.45f;

	private void Start()
	{
		UpdateActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}

	private void UpdateActivation()
	{
		SpriteRenderer light = Light;
		bool flag = (TextMesh.enabled = Activated);
		light.enabled = flag;
	}

	private void FixedUpdate()
	{
		float num = Mathf.Clamp(Phys.Temperature, Min, Max);
		if (Mathf.Abs(num - lastShownTemp) > 0.45f)
		{
			lastShownTemp = num;
			switch (UserPreferenceManager.Current.TemperatureUnit)
			{
			case TemperatureUnit.Fahrenheit:
				TextMesh.text = $"<mspace=0.057>{Mathf.Clamp(Mathf.RoundToInt(Utils.CelsiusToFahrenheit(num)), -999, 9999)} 'F";
				break;
			case TemperatureUnit.Kelvin:
				TextMesh.text = $"<mspace=0.057>{Mathf.Clamp(Mathf.RoundToInt(Utils.CelsiusToKelvin(num)), 0, 9999)}  K";
				break;
			default:
				TextMesh.text = $"<mspace=0.057>{Mathf.RoundToInt(num)} 'C";
				break;
			}
		}
	}

	private void OnEnable()
	{
		UpdateActivation();
	}

	private void OnDisable()
	{
		SpriteRenderer light = Light;
		bool flag2 = (TextMesh.enabled = false);
		light.enabled = flag2;
	}
}

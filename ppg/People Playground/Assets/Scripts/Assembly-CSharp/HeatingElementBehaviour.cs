using NaughtyAttributes;
using UnityEngine;

public class HeatingElementBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public SpriteRenderer ActivationLight;

	[SkipSerialisation]
	public DamagableMachineryBehaviour DamagableMachineryBehaviour;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	[ColorUsage(true, true)]
	public Color Color;

	[SkipSerialisation]
	public SpriteRenderer Glow;

	[SkipSerialisation]
	[MinMaxSlider(-1000f, 1000f)]
	public Vector2 AllowedTemperatureRange = new Vector2(0f, 1000f);

	[SkipSerialisation]
	public float HeatTargetSpeed = 0.1f;

	[SkipSerialisation]
	public bool ActiveCooling;

	public float TargetTemperature = 1f;

	public bool Activated;

	private void Start()
	{
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("heatingElementThingWhateverWhoCaresAtThisPoint" + ActiveCooling, "Set target temperature", "Set target temperature", delegate
		{
			Utils.OpenFloatInputDialog(Utils.CelsiusToPreference(TargetTemperature), this, delegate(HeatingElementBehaviour h, float f)
			{
				h.TargetTemperature = Mathf.Clamp(Utils.PreferenceToCelsius(f), AllowedTemperatureRange.x, AllowedTemperatureRange.y);
			}, "Set target temperature", $"Target temperature in {UserPreferenceManager.Current.TemperatureUnit} ({AllowedTemperatureRange.x} to {AllowedTemperatureRange.y})");
		}));
		ActivationLight.enabled = Activated;
	}

	public void Use(ActivationPropagation activation)
	{
		if (!DamagableMachineryBehaviour.Destroyed)
		{
			Activated = !Activated;
			ActivationLight.enabled = Activated;
		}
	}

	private void FixedUpdate()
	{
		bool flag = ActiveCooling ^ (PhysicalBehaviour.Temperature <= TargetTemperature);
		if (Activated && !DamagableMachineryBehaviour.Destroyed && flag)
		{
			PhysicalBehaviour.Temperature = Mathf.Lerp(PhysicalBehaviour.Temperature, TargetTemperature, HeatTargetSpeed);
		}
	}

	private void OnWillRenderObject()
	{
		float value = Mathf.Clamp(PhysicalBehaviour.Temperature, AllowedTemperatureRange.x, AllowedTemperatureRange.y);
		float num = Mathf.Clamp01(Utils.MapRange(AllowedTemperatureRange.x, AllowedTemperatureRange.y, 0f, 1f, value));
		if (ActiveCooling)
		{
			num = 1f - num;
		}
		Glow.color = Color.Lerp(Color.clear, Color, num * num);
	}
}

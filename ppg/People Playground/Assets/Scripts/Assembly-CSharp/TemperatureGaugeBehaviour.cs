using UnityEngine;

public class TemperatureGaugeBehaviour : GaugeBehaviour
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public float MaxTemperatureDisplay = 2000f;

	protected override float GetProgress()
	{
		return TempFunction(PhysicalBehaviour.Temperature / MaxTemperatureDisplay);
	}

	protected override string GetDisplayValueFor(float progress)
	{
		return (Mathf.RoundToInt(MaxTemperatureDisplay * TempFunction(progress) / 10f) * 10).ToString();
	}

	private float TempFunction(float i)
	{
		return i;
	}
}

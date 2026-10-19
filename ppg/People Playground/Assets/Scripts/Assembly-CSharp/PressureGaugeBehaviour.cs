using UnityEngine;

public class PressureGaugeBehaviour : GaugeBehaviour
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public float MaxChargeDisplay = 500f;

	protected override float GetProgress()
	{
		return ChargeFunction(PhysicalBehaviour.Charge / MaxChargeDisplay);
	}

	protected override string GetDisplayValueFor(float progress)
	{
		return (Mathf.RoundToInt(MaxChargeDisplay * ChargeFunction(progress) / 10f) * 10).ToString();
	}

	private float ChargeFunction(float i)
	{
		return Mathf.Sqrt(i);
	}
}

using UnityEngine;

public class AcidPoolBehaviour : MonoBehaviour
{
	public WaterBehaviour Pool;

	public float TemperatureIncreaseCelsius = 15f;

	public float AcidProgress = 0.01f;

	public float MaxTemperatureCelsius = 300f;

	public bool OnlyOrganic = true;

	public float WinceIntensity = 300f;

	public float PainIntensity = 0.1f;

	private void FixedUpdate()
	{
		if (Pool.IsFrozen || Pool.IsEvaporated)
		{
			return;
		}
		float num = TemperatureIncreaseCelsius * Time.fixedDeltaTime;
		foreach (PhysicalBehaviour submergedObject in Pool.GetSubmergedObjects())
		{
			if ((double)Random.value > 0.99)
			{
				submergedObject.SendMessage("Break", Vector2.zero, SendMessageOptions.DontRequireReceiver);
			}
			if (!(submergedObject.Properties.Softness > 0f) || !(submergedObject.Properties.Flammability > 0f))
			{
				continue;
			}
			if (submergedObject.SimulateTemperature && submergedObject.Temperature < MaxTemperatureCelsius)
			{
				if ((double)Random.value > 0.9995)
				{
					submergedObject.Sizzle();
				}
				if ((double)Random.value > 0.955)
				{
					submergedObject.Ignite(ignoreFlammability: true);
				}
				submergedObject.Temperature += num;
			}
			if (submergedObject.TryGetComponent<SkinMaterialHandler>(out var component))
			{
				component.AcidProgress += AcidProgress;
				component.limb.Wince(WinceIntensity);
				if (component.limb.NodeBehaviour.IsConnectedToRoot)
				{
					component.limb.Person.AddPain(PainIntensity);
				}
			}
		}
	}
}

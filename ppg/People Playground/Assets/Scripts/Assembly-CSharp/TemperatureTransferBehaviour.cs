using UnityEngine;

public class TemperatureTransferBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public PhysicalBehaviour Other;

	public float GuideSpeed = 0.001f;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void FixedUpdate()
	{
		if (phys.SimulateTemperature && Other.SimulateTemperature)
		{
			float temperature = phys.Temperature;
			float temperature2 = Other.Temperature;
			phys.Temperature = Mathf.Lerp(temperature, temperature2, GuideSpeed);
			Other.Temperature = Mathf.Lerp(temperature2, temperature, GuideSpeed);
		}
	}
}

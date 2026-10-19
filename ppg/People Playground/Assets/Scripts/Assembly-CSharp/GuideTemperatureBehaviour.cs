using UnityEngine;

public class GuideTemperatureBehaviour : MonoBehaviour
{
	public float TargetTemperature;

	public float GuideSpeed = 0.001f;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void FixedUpdate()
	{
		phys.Temperature = Mathf.Lerp(phys.Temperature, TargetTemperature, GuideSpeed);
	}
}

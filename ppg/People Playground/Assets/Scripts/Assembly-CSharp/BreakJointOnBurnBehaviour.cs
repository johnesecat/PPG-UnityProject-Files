using UnityEngine;

public class BreakJointOnBurnBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public Joint2D Joint;

	private float initialThreshold = float.PositiveInfinity;

	private PhysicalBehaviour phys;

	private float randomOffset = 1f;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		initialThreshold = Joint.breakForce;
		randomOffset = Random.Range(1, 3);
	}

	private void Update()
	{
		if ((bool)Joint)
		{
			float num = Mathf.Clamp01(1f - phys.BurnProgress * randomOffset);
			Joint.breakForce = initialThreshold * num;
		}
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		if (joint == Joint)
		{
			Object.Destroy(this);
		}
	}
}

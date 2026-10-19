using UnityEngine;

[ExecuteAlways]
public class BreakJointOnShotBehaviour : MonoBehaviour, Messages.IShot, Messages.ISlice
{
	[SkipSerialisation]
	public Joint2D Joint;

	public float Chance = 0.05f;

	public float ShotIntensityInfluence = 1f;

	private void OnJointBreak2D(Joint2D joint)
	{
		if (joint == Joint)
		{
			Object.Destroy(this);
		}
	}

	public void Shot(Shot shot)
	{
		float num = Chance + shot.damage * ShotIntensityInfluence;
		if (Random.value < num && (bool)Joint)
		{
			Joint.breakForce = -1f;
		}
	}

	private void OnEnable()
	{
		if (!Joint)
		{
			Joint = GetComponent<Joint2D>();
		}
	}

	public void Slice()
	{
		Joint.breakForce = -1f;
	}
}

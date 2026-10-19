using UnityEngine;

public class CopperWireBehaviour : EnergyWireBehaviour
{
	protected float breakForceBuffer = 1f;

	protected override void Start()
	{
		base.Start();
		breakForceBuffer = currentBreakingForce;
	}

	public override void Slice()
	{
		breakForceBuffer = 0f;
	}

	protected override void JointBroken()
	{
		if ((bool)physicalBehaviour)
		{
			physicalBehaviour.PlayClipOnce(Resources.LoadAll<AudioClip>("Audio/copper wire break").PickRandom(), Random.value);
		}
	}

	protected override void Tick()
	{
		base.Tick();
		float num = 0f;
		if ((bool)physicalBehaviour)
		{
			num = physicalBehaviour.BurnProgress;
		}
		if ((bool)otherPhysicalBehaviour)
		{
			num = Mathf.Max(num, otherPhysicalBehaviour.BurnProgress);
		}
		typedJoint.breakForce = breakForceBuffer * (1f - num);
	}

	protected override void SetWireColours()
	{
	}
}

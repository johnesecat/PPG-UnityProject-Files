using UnityEngine;

public class WoodStrutBehaviour : DistanceJointWireBehaviour
{
	protected float breakForceBuffer = 1f;

	private static float lastStressSoundTime = float.MinValue;

	private const float MinTimeDeltaSeconds = 0.5f;

	public override int GetVertexCount()
	{
		return 1;
	}

	public override void Slice()
	{
		breakForceBuffer = 0f;
	}

	protected override void Start()
	{
		base.Start();
		breakForceBuffer = currentBreakingForce;
		if (lineRenderer.startColor == LegacyColour)
		{
			SetColor(Color.white);
		}
	}

	protected override void JointBroken()
	{
		if ((bool)physicalBehaviour)
		{
			physicalBehaviour.PlayClipOnce(Global.main.WoodStutSnapClips.PickRandom(), Random.value);
		}
	}

	protected override void Created()
	{
		WireColor = Color.white;
		lineRenderer.startColor = Color.white;
		lineRenderer.endColor = Color.white;
		lineRenderer.numCapVertices = 0;
	}

	protected override void Tick()
	{
		if (Time.time - lastStressSoundTime > 0.5f && (bool)physicalBehaviour && Random.value > 0.9997f && typedJoint.reactionForce.magnitude / typedJoint.breakForce > 0.1f)
		{
			lastStressSoundTime = Time.time;
			physicalBehaviour.PlayClipOnce(Global.main.WoodStrutStressClips.PickRandom(), 0.5f);
		}
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
}

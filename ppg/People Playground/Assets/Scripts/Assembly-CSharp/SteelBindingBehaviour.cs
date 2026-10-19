using UnityEngine;

public class SteelBindingBehaviour : FixedJointWireBehaviour
{
	[SkipSerialisation]
	protected static AudioClip[] stress;

	[SkipSerialisation]
	protected static AudioClip[] breakSound;

	protected float breakForceBuffer = 1f;

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
			physicalBehaviour.PlayClipOnce(breakSound.PickRandom(), Random.value);
		}
	}

	protected override void Created()
	{
		if (stress == null)
		{
			stress = Resources.LoadAll<AudioClip>("Audio/metal stress/");
		}
		if (breakSound == null)
		{
			breakSound = Resources.LoadAll<AudioClip>("Audio/steel break");
		}
		WireColor = Color.white;
		lineRenderer.startColor = Color.white;
		lineRenderer.endColor = Color.white;
		lineRenderer.numCapVertices = 0;
		lineRenderer.sortingLayerName = "Background";
		lineRenderer.textureMode = LineTextureMode.Tile;
	}

	protected override void Tick()
	{
		if ((bool)physicalBehaviour && Random.value > 0.9997f && typedJoint.reactionForce.magnitude / typedJoint.breakForce > 0.2f)
		{
			physicalBehaviour.PlayClipOnce(stress.PickRandom(), 0.5f);
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

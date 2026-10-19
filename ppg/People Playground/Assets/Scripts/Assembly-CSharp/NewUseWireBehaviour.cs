using Activations;
using UnityEngine;

public class NewUseWireBehaviour : DistanceJointWireBehaviour
{
	public Channel Channels = Channel.All;

	public bool CanBreak;

	public Color Color = new Color(11f, 1f, 11f);

	protected float breakForceBuffer = 1f;

	private MaterialPropertyBlock materialPropertyBlock;

	private NodeBehaviour from;

	private NodeBehaviour to;

	private ActivationTarget target;

	protected override void Start()
	{
		base.Start();
		materialPropertyBlock = new MaterialPropertyBlock();
		lineRenderer.GetPropertyBlock(materialPropertyBlock);
		lineRenderer.widthMultiplier = 0.05f;
		materialPropertyBlock.SetColor(ShaderProperties.Get("_GlowColour"), Color);
		lineRenderer.SetPropertyBlock(materialPropertyBlock);
		breakForceBuffer = (CanBreak ? currentBreakingForce : float.PositiveInfinity);
		typedJoint.breakForce = breakForceBuffer;
		if (!TryGetComponent<NodeBehaviour>(out from))
		{
			return;
		}
		if (typedJoint.connectedBody.TryGetComponent<NodeBehaviour>(out to))
		{
			if (to == from)
			{
				to = null;
				return;
			}
			target = new ActivationTarget(to, Channels);
			from.Targets.Add(target);
		}
		from.OnUseStart.AddListener(OnUseStart);
	}

	private void OnUseStart(Channel c)
	{
		if ((c & Channels) != 0)
		{
			SendGlow();
		}
	}

	protected override void Tick()
	{
		base.Tick();
		if ((from.CurrentlyActive & Channels) != 0)
		{
			SendGlow();
		}
		if (CanBreak)
		{
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

	public override void Slice()
	{
		breakForceBuffer = 0f;
	}

	protected override void JointBroken()
	{
		if ((bool)physicalBehaviour)
		{
			physicalBehaviour.PlayClipOnce(Resources.LoadAll<AudioClip>("Audio/use wire break").PickRandom(), Random.value);
		}
	}

	private void SendGlow()
	{
		materialPropertyBlock.SetFloat(ShaderProperties.Get("_Timestamp"), Time.timeSinceLevelLoad);
		lineRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ((bool)from)
		{
			from.OnUseStart.RemoveListener(OnUseStart);
			if ((bool)to)
			{
				from.Targets.Remove(target);
			}
		}
	}
}

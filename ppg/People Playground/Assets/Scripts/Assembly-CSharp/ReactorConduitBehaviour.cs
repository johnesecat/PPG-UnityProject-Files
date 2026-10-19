using UnityEngine;

public class ReactorConduitBehaviour : BloodContainer
{
	public SerialisableDistribution OptionalInflow;

	public ReactorConduitBehaviour LiquidPushTarget;

	public AudioClip[] BreakClips;

	private FixedJoint2D tubeJoint;

	private HingeJoint2D reactorJoint;

	private PhysicalBehaviour phys;

	private PhysicalBehaviour connectedConduit;

	private ReactorConduitBehaviour source;

	private ReactorCoreBehaviour core;

	private float initialBreakForce;

	private float randomResilience;

	public bool Broken;

	public bool HealthyWallJoint = true;

	public override Vector2 Limits => new Vector2(0f, 1f);

	public override bool AllowsOverflow => false;

	private void Awake()
	{
		Broken = false;
		core = Object.FindObjectOfType<ReactorCoreBehaviour>();
		tubeJoint = GetComponent<FixedJoint2D>();
		reactorJoint = GetComponent<HingeJoint2D>();
		phys = GetComponent<PhysicalBehaviour>();
		if ((bool)tubeJoint)
		{
			initialBreakForce = tubeJoint.breakForce;
			randomResilience = Random.Range(0, 100);
			phys.ForceNoChargeParticles = true;
			connectedConduit = tubeJoint.connectedBody.GetComponent<PhysicalBehaviour>();
		}
		if ((bool)LiquidPushTarget)
		{
			LiquidPushTarget.source = this;
		}
	}

	private void FixedUpdate()
	{
		if (OptionalInflow.Amount > 0f)
		{
			AddLiquid(Liquid.GetLiquid(OptionalInflow.LiquidID), OptionalInflow.Amount);
		}
		if ((bool)connectedConduit)
		{
			Utils.AverageTemperature(phys, connectedConduit, 0.2f);
			Utils.TransferEnergyFixedRate(phys, connectedConduit, 0.99f);
		}
		if ((bool)LiquidPushTarget && !LiquidPushTarget.IsFull())
		{
			TransferTo(0.1f, LiquidPushTarget);
		}
		if ((bool)core && (bool)reactorJoint)
		{
			phys.Temperature = Mathf.Lerp(phys.Temperature, core.InternalTemperature, 0.5f);
		}
		if ((bool)tubeJoint)
		{
			float a = Mathf.Clamp01(Utils.MapRange(-100f - randomResilience, 20f, 0f, 1f, phys.Temperature));
			float b = Mathf.Clamp01(Utils.MapRange(5000f, 10000f + randomResilience, 1f, 0f, phys.Temperature));
			tubeJoint.breakForce = initialBreakForce * Mathf.Min(a, b);
			if (!Broken && phys.Temperature > 6000f + randomResilience && Random.value > 0.9999f)
			{
				ExplosionCreator.Explode(new ExplosionCreator.ExplosionParameters(16u, base.transform.position, 4f, 64f, createFx: true, ExplosionCreator.EffectSize.Small, 0f, 4));
			}
		}
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		if (((bool)tubeJoint && joint == tubeJoint) || ((bool)reactorJoint && joint == reactorJoint))
		{
			phys.PlayClipOnce(BreakClips.PickRandom());
			if (joint == tubeJoint)
			{
				if ((bool)LiquidPushTarget && tubeJoint.connectedBody.gameObject == LiquidPushTarget.gameObject)
				{
					LiquidPushTarget.source = null;
					LiquidPushTarget = null;
				}
				Broken = true;
			}
			else if (joint == reactorJoint)
			{
				HealthyWallJoint = false;
			}
		}
		if ((bool)source && joint == source.tubeJoint)
		{
			source.LiquidPushTarget = null;
			source = null;
		}
		if ((bool)LiquidPushTarget && joint == LiquidPushTarget.tubeJoint)
		{
			LiquidPushTarget.source = null;
			LiquidPushTarget = null;
		}
	}
}

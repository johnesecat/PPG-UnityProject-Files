using UnityEngine;

public class LiquidOutletBehaviour : BloodContainer
{
	[SkipSerialisation]
	public ParticleSystem LiquidParticles;

	[SkipSerialisation]
	public ParticleSystem WaterPuffParticles;

	[SkipSerialisation]
	public FancyBloodSplatController PhysicalParticles;

	[SkipSerialisation]
	public PhysicalBehaviour Phys;

	public const float DrainRate = 0.1f;

	public override float ScaledLiquidAmount => base.ScaledLiquidAmount;

	public override PressureDirection Pressure => PressureDirection.Pull;

	public override bool AllowsOverflow => true;

	public override bool AllowsTransfer => true;

	public override Vector2 Limits => new Vector2(0f, 2.8f);

	public override bool AllowPressureTransfer => true;

	private void Awake()
	{
		PhysicalParticles.OnCollide += OnParticleCollide;
	}

	private bool IsParticleSubmerged()
	{
		return WaterBehaviour.IsPointUnderWater(WaterPuffParticles.transform.position);
	}

	private void FixedUpdate()
	{
		float num = Phys.Charge * 0.4f + 2f;
		PhysicalParticles.SpawningChance = 0f;
		bool num2 = IsParticleSubmerged();
		bool flag = base.TotalLiquidAmount > 0.1f;
		if (num2)
		{
			ParticleSystem.EmissionModule emission = WaterPuffParticles.emission;
			ParticleSystem.MainModule main = WaterPuffParticles.main;
			main.startColor = GetComputedColor();
			emission.rateOverTimeMultiplier = (flag ? 1 : 0);
			emission.rateOverDistanceMultiplier = (flag ? 3 : 0);
		}
		else
		{
			ParticleSystem.EmissionModule emission2 = LiquidParticles.emission;
			ParticleSystem.MainModule main2 = LiquidParticles.main;
			main2.startColor = GetComputedColor();
			main2.startSpeedMultiplier = num;
			emission2.rateOverTimeMultiplier = (flag ? 120 : 0);
			PhysicalParticles.DecalColourMultiplier = GetComputedColor();
			PhysicalParticles.EjectionForce = new Vector2(0.25f, 0.3f) * num;
		}
		if (num2)
		{
			if (LiquidParticles.isPlaying)
			{
				LiquidParticles.Stop();
			}
			if (!WaterPuffParticles.isPlaying)
			{
				WaterPuffParticles.Play();
			}
		}
		else
		{
			if (!LiquidParticles.isPlaying)
			{
				LiquidParticles.Play();
			}
			if (WaterPuffParticles.isPlaying)
			{
				WaterPuffParticles.Stop();
			}
		}
		if (flag)
		{
			PhysicalParticles.CreateParticle();
			Drain(0.1f);
		}
	}

	private void OnParticleCollide(Collider2D hit)
	{
		if (hit.TryGetComponent<BloodContainer>(out var component))
		{
			CopyTo(0.1f, component);
		}
	}

	private void OnDestroy()
	{
		PhysicalParticles.OnCollide -= OnParticleCollide;
	}
}

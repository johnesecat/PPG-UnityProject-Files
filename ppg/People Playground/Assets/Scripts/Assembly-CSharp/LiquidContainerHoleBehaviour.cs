using UnityEngine;

public class LiquidContainerHoleBehaviour : MonoBehaviour
{
	public float DrainRate = 0.1f;

	[SkipSerialisation]
	public float ForceMultiplier = 0.001f;

	[SkipSerialisation]
	public BloodContainer Source;

	[SkipSerialisation]
	public ParticleSystem LiquidParticles;

	[SkipSerialisation]
	public ParticleSystem WaterPuffParticles;

	[SkipSerialisation]
	public FancyBloodSplatController PhysicalParticles;

	private bool IsParticleSubmerged()
	{
		return WaterBehaviour.IsPointUnderWater(WaterPuffParticles.transform.position);
	}

	private void Awake()
	{
		PhysicalParticles.OnCollide += OnParticleCollide;
	}

	private void FixedUpdate()
	{
		float num = Source.MeasuredPressure * ForceMultiplier + 0.5f;
		PhysicalParticles.SpawningChance = 0f;
		bool num2 = IsParticleSubmerged();
		bool flag = Source.TotalLiquidAmount > DrainRate;
		if (num2)
		{
			ParticleSystem.EmissionModule emission = WaterPuffParticles.emission;
			ParticleSystem.MainModule main = WaterPuffParticles.main;
			main.startColor = Source.GetComputedColor();
			emission.rateOverTimeMultiplier = (flag ? 1 : 0);
			emission.rateOverDistanceMultiplier = (flag ? 3 : 0);
		}
		else
		{
			ParticleSystem.EmissionModule emission2 = LiquidParticles.emission;
			ParticleSystem.MainModule main2 = LiquidParticles.main;
			main2.startColor = Source.GetComputedColor();
			main2.startSpeedMultiplier = num;
			emission2.rateOverTimeMultiplier = (flag ? 16 : 0);
			PhysicalParticles.DecalColourMultiplier = Source.GetComputedColor();
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
		if (Source.TotalLiquidAmount > DrainRate)
		{
			PhysicalParticles.CreateParticle();
			Source.Drain(DrainRate);
		}
	}

	private void OnParticleCollide(Collider2D hit)
	{
		if (hit.transform != Source.transform && hit.TryGetComponent<BloodContainer>(out var component))
		{
			Source.CopyTo(DrainRate, component);
		}
	}
}

using UnityEngine;

public class BleedingParticleBehaviour : MonoBehaviour
{
	public AudioClip[] DripSounds;

	public CirculationBehaviour CirculationBehaviour;

	public ParticleSystem BloodParticleSystem;

	public ParticleSystem WaterPuffSystem;

	public CirculationBehaviour PushingTo;

	public FancyBloodSplatController BloodSplatController;

	public bool IsOnlyForInternalBleeding;

	private ParticleSystem.MainModule main;

	private ParticleSystem.EmissionModule emission;

	public float SpeedMultiplier = 1f;

	public float RateMultiplier = 1f;

	public float Laminarity;

	public float DripSoundRateThreshold = 2f;

	public bool ShouldBecomeSmokeInWater;

	private ParticleSystemRenderer particleRenderer;

	private FixedIntervalDistributor dripSoundClock;

	private void Awake()
	{
		BloodParticleSystem = GetComponent<ParticleSystem>();
		particleRenderer = BloodParticleSystem.GetComponent<ParticleSystemRenderer>();
		main = BloodParticleSystem.main;
		emission = BloodParticleSystem.emission;
		dripSoundClock = new FixedIntervalDistributor();
		dripSoundClock.RateHz = 0.5f;
		if (UserPreferenceManager.Current.FancyEffects)
		{
			ParticleSystem.CollisionModule collision = BloodParticleSystem.collision;
			collision.enabled = true;
		}
	}

	private void Start()
	{
		if (!ShouldBecomeSmokeInWater && (bool)WaterPuffSystem)
		{
			Object.Destroy(WaterPuffSystem);
		}
	}

	private void Update()
	{
		float num = (IsOnlyForInternalBleeding ? CirculationBehaviour.InternalBleedingIntensity : CirculationBehaviour.BleedingRate);
		if ((!PushingTo || PushingTo.IsDisconnected) && num > 0.05f && CirculationBehaviour.ScaledLiquidAmount > 0.05f && CirculationBehaviour.Limb.PhysicalBehaviour.Temperature > 0f)
		{
			Color color = CirculationBehaviour.GetComputedColor(CirculationBehaviour.Limb.GetOriginalBloodType().Color);
			if (UserPreferenceManager.Current.GorelessMode)
			{
				color = Utils.ChangeRedToOrange(in color);
			}
			bool flag = WaterBehaviour.IsPointUnderWater(base.transform.position);
			main.startSpeedMultiplier = SpeedMultiplier * (CirculationBehaviour.BloodFlow * CirculationBehaviour.BloodFlow * CirculationBehaviour.BloodFlow * 2f) * CirculationBehaviour.BloodLossRateMultiplier * Mathf.Lerp(0.7f, 1f, Laminarity);
			main.startColor = color;
			float num2 = ((CirculationBehaviour.TotalLiquidAmount > float.Epsilon) ? Mathf.Min(CirculationBehaviour.BloodLossRateMultiplier * num, 1f) : 0f);
			emission.rateOverTimeMultiplier = (flag ? 0f : (Mathf.Lerp(18f, 32f, Laminarity) * num2 * RateMultiplier));
			emission.enabled = true;
			ParticleSystem.ShapeModule shape = BloodParticleSystem.shape;
			shape.radius = Mathf.Lerp(0.1f, 0.02f, Laminarity);
			shape.arc = Mathf.Lerp(10f, 5f, Laminarity);
			ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = BloodParticleSystem.limitVelocityOverLifetime;
			ParticleSystem.MinMaxCurve drag = limitVelocityOverLifetime.drag;
			drag.constantMax = Mathf.Lerp(1f, drag.constantMin, Laminarity);
			limitVelocityOverLifetime.drag = drag;
			if ((bool)BloodSplatController)
			{
				BloodSplatController.DecalColourMultiplier = color;
				BloodSplatController.EjectionForce = new Vector2(0.25f, 0.35f) * main.startSpeedMultiplier;
				BloodSplatController.SpawningChance = (flag ? 0f : (CirculationBehaviour.TotalLiquidAmount * 0.5f));
			}
			bool flag2 = DripSounds != null && particleRenderer.isVisible && num2 * RateMultiplier >= DripSoundRateThreshold;
			if (ShouldBecomeSmokeInWater && (bool)WaterPuffSystem)
			{
				ParticleSystem.MainModule mainModule = WaterPuffSystem.main;
				mainModule.startColor = color;
				if (flag)
				{
					ParticleSystem.EmissionModule emissionModule = WaterPuffSystem.emission;
					emissionModule.rateOverTimeMultiplier = num2;
					if (!WaterPuffSystem.isPlaying)
					{
						WaterPuffSystem.Play();
					}
					flag2 = false;
				}
				else if (WaterPuffSystem.isPlaying)
				{
					WaterPuffSystem.Stop();
				}
			}
			if (!flag2)
			{
				return;
			}
			float num3 = num2 * RateMultiplier / 1.5f;
			float volume = Mathf.Clamp(0.1f * Mathf.Max(0.1f, num3 * num3 * num3), 0.01f, 0.1f);
			for (int i = 0; i < dripSoundClock.CalculateCycleCount(Time.deltaTime); i++)
			{
				if (Random.value > 0.2f)
				{
					CirculationBehaviour.Limb.PhysicalBehaviour.PlayClipOnce(DripSounds.PickRandom(), volume);
				}
			}
		}
		else
		{
			main.startSpeedMultiplier = 0f;
			emission.rateOverTimeMultiplier = 0f;
			emission.enabled = false;
			if ((bool)BloodSplatController)
			{
				BloodSplatController.SpawningChance = 0f;
			}
			if (ShouldBecomeSmokeInWater && (bool)WaterPuffSystem && WaterPuffSystem.isPlaying)
			{
				WaterPuffSystem.Stop();
			}
		}
	}
}

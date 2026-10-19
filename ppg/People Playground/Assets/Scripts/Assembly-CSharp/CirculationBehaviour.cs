using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(LimbBehaviour))]
public class CirculationBehaviour : BloodContainer, Messages.IShot, Messages.IExitShot, Messages.IStabbed, Messages.IOnImpactCreated, Messages.IUnstabbed
{
	[SkipSerialisation]
	[HideInInspector]
	public LimbBehaviour Limb;

	[SkipSerialisation]
	[HideInInspector]
	public CirculationBehaviour Source;

	[SkipSerialisation]
	public CirculationBehaviour[] PushesTo;

	[SkipSerialisation]
	[HideInInspector]
	public List<GameObject> BleedingParticles = new List<GameObject>();

	[Header("Settings")]
	public bool IsPump;

	public float BloodLossRateMultiplier = 1f;

	[Header("Status")]
	[ReadOnly]
	public float BleedingRate;

	[ReadOnly]
	public bool IsDisconnected;

	[ReadOnly]
	public bool WasInitiallyPumping;

	[ReadOnly]
	public float BloodFlow;

	private const byte MaximumBleedingPoints = 8;

	public float BloodRegenerationPerSecond = 0.0025f;

	[HideInInspector]
	public byte BleedingPointCount;

	[HideInInspector]
	public ushort StabWoundCount;

	[HideInInspector]
	public ushort GunshotWoundCount;

	public float InternalBleedingIntensity;

	[SkipSerialisation]
	public Bounds[] LocalArteryRects;

	public const float ActualBloodLimit = 1f;

	public bool ImmuneToDamage;

	private bool diffuseOscillator;

	private bool canUnlockGlowingHumanAchievement = true;

	[ReadOnly]
	public bool NewlySpawned = true;

	[SkipSerialisation]
	public float ArtificialHeartbeat;

	private float? cachedHeartRate;

	[SkipSerialisation]
	public bool HasCirculation
	{
		get
		{
			if (BloodFlow > 0.01f)
			{
				return GetAmountOfBlood() > 0.01f;
			}
			return false;
		}
	}

	[SkipSerialisation]
	public bool HasBloodFlow => BloodFlow > 0.05f;

	[Obsolete]
	public bool IsConnectedToMainBody
	{
		get
		{
			if (!Limb || !Limb.NodeBehaviour)
			{
				return HasBloodFlow;
			}
			return Limb.NodeBehaviour.IsConnectedToRoot;
		}
	}

	public override Vector2 Limits => new Vector2(0f, 1f);

	public override bool AllowsTransfer => true;

	public float GetAmountOfBlood()
	{
		return GetAmount(Limb.GetOriginalBloodType());
	}

	public float GetHeartRate()
	{
		if (!IsPump || !Limb.IsConsideredAlive)
		{
			return ArtificialHeartbeat;
		}
		if (cachedHeartRate.HasValue)
		{
			return cachedHeartRate.Value;
		}
		bool isConnectedToRoot = Limb.NodeBehaviour.IsConnectedToRoot;
		float num = Mathf.Clamp01(BloodFlow);
		float t = Mathf.Clamp01(base.TotalLiquidAmount);
		float t2 = (isConnectedToRoot ? Mathf.Clamp01(Limb.Person.OxygenLevel) : 0f);
		float num2 = (isConnectedToRoot ? Mathf.Clamp01(Limb.Person.Consciousness) : 0f);
		float num3 = (isConnectedToRoot ? Mathf.Clamp01(Limb.Person.PainLevel) : 0f);
		float num4 = (isConnectedToRoot ? Mathf.Clamp01(Limb.Person.AdrenalineLevel) : 0f);
		float num5 = (isConnectedToRoot ? Mathf.Clamp01(Limb.Person.ShockLevel) : 0f);
		float num6 = Mathf.Lerp(0f, 95f, num4 + num5 + num3);
		num6 += (isConnectedToRoot ? Mathf.Clamp(Limb.Person.SeizureTime * 0.5f, 0f, 30f) : 0f);
		num6 += Mathf.PerlinNoise(Time.time / 7f, Limb.randomOffset) * 10f;
		cachedHeartRate = Mathf.Max(ArtificialHeartbeat, (Mathf.Lerp(0f, 70f - (1f - num2) * 22f, t) + num6) * Mathf.Lerp(0.5f, 1f, t2) * num);
		return cachedHeartRate.Value;
	}

	private void Awake()
	{
		NewlySpawned = true;
		WasInitiallyPumping = IsPump;
		BloodFlow = 1f;
		CirculationBehaviour[] pushesTo = PushesTo;
		for (int i = 0; i < pushesTo.Length; i++)
		{
			pushesTo[i].Source = this;
		}
	}

	private void Start()
	{
		if (Limb.SpeciesIdentity != "Human" || SerialisableDistributions.Any(glowingCheck))
		{
			canUnlockGlowingHumanAchievement = false;
		}
		Liquid liquid = Liquid.GetLiquid(Limb.BloodLiquidType);
		if (NewlySpawned)
		{
			AddLiquid(liquid, 1f);
			NewlySpawned = false;
		}
		if (!Limb.PhysicalBehaviour.isDisintegrated)
		{
			CreateDismembermentBloodParticle();
		}
		bool glowingCheck(SerialisableDistribution s)
		{
			if (s.LiquidID == "TRITIUM")
			{
				return s.Amount > float.Epsilon;
			}
			return false;
		}
	}

	private void FixedUpdate()
	{
		cachedHeartRate = null;
		float fixedDeltaTime = Time.fixedDeltaTime;
		if (Limb.InternalTemperature <= Limb.BodyTemperature - 9f && UnityEngine.Random.value > 0.999f)
		{
			IsPump = false;
		}
		if (Limb.NodeBehaviour.IsConnectedToRoot && Limb.Person.SeizureTime > 0f && UnityEngine.Random.value > 0.999f)
		{
			IsPump = false;
		}
		PumpBehaviour(fixedDeltaTime);
		HandleBleeding(fixedDeltaTime);
		HandlePenetrationBleeding();
		HandleDamageEdgeCases();
		BloodFlow = Mathf.Clamp01(BloodFlow);
		InternalBleedingIntensity = Mathf.Max(0f, InternalBleedingIntensity - fixedDeltaTime * 0.01f);
		DiffuseStep();
		FlowStep();
		if (!IsDisconnected && BleedingRate < 1f && BloodRegenerationPerSecond >= float.Epsilon && Limb.Health > 0.1f && GetAmountOfBlood() < 1f)
		{
			AddLiquid(Liquid.GetLiquid(Limb.BloodLiquidType), BloodRegenerationPerSecond * fixedDeltaTime);
		}
		if (canUnlockGlowingHumanAchievement && GetAmount(Liquid.GetLiquid("TRITIUM")) > float.Epsilon)
		{
			StatManager.UnlockAchievement("GLOWING_HUMAN");
		}
	}

	private void PumpBehaviour(float deltaTime)
	{
		if (IsPump)
		{
			if (Limb.PhysicalBehaviour.Charge > 0.5f && (double)UnityEngine.Random.value > 0.995 - (double)(Limb.PhysicalBehaviour.Charge / 500f))
			{
				IsPump = false;
			}
		}
		else if (WasInitiallyPumping && (double)UnityEngine.Random.value > 0.999 && Limb.PhysicalBehaviour.Charge > 0.001f && BloodFlow < 0.1f)
		{
			IsPump = true;
		}
		if (IsPump && Limb.NodeBehaviour.IsConnectedToRoot && !Limb.Person.Braindead)
		{
			BloodFlow = base.TotalLiquidAmount;
		}
		else
		{
			BloodFlow -= deltaTime / 20f;
		}
		BloodFlow = Mathf.Clamp(BloodFlow, 0f, base.TotalLiquidAmount);
	}

	private void HandleDamageEdgeCases()
	{
		float num = Mathf.Max(Limb.PhysicalBehaviour.BurnProgress, Limb.SkinMaterialHandler.AcidProgress);
		if (num > 0.8f && Limb.PhysicalBehaviour.Temperature > 0f)
		{
			Drain(Mathf.Lerp(0.02f, 0.1f, Utils.MapRange(0.8f, 1f, 0f, 1f, num)));
		}
	}

	private void HandlePenetrationBleeding()
	{
		foreach (PhysicalBehaviour.Penetration victimPenetration in Limb.PhysicalBehaviour.victimPenetrations)
		{
			if (victimPenetration.Active && victimPenetration.Stabber.StabCausesWound)
			{
				BleedingRate = Mathf.Max(BleedingRate, victimPenetration.GetCurrentDepth() * 4f);
			}
		}
	}

	private void HandleBleeding(float deltaTime)
	{
		if (BleedingRate > 0.05f && Limb.PhysicalBehaviour.Temperature > 0f)
		{
			if (BleedingRate < 1f)
			{
				BleedingRate -= Time.deltaTime * 0.05f;
			}
			Drain(deltaTime / Mathf.Lerp(60f, 20f, BloodFlow) * BleedingRate * 0.15f);
		}
	}

	public void CreateDismembermentBloodParticle()
	{
		if (!Limb.IsAndroid && Limb.HasJoint)
		{
			Transform transform = Limb.Joint.connectedBody.transform;
			Vector3 position = transform.TransformPoint(Limb.Joint.connectedAnchor);
			Vector3 up = Limb.transform.position - transform.position;
			GameObject obj = UnityEngine.Object.Instantiate(Limb.Person.BleedingParticlePrefab, position, Quaternion.identity, transform);
			obj.AddComponent<Optout>();
			obj.transform.up = up;
			BleedingParticleBehaviour component = obj.GetComponent<BleedingParticleBehaviour>();
			component.CirculationBehaviour = transform.GetComponent<CirculationBehaviour>();
			component.PushingTo = this;
			component.ShouldBecomeSmokeInWater = true;
			component.Laminarity = UnityEngine.Random.value;
		}
	}

	private void FlowStep()
	{
		CirculationBehaviour[] pushesTo = PushesTo;
		foreach (CirculationBehaviour circulationBehaviour in pushesTo)
		{
			if (!circulationBehaviour.Limb.PhysicalBehaviour.isDisintegrated && Limb.NodeBehaviour.IsConnectedTo(circulationBehaviour.Limb.NodeBehaviour) && circulationBehaviour.BloodFlow < BloodFlow && !circulationBehaviour.IsDisconnected)
			{
				circulationBehaviour.BloodFlow = Mathf.Lerp(circulationBehaviour.BloodFlow, BloodFlow, 0.5f);
			}
		}
	}

	private void DiffuseStep()
	{
		float rate = Time.fixedDeltaTime * 1.5f;
		if (diffuseOscillator)
		{
			for (int i = 0; i < Limb.ConnectedLimbs.Count; i++)
			{
				DiffuseToLimb(Limb.ConnectedLimbs[i], rate);
			}
		}
		else
		{
			for (int num = Limb.ConnectedLimbs.Count - 1; num >= 0; num--)
			{
				DiffuseToLimb(Limb.ConnectedLimbs[num], rate);
			}
		}
		diffuseOscillator = !diffuseOscillator;
	}

	private void DiffuseToLimb(LimbBehaviour other, float rate)
	{
		if (!other.PhysicalBehaviour.isDisintegrated && !(other == Limb) && Limb.NodeBehaviour.IsConnectedTo(other.NodeBehaviour))
		{
			CirculationBehaviour circulationBehaviour = other.CirculationBehaviour;
			if (!(circulationBehaviour.TotalLiquidAmount > base.TotalLiquidAmount))
			{
				TransferTo(rate, circulationBehaviour);
			}
		}
	}

	public void Shot(Shot shot)
	{
		if (!ImmuneToDamage && (!Limb.IsAndroid || !(shot.damage < 50f)))
		{
			shot.damage *= Limb.ShotDamageMultiplier;
			if (!Limb.IsZombie && UnityEngine.Random.value < Limb.PhysicalBehaviour.Properties.Softness + 0.001f)
			{
				bool flag = IsWorldPointInArteryRect(shot.point);
				BleedingRate += Mathf.Max(0.5f, shot.damage / 3.5f) * (float)((!flag) ? 1 : 2);
				CreateBleedingParticle(shot.point, shot.normal, flag ? 1 : 0);
			}
			if (!Limb.IsAndroid && !Limb.IsZombie && UnityEngine.Random.value > 0.2f && Limb.IsWorldPointInVitalPart(shot.point))
			{
				IsPump = false;
			}
			GunshotWoundCount++;
		}
	}

	public void ExitShot(Shot shot)
	{
		if (!ImmuneToDamage && !Limb.IsZombie && !Limb.IsAndroid)
		{
			shot.damage = Limb.ShotDamageMultiplier;
			bool flag = IsWorldPointInArteryRect(shot.point);
			BleedingRate += Mathf.Max(0.5f, shot.damage / 10f) * (float)((!flag) ? 1 : 4);
			CreateBleedingParticle(shot.point, shot.normal, flag ? 1 : 0);
			if (!Limb.IsAndroid && UnityEngine.Random.value > 0.2f && Limb.IsWorldPointInVitalPart(shot.point))
			{
				IsPump = false;
			}
		}
	}

	public void HealBleeding()
	{
		foreach (GameObject bleedingParticle in BleedingParticles)
		{
			UnityEngine.Object.Destroy(bleedingParticle);
		}
		BleedingRate = 0f;
		BleedingParticles.Clear();
		BleedingPointCount = 0;
		InternalBleedingIntensity = 0f;
	}

	public void CreateBleedingParticle(Vector2 worldPosition, Vector2 direction, float laminarity = 0f, bool makeSound = false)
	{
		if (!Limb.IsAndroid && !Limb.IsZombie && BleedingPointCount < 8)
		{
			worldPosition = Limb.PhysicalBehaviour.spriteRenderer.bounds.ClosestPoint(worldPosition);
			GameObject gameObject = UnityEngine.Object.Instantiate(Limb.Person.BleedingParticlePrefab, worldPosition - direction * 0.1f, Quaternion.identity, base.transform);
			gameObject.transform.up = direction;
			BleedingParticleBehaviour component = gameObject.GetComponent<BleedingParticleBehaviour>();
			component.ShouldBecomeSmokeInWater = BleedingPointCount == 0;
			component.CirculationBehaviour = this;
			component.Laminarity = laminarity;
			if (!makeSound)
			{
				component.DripSounds = null;
			}
			BleedingParticles.Add(gameObject);
			BleedingPointCount++;
		}
	}

	public void Stabbed(Stabbing stabbing)
	{
		if (!ImmuneToDamage && !Limb.IsZombie && stabbing.stabber.StabCausesWound)
		{
			StabWoundCount++;
			InternalBleedingIntensity += 0.1f;
			if (IsPump && UnityEngine.Random.value < 0.6f && Limb.IsWorldPointInVitalPart(stabbing.point))
			{
				IsPump = false;
			}
		}
	}

	public void Unstabbed(Stabbing stabbing)
	{
		if (!ImmuneToDamage && stabbing.stabber.StabCausesWound)
		{
			bool flag = IsWorldPointInArteryRect(stabbing.point);
			BleedingRate += ((!flag) ? 1 : 4);
			CreateBleedingParticle(stabbing.point, stabbing.normal, flag ? 1 : 0);
		}
	}

	public void Cut(Vector2 point, Vector2 direction)
	{
		if (!ImmuneToDamage)
		{
			bool flag = IsWorldPointInArteryRect(point);
			BleedingRate += (flag ? 1f : 0.25f);
			CreateBleedingParticle(point, direction, flag ? 1 : 0);
		}
	}

	public void ActOnJointBreak2D(Joint2D joint)
	{
		if (!(joint != Limb.Joint))
		{
			BleedingRate += 10f;
			if ((bool)Source && Limb.NodeBehaviour.IsConnectedTo(Source.Limb.NodeBehaviour))
			{
				Source.BleedingRate += 10f;
			}
			IsDisconnected = true;
		}
	}

	public bool IsWorldPointInArteryRect(Vector2 worldPoint, float mindistance = 4f / 35f)
	{
		if (LocalArteryRects == null)
		{
			return false;
		}
		float num = mindistance * mindistance;
		Vector3 vector = base.transform.InverseTransformPoint(worldPoint);
		for (int i = 0; i < LocalArteryRects.Length; i++)
		{
			if ((LocalArteryRects[i].ClosestPoint(vector) - vector).sqrMagnitude <= num)
			{
				return true;
			}
		}
		return false;
	}

	public void Disintegrate()
	{
		Limb.BreakingThreshold = 0f;
		BleedingRate += 20f;
		CirculationBehaviour[] pushesTo = PushesTo;
		foreach (CirculationBehaviour circulationBehaviour in pushesTo)
		{
			if (Limb.NodeBehaviour.IsConnectedTo(circulationBehaviour.Limb.NodeBehaviour) && !circulationBehaviour.Limb.PhysicalBehaviour.isDisintegrated)
			{
				circulationBehaviour.BleedingRate += 20f;
			}
		}
		if ((bool)Source && Limb.NodeBehaviour.IsConnectedTo(Source.Limb.NodeBehaviour))
		{
			Source.BleedingRate += 20f;
		}
		IsDisconnected = true;
	}

	protected override void OnLiquidEnter(Liquid type)
	{
		base.OnLiquidEnter(type);
		type.OnEnterLimb(Limb);
	}

	public void OnImpactCreated(GameObject gm)
	{
		if (gm.TryGetComponent<BloodImpactBehaviour>(out var component))
		{
			if (ScaledLiquidAmount > 0.05f)
			{
				component.SetColor(GetComputedColor(Limb.GetOriginalBloodType().Color));
			}
			else
			{
				component.SetColor(Limb.GetOriginalBloodType().Color);
			}
		}
	}
}

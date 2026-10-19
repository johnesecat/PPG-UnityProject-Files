using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ExplosiveBehaviour : MonoBehaviour, Messages.IShot, Messages.IOnFragmentHit, Messages.IUse
{
	public float Range = 27f;

	public float ShockwaveStrength = 1f;

	public float Delay = 2f;

	public uint FragmentationRayCount = 32u;

	public float FragmentForce = 2f;

	public float ShockwaveLiftForce;

	public bool ArmOnAwake;

	public bool DestroyOnExplode = true;

	public bool DisintegrateOnExplode;

	public bool ParticleEffectAndSound = true;

	public float BurnPower;

	public float TemperatureLimit = 500f;

	public bool BigExplosion;

	public bool ArmOnUse = true;

	public int BallisticShrapnelCount;

	public float AffectVictimTemperature;

	public float ShotExplodeChance = 1f;

	public bool ExplodesOnFragmentHit;

	[Tooltip("Negative means it doesnt")]
	public float ImpactForceThreshold = -1f;

	[Tooltip("More than 1 means it doesnt")]
	public float ExplodeBurnProgressThreshold = 2f;

	public float DismemberChance;

	public bool ShouldCreateKillzone = true;

	public float MinChargeToExplode = float.PositiveInfinity;

	[SkipSerialisation]
	public GameObject Killzone;

	[SkipSerialisation]
	public GameObject VFXOverride;

	[SkipSerialisation]
	public GameObject UnderwaterVFXOverride;

	private PhysicalBehaviour physicalBehaviour;

	private bool hasExploded;

	public bool CanExplodeMultipleTimes;

	[SkipSerialisation]
	public LayerMask BlockingLayers;

	public UnityEvent OnExplode;

	public UnityEvent OnArm;

	[HideInInspector]
	public string Tag;

	private static readonly RaycastHit2D[] losBuffer = new RaycastHit2D[8];

	[SkipSerialisation]
	public bool Armed { get; private set; }

	public void Use(ActivationPropagation _)
	{
		if (ArmOnUse)
		{
			Activate();
		}
	}

	public void Activate()
	{
		StartCoroutine(Explode(Delay));
	}

	[Obsolete]
	public void ForceImmediateExposion()
	{
		ForceImmediateExplosion();
	}

	public void ForceImmediateExplosion()
	{
		StopAllCoroutines();
		StartCoroutine(Explode(0f));
	}

	public void Disarm()
	{
		StopAllCoroutines();
		Armed = false;
	}

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		if ((int)BlockingLayers == 0)
		{
			BlockingLayers = LayerMask.GetMask("Objects", "CollidingDebris", "Bounds");
		}
	}

	private void Start()
	{
		if (ShouldCreateKillzone)
		{
			Killzone = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Killzone"), base.transform.position, Quaternion.identity, base.transform);
			SetRange(Range);
		}
		if (ArmOnAwake)
		{
			Activate();
		}
	}

	public void SetRange(float r)
	{
		if ((bool)Killzone)
		{
			Killzone.GetComponent<IgnoreParentSize>().DesiredSize = 2f * r * Vector2.one;
		}
	}

	private IEnumerator Explode(float delay, bool forceDoubleCheck = false)
	{
		if ((!CanExplodeMultipleTimes && hasExploded) || (forceDoubleCheck && hasExploded))
		{
			yield break;
		}
		Armed = true;
		OnArm?.Invoke();
		yield return new WaitForSeconds(delay);
		yield return new WaitForEndOfFrame();
		OnExplode?.Invoke();
		if (DestroyOnExplode)
		{
			Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		if (TryGetComponent<LimbBehaviour>(out var _))
		{
			StatManager.UnlockAchievement("INSIDES_HURT");
		}
		bool flag = (bool)physicalBehaviour && physicalBehaviour.IsUnderWater;
		flag |= WaterBehaviour.IsPointUnderWater(base.transform.position);
		ExplosionCreator.ExplosionParameters explosionParameters = default(ExplosionCreator.ExplosionParameters);
		explosionParameters.FragmentationRayCount = FragmentationRayCount;
		explosionParameters.Position = base.transform.position;
		explosionParameters.Range = Range;
		explosionParameters.FragmentForce = FragmentForce;
		explosionParameters.CreateParticlesAndSound = ParticleEffectAndSound;
		explosionParameters.DismemberChance = DismemberChance;
		explosionParameters.LargeExplosionParticles = BigExplosion;
		explosionParameters.BallisticShrapnelCount = BallisticShrapnelCount;
		ExplosionCreator.ExplosionParameters ex = explosionParameters;
		ExplosionCreator.CreateExplosionWithWater(flag, ex);
		if (BurnPower > 0.001f || ShockwaveLiftForce > 0.01f || Mathf.Abs(AffectVictimTemperature) > 1f)
		{
			Collider2D[] componentsInChildren = Physics2D.OverlapCircleAll(base.transform.position, Range, BlockingLayers);
			foreach (Collider2D collider2D in componentsInChildren)
			{
				if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value) && Physics2D.LinecastNonAlloc(collider2D.transform.position, base.transform.position, losBuffer, BlockingLayers) <= 0)
				{
					value.rigidbody.AddForce(value.rigidbody.mass * ShockwaveLiftForce * Vector2.up);
					float num = BurnPower / 8f * UnityEngine.Random.value;
					if (value.BurnProgress < num)
					{
						value.BurnProgress = num;
					}
					if (UnityEngine.Random.value < BurnPower)
					{
						value.Ignite(ignoreFlammability: true, ex.Position);
					}
					if ((AffectVictimTemperature > 0f && value.Temperature < AffectVictimTemperature) || (AffectVictimTemperature < 0f && value.Temperature > AffectVictimTemperature))
					{
						value.Temperature = Mathf.Lerp(value.Temperature, AffectVictimTemperature, 0.035f / value.GetHeatCapacity());
					}
				}
			}
		}
		if (TryGetComponent<BloodContainer>(out var component2))
		{
			component2.ClearLiquid();
		}
		if (!flag && (bool)VFXOverride)
		{
			UnityEngine.Object.Instantiate(VFXOverride, base.transform.position, Quaternion.identity);
		}
		DestroyableBehaviour component6;
		if (DestroyOnExplode)
		{
			if (TryGetComponent<LimbBehaviour>(out var component3))
			{
				component3.Crush();
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else if (DisintegrateOnExplode)
		{
			PhysicalBehaviour component5;
			if (TryGetComponent<LimbBehaviour>(out var component4))
			{
				component4.Crush();
			}
			else if (TryGetComponent<PhysicalBehaviour>(out component5) && component5.Disintegratable && !component5.isDisintegrated)
			{
				component5.Disintegrate();
			}
			if ((bool)Killzone)
			{
				UnityEngine.Object.Destroy(Killzone);
			}
		}
		else if (TryGetComponent<DestroyableBehaviour>(out component6))
		{
			component6.Break(Vector2.zero);
		}
		hasExploded = true;
	}

	private void FixedUpdate()
	{
		if ((bool)physicalBehaviour && (physicalBehaviour.Temperature >= TemperatureLimit || (ExplodeBurnProgressThreshold < 1f && physicalBehaviour.BurnProgress > ExplodeBurnProgressThreshold) || physicalBehaviour.Charge > MinChargeToExplode))
		{
			StartCoroutine(Explode(0f));
		}
	}

	public void Shot(Shot shot)
	{
		if (shot.triggersExplosiveOverride && UnityEngine.Random.value < ShotExplodeChance)
		{
			StartCoroutine(Explode(UnityEngine.Random.value * 0.2f, forceDoubleCheck: true));
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!(ImpactForceThreshold < 0f) && collision.GetContact(0).normalImpulse > ImpactForceThreshold)
		{
			StartCoroutine(Explode(0.05f));
		}
	}

	public void OnFragmentHit(float force)
	{
		if (ExplodesOnFragmentHit && !Armed)
		{
			StartCoroutine(Explode(0.05f, forceDoubleCheck: true));
		}
	}
}

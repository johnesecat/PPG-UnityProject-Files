using System;
using System.Collections.Generic;
using Ceras;
using NaughtyAttributes;
using UnityEngine;

public class SkinMaterialHandler : MonoBehaviour, Messages.IStabbed, Messages.IOnAfterDeserialise
{
	private Material material;

	[SkipSerialisation]
	[ReadOnly]
	public SpriteRenderer renderer;

	[SkipSerialisation]
	[ReadOnly]
	public LimbBehaviour limb;

	[SkipSerialisation]
	public bool BluntDamageRegen = true;

	[SkipSerialisation]
	public bool GunshotDamageRegen = true;

	[SkipSerialisation]
	public bool BurnWoundRegen = true;

	[SkipSerialisation]
	public bool TrackWoundAge = true;

	[SkipSerialisation]
	public SkinMaterialHandler[] adjacentLimbs;

	public bool ShouldRecolourWhenCold;

	public bool ShouldGlowWhenHot;

	public int MaxGlowTemperature = 1000;

	public int MinFreezeTemperature = -30;

	private const int MaxDamagePointCount = 128;

	[HideInInspector]
	public Vector4[] damagePoints;

	[HideInInspector]
	public float[] damagePointTimeStamps;

	[HideInInspector]
	public int currentDamagePointCount;

	public float intensityMultiplier = 1f;

	[SerializeField]
	[Include]
	[ReadOnly]
	private float rottenProgress;

	[NonSerialized]
	private float oldRottenProgress;

	[SerializeField]
	[Include]
	[ReadOnly]
	private float acidProgress;

	[NonSerialized]
	private float oldAcidProgress;

	[SkipSerialisation]
	[HideInInspector]
	public bool ShouldRot = true;

	private readonly FixedIntervalDistributor intervalDistributor = new FixedIntervalDistributor
	{
		RateHz = 16f
	};

	[SkipSerialisation]
	[HideInInspector]
	[Obsolete]
	public Vector4[] bulletHolePoints = Array.Empty<Vector4>();

	private static readonly Collider2D[] colliderBuffer = new Collider2D[16];

	[SkipSerialisation]
	public bool IsSkinVisible
	{
		get
		{
			if (acidProgress < 0.62f)
			{
				return limb.PhysicalBehaviour.BurnProgress < 0.45f;
			}
			return false;
		}
	}

	[SkipSerialisation]
	public bool IsFleshVisible
	{
		get
		{
			if (acidProgress < 0.85f)
			{
				return limb.PhysicalBehaviour.BurnProgress < 0.78f;
			}
			return false;
		}
	}

	public float RottenProgress
	{
		get
		{
			return rottenProgress;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if ((bool)material && Mathf.Abs(oldRottenProgress - num) > 0.01f)
			{
				material.SetFloat(ShaderProperties.Get("_RottenProgress"), num);
				oldRottenProgress = num;
			}
			rottenProgress = num;
		}
	}

	public float AcidProgress
	{
		get
		{
			return acidProgress;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if ((bool)material && Mathf.Abs(oldAcidProgress - num) > 0.01f)
			{
				material.SetFloat(ShaderProperties.Get("_AcidProgress"), num);
				oldAcidProgress = acidProgress;
			}
			acidProgress = num;
		}
	}

	private void Awake()
	{
		renderer = GetComponent<SpriteRenderer>();
		limb = GetComponent<LimbBehaviour>();
		if (limb.Person.ChosenMaterial != null)
		{
			renderer.material = limb.Person.ChosenMaterial;
		}
		material = renderer.material;
		damagePointTimeStamps = new float[128];
		damagePoints = new Vector4[128];
		float time = Time.time;
		for (int i = 0; i < 128; i++)
		{
			damagePointTimeStamps[i] = time;
			damagePoints[i] = DamagePoint.None;
		}
	}

	public void OnAfterDeserialise(List<GameObject> gameObjects)
	{
		if (damagePoints.Length != 128)
		{
			damagePoints = new Vector4[128];
			for (int i = 0; i < 128; i++)
			{
				damagePoints[i] = DamagePoint.None;
			}
		}
	}

	private void Start()
	{
		Sync();
	}

	public void AddDamagePoint(DamageType type, Vector2 globalPosition, float intensity)
	{
		if (currentDamagePointCount < 128 && !(intensity <= float.Epsilon))
		{
			intensity *= intensityMultiplier;
			InternalAddDamagePoint(type, globalPosition, intensity);
			SkinMaterialHandler[] array = adjacentLimbs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].InternalAddDamagePoint(type, globalPosition, intensity);
			}
		}
	}

	private void InternalAddDamagePoint(DamageType type, Vector2 globalPosition, float intensity)
	{
		if (currentDamagePointCount < 128 && !(intensity <= float.Epsilon))
		{
			intensity = Mathf.Clamp(intensity, 0f, 100f);
			Vector3 localPos = base.transform.InverseTransformPoint(globalPosition);
			DamagePoint damagePoint = new DamagePoint(localPos.x, localPos.y, intensity * 0.1f, type);
			float minDistance = 0f;
			switch (type)
			{
			case DamageType.Blunt:
				minDistance = 0.1f * damagePoint.Intensity;
				break;
			case DamageType.Bullet:
				minDistance = 0.01f * Mathf.Min(damagePoint.Intensity, 5f);
				break;
			case DamageType.Stab:
				minDistance = 0.02f * damagePoint.Intensity;
				break;
			case DamageType.Dismemberment:
				minDistance = 0.05f;
				break;
			case DamageType.Burn:
				minDistance = 0.015f;
				break;
			}
			if (GetNearEntry(localPos, minDistance, type, out var index))
			{
				damagePoints[index].z = Mathf.Max(damagePoints[index].z, damagePoint.Intensity);
			}
			else
			{
				damagePointTimeStamps[currentDamagePointCount] = Time.time;
				damagePoints[currentDamagePointCount] = damagePoint;
				currentDamagePointCount++;
			}
			Sync();
		}
	}

	[Obsolete]
	public void ImpactDamage(float intensity, Vector3 globalPosition, bool fromAdjacent = false)
	{
		AddDamagePoint(DamageType.Blunt, globalPosition, intensity);
	}

	[Obsolete]
	public void ShotDamage(float intensity, Vector3 globalPosition, bool fromAdjacent = false)
	{
		AddDamagePoint(DamageType.Bullet, globalPosition, intensity);
	}

	private bool GetNearEntry(Vector3 localPos, float minDistance, DamageType type, out int index)
	{
		float num = minDistance * minDistance;
		for (index = 0; index < currentDamagePointCount; index++)
		{
			DamagePoint damagePoint = damagePoints[index];
			if ((damagePoint.Type == type || damagePoint.Intensity <= 0.01f) && (localPos - new Vector3(damagePoint.X, damagePoint.Y)).sqrMagnitude <= num)
			{
				damagePoints[index].w = (float)type;
				return true;
			}
		}
		return false;
	}

	public void Sync()
	{
		material.SetVectorArray(ShaderProperties.Get("_DamagePoints"), damagePoints);
		material.SetInt(ShaderProperties.Get("_DamagePointCount"), currentDamagePointCount);
		if (TrackWoundAge)
		{
			material.SetFloatArray(ShaderProperties.Get("_DamagePointTimeStamp"), damagePointTimeStamps);
		}
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(material);
	}

	public void ClearAllDamage()
	{
		RottenProgress = 0f;
		AcidProgress = 0f;
		for (int i = 0; i < damagePoints.Length; i++)
		{
			damagePoints[i] = DamagePoint.None;
		}
		Sync();
	}

	public void Stabbed(Stabbing stabbing)
	{
		if (stabbing.stabber.StabCausesWound)
		{
			AddDamagePoint(DamageType.Stab, stabbing.point, 8f * stabbing.stabber.StabWoundSizeMultiplier);
		}
	}

	private void Update()
	{
		for (int i = 0; i < intervalDistributor.CalculateCycleCount(Time.deltaTime); i++)
		{
			if (!limb.IsZombie && UserPreferenceManager.Current.AutoHealWounds && limb.IsConsideredAlive && RegenerateWounds(intervalDistributor.IntervalSeconds))
			{
				Sync();
			}
			for (int j = 0; j < limb.PhysicalBehaviour.victimPenetrations.Count; j++)
			{
				PhysicalBehaviour.Penetration penetration = limb.PhysicalBehaviour.victimPenetrations[j];
				if (!penetration.Active || !penetration.Stabber || !penetration.Stabber.StabCausesWound || penetration.Stabber.isDisintegrated)
				{
					continue;
				}
				Vector2 sharpEnd = penetration.Stabber.GetSharpEnd(penetration.Axis);
				bool flag = true;
				for (int k = 0; k < Physics2D.OverlapPointNonAlloc(sharpEnd, colliderBuffer); k++)
				{
					if (colliderBuffer[k].gameObject == penetration.Victim.gameObject)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					Vector3 vector = renderer.bounds.ClosestPoint(sharpEnd);
					AddDamagePoint(DamageType.Stab, vector, 8f * penetration.Stabber.StabWoundSizeMultiplier);
				}
			}
		}
	}

	private bool RegenerateWounds(float deltaTime)
	{
		float num = Mathf.Pow(0.95f, deltaTime);
		float num2 = Mathf.Pow(0.995f, deltaTime);
		float num3 = Mathf.Pow(0.97f, deltaTime);
		bool result = false;
		bool flag = GunshotDamageRegen && limb.CirculationBehaviour.BleedingRate < 1f && limb.Health > limb.InitialHealth / 2f;
		for (int i = 0; i < 128; i++)
		{
			DamagePoint damagePoint = damagePoints[i];
			switch (damagePoint.Type)
			{
			case DamageType.Blunt:
				if (BluntDamageRegen && damagePoint.Intensity > 0.001f)
				{
					damagePoints[i].z *= num;
					result = true;
				}
				break;
			case DamageType.Bullet:
			case DamageType.Stab:
				if (flag && damagePoint.Intensity > 0.001f)
				{
					damagePoints[i].z *= num2;
					result = true;
				}
				break;
			case DamageType.Burn:
				if (BurnWoundRegen && damagePoint.Intensity > 0.001f)
				{
					damagePoints[i].z *= num3;
					result = true;
				}
				break;
			}
		}
		return result;
	}

	private void OnWillRenderObject()
	{
		material.SetFloat(ShaderProperties.Get("_ElectrocutionIntensity"), limb.PhysicalBehaviour.Charge / 25f);
		if (limb.PhysicalBehaviour.Temperature > 0f)
		{
			if (ShouldGlowWhenHot)
			{
				float num = Mathf.Clamp01(limb.PhysicalBehaviour.Temperature / (float)MaxGlowTemperature);
				material.SetFloat(ShaderProperties.Get("_Temperature"), num * num);
			}
			else
			{
				material.SetFloat(ShaderProperties.Get("_Temperature"), 0f);
			}
		}
		else if (ShouldRecolourWhenCold)
		{
			float value = Mathf.Clamp(Utils.MapRange(MinFreezeTemperature, 0f, -1f, 0f, limb.PhysicalBehaviour.Temperature), -1f, 0f);
			material.SetFloat(ShaderProperties.Get("_Temperature"), value);
		}
		else
		{
			material.SetFloat(ShaderProperties.Get("_Temperature"), 0f);
		}
		material.SetFloat(ShaderProperties.Get("_BurnProgress"), limb.PhysicalBehaviour.BurnProgress);
	}

	private void LateUpdate()
	{
		if (!(limb.SpeciesIdentity == "Android") && ShouldRot && !(limb.PhysicalBehaviour.Temperature < -50f))
		{
			float num = 0f;
			if (limb.Health < 0.05f)
			{
				num += Time.deltaTime * 0.002f;
			}
			if (limb.IsZombie)
			{
				num += Time.deltaTime * 0.02f;
			}
			RottenProgress += num;
		}
	}
}

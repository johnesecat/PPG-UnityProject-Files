using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class DamagableMachineryBehaviour : MonoBehaviour, Messages.IShot, Messages.ISlice, Messages.IOnFragmentHit, Messages.IBreak, Messages.IStabbed, Messages.IRepair
{
	public float FirearmDamageMultiplier;

	public float ExplosionDamageMultiplier;

	public float StabDamageMultiplier;

	public float BreakDamage;

	public bool ExplodesOnBreak;

	public bool Waterproof = true;

	public bool IndestructibilityCanBeToggled = true;

	[ShowIf("ExplodesOnBreak")]
	[Obsolete]
	public float FragmentForce = 0.02f;

	[ShowIf("ExplodesOnBreak")]
	[Obsolete]
	public float ExplosionRange = 5f;

	[SkipSerialisation]
	[ShowIf("ExplodesOnBreak")]
	public ExplosionCreator.ExplosionParameters Explosion;

	public float TemperatureThreshold = float.PositiveInfinity;

	[Range(0f, 1f)]
	public float FireDamageThreshold = 1f;

	[Space]
	public float Health = 10f;

	private float initialHealth;

	public bool CanRepair = true;

	[ReadOnly]
	public bool Destroyed;

	public bool Indestructible;

	[Space]
	[SkipSerialisation]
	public GameObject ExplosionPrefab;

	[HideInInspector]
	[SkipSerialisation]
	public Vector4 RandomOffset;

	[SkipSerialisation]
	[HideInInspector]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	private SpriteRenderer SpriteRenderer;

	[SkipSerialisation]
	public GameObject SparkPrefab;

	[SkipSerialisation]
	public MonoBehaviour[] BehavioursToToggle;

	[SkipSerialisation]
	public GameObject[] GameObjectsToToggle;

	[SkipSerialisation]
	public UnityEvent OnDamaged;

	[SkipSerialisation]
	public UnityEvent OnRepaired;

	[SkipSerialisation]
	private Material originalMaterial;

	[SkipSerialisation]
	private Material brokenMachineMaterial;

	[SkipSerialisation]
	private MaterialPropertyBlock propertyBlock;

	public bool HeatShielded;

	public const float SparkChancePerSecond = 4E-05f;

	private float t;

	private void Awake()
	{
		initialHealth = Health;
		RandomOffset = new Vector4(UnityEngine.Random.value * 1000f, UnityEngine.Random.value * 1000f);
		SpriteRenderer = GetComponent<SpriteRenderer>();
		PhysicalBehaviour = GetComponent<PhysicalBehaviour>();
		originalMaterial = SpriteRenderer.sharedMaterial;
		brokenMachineMaterial = Resources.Load<Material>("Materials/BrokenMachine");
		if (!SparkPrefab)
		{
			SparkPrefab = Resources.Load<GameObject>("Prefabs/BrokenElectronicsSpark");
		}
	}

	private void Start()
	{
		propertyBlock = new MaterialPropertyBlock();
		SpriteRenderer.GetPropertyBlock(propertyBlock);
		if (Destroyed)
		{
			SetAsBroken();
		}
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton item = new ContextMenuButton("repairMachinery", "Repair", "Repair broken machinery", delegate
		{
			Health = initialHealth;
			ActOnRepair();
		})
		{
			Condition = () => Destroyed && CanRepair
		};
		buttons.Add(item);
		List<ContextMenuButton> buttons2 = PhysicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton("shieldFromHeat", "Disable heat damage", "Disable machine heat damage", delegate
		{
			HeatShielded = true;
		})
		{
			Condition = () => !Indestructible && !HeatShielded && !float.IsPositiveInfinity(TemperatureThreshold)
		};
		buttons2.Add(item);
		List<ContextMenuButton> buttons3 = PhysicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton("unshieldFromHeat", "Enable heat damage", "Enable machine heat damage", delegate
		{
			HeatShielded = false;
		})
		{
			Condition = () => !Indestructible && HeatShielded && !float.IsPositiveInfinity(TemperatureThreshold)
		};
		buttons3.Add(item);
		List<ContextMenuButton> buttons4 = PhysicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton("machineryIndestructible", () => (!Indestructible) ? "Make indestructible" : "Make destructible", "Toggle indestructibility", delegate
		{
			Indestructible = !Indestructible;
			if (Indestructible)
			{
				Health = initialHealth;
				ActOnRepair();
			}
		})
		{
			Condition = () => IndestructibilityCanBeToggled
		};
		buttons4.Add(item);
	}

	public void Shot(Shot shot)
	{
		if (!Indestructible && !Destroyed)
		{
			Health -= shot.damage * FirearmDamageMultiplier;
			ActOnDamage();
		}
	}

	public void Stabbed(Stabbing stabbing)
	{
		if (!Indestructible && !Destroyed && stabbing.stabber.StabCausesWound)
		{
			Health -= StabDamageMultiplier;
			ActOnDamage();
		}
	}

	public void OnFragmentHit(float force)
	{
		if (!Indestructible)
		{
			Health -= force * ExplosionDamageMultiplier * 250f;
			ActOnDamage();
		}
	}

	public void Break(Vector2 velocity)
	{
		if (!Indestructible && !Destroyed)
		{
			Health -= BreakDamage;
			ActOnDamage();
		}
	}

	public void OnEMPHit()
	{
		if (!Indestructible && !Destroyed)
		{
			SetAsBroken();
			UnityEngine.Object.Instantiate(SparkPrefab, base.transform.position, Quaternion.identity);
		}
	}

	public void BreakPermanently()
	{
		if (!Indestructible && !Destroyed)
		{
			OnEMPHit();
			CanRepair = false;
		}
	}

	public void Slice()
	{
		if (!Indestructible && !Destroyed)
		{
			Health = 0f;
			ActOnDamage();
		}
	}

	private void FixedUpdate()
	{
		if (!Indestructible && !Destroyed)
		{
			if (!HeatShielded && (PhysicalBehaviour.BurnProgress > FireDamageThreshold || PhysicalBehaviour.Temperature > TemperatureThreshold))
			{
				Health = 0f;
				ActOnDamage();
			}
			if (!Waterproof && (double)PhysicalBehaviour.Wetness > 0.95 && (double)UnityEngine.Random.value > 0.9)
			{
				Health = 0f;
				ActOnDamage();
			}
		}
	}

	private void Update()
	{
		t += Time.deltaTime;
		if (Destroyed && PhysicalBehaviour.LocalColliderGridPoints.Length != 0 && t > 1f && (bool)SparkPrefab && UnityEngine.Random.value < 4E-05f)
		{
			t = 0f;
			Vector2 vector = PhysicalBehaviour.LocalColliderGridPoints[UnityEngine.Random.Range(0, PhysicalBehaviour.LocalColliderGridPoints.Length)];
			UnityEngine.Object.Instantiate(SparkPrefab, base.transform.TransformPoint(vector), Quaternion.identity);
		}
	}

	public void ForceBreak()
	{
		if (!Indestructible && !Destroyed)
		{
			Health = -1f;
			ActOnDamage();
		}
	}

	private void ActOnDamage()
	{
		if (!(Health > 0f) && !Destroyed)
		{
			SetAsBroken();
			if (ExplodesOnBreak)
			{
				Explosion.Position = base.transform.position;
				ExplosionCreator.CreateExplosionWithWater(PhysicalBehaviour.IsUnderWater, Explosion);
			}
			if ((bool)ExplosionPrefab)
			{
				UnityEngine.Object.Instantiate(ExplosionPrefab, base.transform.position, Quaternion.identity);
			}
		}
	}

	private void SetAsBroken()
	{
		Destroyed = true;
		Health = 0f;
		MonoBehaviour[] behavioursToToggle = BehavioursToToggle;
		foreach (MonoBehaviour monoBehaviour in behavioursToToggle)
		{
			if ((bool)monoBehaviour)
			{
				monoBehaviour.enabled = false;
			}
		}
		GameObject[] gameObjectsToToggle = GameObjectsToToggle;
		foreach (GameObject gameObject in gameObjectsToToggle)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(value: false);
			}
		}
		SpriteRenderer.sharedMaterial = brokenMachineMaterial;
		propertyBlock.SetTexture(ShaderProperties.Get("_MainTex"), SpriteRenderer.sprite.texture);
		propertyBlock.SetVector(ShaderProperties.Get("_Offset"), RandomOffset);
		SpriteRenderer.SetPropertyBlock(propertyBlock);
		OnDamaged?.Invoke();
	}

	public void Repair()
	{
		if (CanRepair)
		{
			PhysicalBehaviour.Temperature = Mathf.Min(PhysicalBehaviour.Temperature, TemperatureThreshold - 1f);
			Health += initialHealth / 4f;
			Health = Mathf.Clamp(Health, 0f, initialHealth);
			ActOnRepair();
		}
	}

	private void ActOnRepair()
	{
		if (!(Health <= 0.001f))
		{
			PhysicalBehaviour.BurnProgress = 0f;
			Destroyed = false;
			SpriteRenderer.sharedMaterial = originalMaterial;
			MonoBehaviour[] behavioursToToggle = BehavioursToToggle;
			for (int i = 0; i < behavioursToToggle.Length; i++)
			{
				behavioursToToggle[i].enabled = true;
			}
			GameObject[] gameObjectsToToggle = GameObjectsToToggle;
			for (int i = 0; i < gameObjectsToToggle.Length; i++)
			{
				gameObjectsToToggle[i].SetActive(value: true);
			}
			OnRepaired?.Invoke();
		}
	}
}

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SkinMaterialHandler), typeof(CirculationBehaviour))]
public class LimbBehaviour : MonoBehaviour, Messages.IShot, Messages.IExitShot, Messages.ISlice, Messages.IDamage, Messages.IOnEMPHit, Messages.IWaterImpact, IManagedBehaviour
{
	[Flags]
	public enum ShatterFlags : byte
	{
		None = 0x0,
		Bone = 0x1,
		Flesh = 0x2,
		Skin = 0x4,
		All = byte.MaxValue
	}

	public enum BodyPart
	{
		Head,
		Torso,
		Legs,
		Arms
	}

	[SkipSerialisation]
	public ConnectedNodeBehaviour NodeBehaviour;

	[SkipSerialisation]
	public BodyPart RoughClassification;

	[SkipSerialisation]
	[HideInInspector]
	public SkinMaterialHandler SkinMaterialHandler;

	[SkipSerialisation]
	[HideInInspector]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	[HideInInspector]
	public CirculationBehaviour CirculationBehaviour;

	[SkipSerialisation]
	[HideInInspector]
	public PersonBehaviour Person;

	[SkipSerialisation]
	[HideInInspector]
	public HingeJoint2D Joint;

	[SkipSerialisation]
	[HideInInspector]
	public Collider2D Collider;

	[SkipSerialisation]
	public Bounds[] VitalParts;

	[SkipSerialisation]
	public ParticleSystem KillShotParticles;

	[SkipSerialisation]
	public float GForcePassoutThreshold = 10f;

	[SkipSerialisation]
	public float GForceDamageThreshold = 10f;

	[HideInInspector]
	public bool KillShotParticlesEmitted;

	[SkipSerialisation]
	[HideInInspector]
	public GripBehaviour GripBehaviour;

	private GameObject myStatus;

	[Header("Settings")]
	[SkipSerialisation]
	public List<LimbBehaviour> ConnectedLimbs;

	[Space]
	public float Vitality;

	[HideInInspector]
	public float InitialHealth;

	public bool HasBrain;

	public float FreezingTemperature = -25f;

	public float DiscomfortingHeatTemperature = 40f;

	public float RegenerationSpeed;

	public float ImpactPainMultiplier;

	public float ImpactDamageMultiplier = 1f;

	public float BreakingThreshold;

	public float BaseStrength = 5f;

	public float FakeUprightForce;

	public float BodyTemperature = 37f;

	public float ShotDamageMultiplier = 1f;

	public bool DoStumble;

	public bool IsLethalToBreak;

	public bool DoBalanceJerk;

	[Obsolete]
	public float ExplodeLiquidAmount = 5.357143f;

	[SkipSerialisation]
	public DecalDescriptor BloodDecal;

	private Color color;

	public float BloodMuscleStrengthRatio = 1f;

	[ShowIf("DoBalanceJerk")]
	public float BalanceMuscleMovement = 1f;

	[Header("Status")]
	public float Health;

	[ReadOnly]
	public float Numbness;

	public bool IsAndroid;

	[ReadOnly]
	public bool IsZombie;

	[ReadOnly]
	public bool HasJoint;

	[ReadOnly]
	public bool Broken;

	[ReadOnly]
	public bool Frozen;

	[ReadOnly]
	public bool IsDismembered;

	private GoreStringBehaviour goreString;

	[SkipSerialisation]
	public string SpeciesIdentity = "Human";

	[SkipSerialisation]
	public string BloodLiquidType = "BLOOD";

	[SkipSerialisation]
	public LimbBehaviour NearestLimbToBrain;

	[SkipSerialisation]
	public int DistanceToBrain;

	[SkipSerialisation]
	[Obsolete]
	[HideInInspector]
	public float InternalExternalTemperatureTransferRate = 0.01f;

	[SkipSerialisation]
	public float InternalToExternalTempTransferRate = 0.002f;

	[SkipSerialisation]
	public float ExternalToInternalTempTransferRate = 0.005f;

	[HideInInspector]
	public float InternalTemperature;

	[HideInInspector]
	public bool IsActiveInCurrentPose;

	private Vector2 previousVelocity;

	[SkipSerialisation]
	public float FrictionBurnWoundMinSpeedSqrd = 62f;

	[SkipSerialisation]
	public float BodyHeatProductionFactor = 1f;

	private float stumbleTime;

	private static readonly ContactPoint2D[] contactBuffer = new ContactPoint2D[8];

	[HideInInspector]
	[SkipSerialisation]
	public float randomOffset;

	[HideInInspector]
	public ushort BruiseCount;

	public const float DrownTimeInSeconds = 40f;

	[SkipSerialisation]
	public GameObject LimbStatus;

	[SkipSerialisation]
	public ShatteredObjectSpriteInitialiser ShatteredObjectGenerators;

	public float ShatteredObjectChance = 0.1f;

	public ShatterFlags CurrentlyShattered;

	public bool ImmuneToDamage;

	private float previousHealth;

	private ObjectCreationAction undoAction;

	public bool HasLungs;

	public bool LungsPunctured;

	private float shotHeat;

	private Vector2 originalJointLimits;

	private GForceMeasureBehaviour gforce;

	[SkipSerialisation]
	public float RegenerateBurnProgressSpeed = 1f;

	[HideInInspector]
	public List<GameObject> DestroyWith = new List<GameObject>();

	[HideInInspector]
	public float[] StrengthMultipliers;

	[SkipSerialisation]
	public bool IsOnFloor { get; set; }

	[SkipSerialisation]
	public bool IsParalysed
	{
		get
		{
			if (!IsAndroid && !IsZombie && (bool)NearestLimbToBrain)
			{
				if (NearestLimbToBrain.NodeBehaviour.IsConnectedToRoot)
				{
					if (!NearestLimbToBrain.Broken || NearestLimbToBrain.RoughClassification != BodyPart.Torso)
					{
						return NearestLimbToBrain.IsParalysed;
					}
					return true;
				}
				return false;
			}
			return false;
		}
	}

	[SkipSerialisation]
	public float JointStress { get; private set; }

	[SkipSerialisation]
	public Vector2 OriginalJointLimits
	{
		get
		{
			return originalJointLimits;
		}
		set
		{
			originalJointLimits = value;
		}
	}

	[SkipSerialisation]
	public bool IsCapable
	{
		get
		{
			if (Frozen || PhysicalBehaviour.Temperature <= FreezingTemperature)
			{
				return false;
			}
			if (NodeBehaviour.IsConnectedToRoot && Health > 1f && Person.Consciousness > 0.8f && Person.ShockLevel < 0.5f && Person.PainLevel < 0.5f && Numbness < 0.9f && CirculationBehaviour.InternalBleedingIntensity < 0.5f && CirculationBehaviour.HasCirculation && !Broken)
			{
				return CirculationBehaviour.BloodFlow > 0.25f;
			}
			return false;
		}
	}

	[SkipSerialisation]
	public bool IsConsideredAlive
	{
		get
		{
			if (NodeBehaviour.IsConnectedToRoot && CirculationBehaviour.BloodFlow > 0.25f)
			{
				return Health > 0.01f;
			}
			return false;
		}
	}

	[SkipSerialisation]
	public float MotorStrength
	{
		get
		{
			if (Person.Consciousness < Mathf.Max(0.3f, 0.8f - Mathf.Clamp01(Person.AdrenalineLevel)))
			{
				return 0f;
			}
			Health = Mathf.Max(0f, Health);
			Numbness = Mathf.Clamp01(Numbness + Mathf.Min(CirculationBehaviour.InternalBleedingIntensity * 0.25f, 0.1f));
			if ((Broken || IsParalysed) && PhysicalBehaviour.Charge < 0.05f)
			{
				return 0f;
			}
			float a = (Health / InitialHealth + Mathf.Clamp01(Person.AdrenalineLevel)) * Mathf.Pow(CirculationBehaviour.GetAmountOfBlood(), 3f * BloodMuscleStrengthRatio) * Mathf.Pow(CirculationBehaviour.BloodFlow, 3f * BloodMuscleStrengthRatio) * Mathf.Clamp01(Person.Consciousness + Mathf.Clamp01(Person.AdrenalineLevel)) * Mathf.Clamp01(1f - PhysicalBehaviour.BurnProgress) * Mathf.Clamp01(1f - SkinMaterialHandler.AcidProgress) * Mathf.Clamp01(1f - (Numbness - Mathf.Clamp01(Person.AdrenalineLevel)));
			float num = ((Person.ActivePose != null && IsActiveInCurrentPose) ? Person.ActivePose.ForceMultiplier : 1f);
			return BaseStrength * num * 0.9f * Mathf.Clamp01(Mathf.Max(a, PhysicalBehaviour.Charge * 0.5f)) * GetMassStrengthRatio();
		}
	}

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
			GetComponent<SpriteRenderer>().color = value;
		}
	}

	public Liquid GetOriginalBloodType()
	{
		return Liquid.GetLiquid(BloodLiquidType);
	}

	private float GetMassStrengthRatio()
	{
		return PhysicalBehaviour.rigidbody.mass / PhysicalBehaviour.TrueInitialMass;
	}

	private void Awake()
	{
		Color = Color.white;
		SkinMaterialHandler = GetComponent<SkinMaterialHandler>();
		PhysicalBehaviour = GetComponent<PhysicalBehaviour>();
		CirculationBehaviour = GetComponent<CirculationBehaviour>();
		NodeBehaviour = GetComponent<ConnectedNodeBehaviour>();
		GripBehaviour = GetComponent<GripBehaviour>();
		goreString = GetComponent<GoreStringBehaviour>();
		myStatus = UnityEngine.Object.Instantiate(LimbStatus, base.transform);
		myStatus.GetComponent<LimbStatusBehaviour>().limb = this;
		SkinMaterialHandler.limb = this;
		CirculationBehaviour.Limb = this;
		Collider = GetComponent<Collider2D>();
		Joint = GetComponent<HingeJoint2D>();
		ShatteredObjectGenerators = GetComponent<ShatteredObjectSpriteInitialiser>();
		InitialHealth = Health;
		HasJoint = Joint;
		randomOffset = UnityEngine.Random.Range(-10000, 10000);
		if (HasJoint)
		{
			SetupJoint();
		}
		InternalTemperature = BodyTemperature;
		PhysicalBehaviour.Temperature = BodyTemperature;
	}

	private void Start()
	{
		if (UndoControllerBehaviour.FindRelevantAction(base.transform.root.gameObject, out var result))
		{
			ObjectCreationAction objectCreationAction = result as ObjectCreationAction;
			if (objectCreationAction != null)
			{
				undoAction = objectCreationAction;
			}
		}
		gforce = base.gameObject.GetOrAddComponent<GForceMeasureBehaviour>();
		LimbBehaviourManager.Limbs.Add(this);
		PhysicalBehaviour.OnDisintegration += PhysicalBehaviour_OnDisintegration;
		IsOnFloor = false;
		CreateContextMenuOptions();
		if (Broken)
		{
			BreakBoneInternal();
		}
		SynchroniseDismemberment();
		if (HasJoint)
		{
			originalJointLimits.x = Joint.limits.min;
			originalJointLimits.y = Joint.limits.max;
		}
		for (int i = 0; i < ConnectedLimbs.Count; i++)
		{
			LimbBehaviour limbBehaviour = ConnectedLimbs[i];
			if ((!NearestLimbToBrain || limbBehaviour.DistanceToBrain < NearestLimbToBrain.DistanceToBrain) && limbBehaviour.DistanceToBrain < DistanceToBrain)
			{
				NearestLimbToBrain = limbBehaviour;
			}
		}
		if ((bool)goreString)
		{
			goreString.TissueColour = GetOriginalBloodType().Color;
		}
	}

	private void PhysicalBehaviour_OnDisintegration(object sender, EventArgs e)
	{
		if (NodeBehaviour.IsConnectedToRoot && !IsParalysed)
		{
			Person.AddPain(25f);
		}
		SkinMaterialHandler.AddDamagePoint(DamageType.Dismemberment, base.transform.position, 25f);
		CirculationBehaviour.Disintegrate();
		for (int i = 0; i < ConnectedLimbs.Count; i++)
		{
			LimbBehaviour limbBehaviour = ConnectedLimbs[i];
			if (NodeBehaviour.IsConnectedTo(limbBehaviour.NodeBehaviour) && limbBehaviour.HasJoint && limbBehaviour.Joint.connectedBody == PhysicalBehaviour.rigidbody)
			{
				limbBehaviour.Joint.breakForce = 0f;
			}
		}
		if ((bool)goreString)
		{
			goreString.DestroyJoint();
		}
		NodeBehaviour.DisconnectFromEverything();
		if (HasJoint)
		{
			Joint.breakForce = 0f;
			Joint.breakTorque = 0f;
		}
	}

	private void CreateContextMenuOptions()
	{
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("inspectLimb", "Inspect", "Inspect", delegate
		{
			LimbStatusViewBehaviour.Main.Limbs = new List<LimbBehaviour>();
			foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
			{
				if (selectedObject.TryGetComponent<LimbBehaviour>(out var component))
				{
					LimbStatusViewBehaviour.Main.Limbs.Add(component);
				}
			}
			LimbStatusViewBehaviour.Main.gameObject.SetActive(value: true);
		}));
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("breakBones", () => (!Broken) ? "Break bone" : "Mend bone", "Mend or break bone", delegate
		{
			if (Broken)
			{
				HealBone();
			}
			else
			{
				BreakBone();
			}
		}));
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton item = new ContextMenuButton(() => Person.OverridePoseIndex != -1, "clearOverride", "Clear animation override", "Resets the animation to be controlled by the algorithm again", delegate
		{
			Person.OverridePoseIndex = -1;
		})
		{
			LabelWhenMultipleAreSelected = "Reset all animations"
		};
		buttons.Add(item);
		List<ContextMenuButton> buttons2 = PhysicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton(() => Person.OverridePoseIndex != 3, "startStumbling", "Stumble", "Forces the stumbling animation override", delegate
		{
			Person.OverridePoseIndex = 3;
		})
		{
			LabelWhenMultipleAreSelected = "Stumble"
		};
		buttons2.Add(item);
		List<ContextMenuButton> buttons3 = PhysicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton(() => Person.OverridePoseIndex != 6, "startWalking", "Walk", "Forces the walking animation override", delegate
		{
			Person.OverridePoseIndex = 6;
		})
		{
			LabelWhenMultipleAreSelected = "Walk"
		};
		buttons3.Add(item);
		List<ContextMenuButton> buttons4 = PhysicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton(() => Person.OverridePoseIndex != 1, "startProtect", "Cower", "Forces the protection animation override", delegate
		{
			Person.OverridePoseIndex = 1;
		})
		{
			LabelWhenMultipleAreSelected = "Cower"
		};
		buttons4.Add(item);
		List<ContextMenuButton> buttons5 = PhysicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton(() => Person.OverridePoseIndex != 7, "startSit", "Sit", "Forces the sitting animation override", delegate
		{
			Person.OverridePoseIndex = 7;
		})
		{
			LabelWhenMultipleAreSelected = "Sit"
		};
		buttons5.Add(item);
		List<ContextMenuButton> buttons6 = PhysicalBehaviour.ContextMenuOptions.Buttons;
		item = new ContextMenuButton(() => Person.OverridePoseIndex != 8, "startPetrified", "Flat pose", "Forces the resting animation override", delegate
		{
			Person.OverridePoseIndex = 8;
		})
		{
			LabelWhenMultipleAreSelected = "Flat pose"
		};
		buttons6.Add(item);
	}

	public void ManagedFixedUpdate()
	{
		shotHeat = 0f;
		for (int i = 0; i < ConnectedLimbs.Count; i++)
		{
			LimbBehaviour limbBehaviour = ConnectedLimbs[i];
			if ((bool)limbBehaviour && !limbBehaviour.PhysicalBehaviour.isDisintegrated && NodeBehaviour.IsConnectedTo(limbBehaviour.NodeBehaviour))
			{
				Utils.TransferEnergyFixedRate(limbBehaviour.PhysicalBehaviour, PhysicalBehaviour);
				Utils.AverageTemperature(limbBehaviour.PhysicalBehaviour, PhysicalBehaviour);
			}
		}
		float temperature = PhysicalBehaviour.Temperature;
		float internalTemperature = InternalTemperature;
		PhysicalBehaviour.Temperature = Mathf.Lerp(temperature, internalTemperature, InternalToExternalTempTransferRate);
		InternalTemperature = Mathf.Lerp(internalTemperature, temperature, (temperature > internalTemperature) ? (ExternalToInternalTempTransferRate * 0.3f) : ExternalToInternalTempTransferRate);
		if (PhysicalBehaviour.IsTouchingSomething != IsOnFloor)
		{
			IsOnFloor = PhysicalBehaviour.IsTouchingSomething;
		}
		if (HasBrain)
		{
			if (CirculationBehaviour.BloodFlow < 0.25f)
			{
				Person.OxygenLevel -= Time.deltaTime;
			}
			if (Health < InitialHealth / 2f)
			{
				LimbBehaviour[] limbs = Person.Limbs;
				foreach (LimbBehaviour limbBehaviour2 in limbs)
				{
					if (limbBehaviour2.NodeBehaviour.IsConnectedToRoot)
					{
						limbBehaviour2.InfluenceMotorSpeed(0f, 0.3f);
					}
				}
			}
			if (IsZombie)
			{
				Person.BrainDamaged = false;
			}
			else if (UserPreferenceManager.Current.BrainDamage && !IsAndroid && (CirculationBehaviour.InternalBleedingIntensity > 0.5f || Person.OxygenLevel <= 0.25f))
			{
				Person.BrainDamaged |= UnityEngine.Random.value > 0.999f;
			}
		}
		if (!IsZombie && !IsAndroid && CirculationBehaviour.InternalBleedingIntensity > 0.2f)
		{
			float internalBleedingIntensity = CirculationBehaviour.InternalBleedingIntensity;
			switch (RoughClassification)
			{
			case BodyPart.Head:
				Damage(((UnityEngine.Random.value > 5f / internalBleedingIntensity) ? 20f : 0.0004f) * internalBleedingIntensity);
				Person.OxygenLevel -= 0.0001f * internalBleedingIntensity;
				Person.Consciousness -= 0.0001f * internalBleedingIntensity;
				break;
			case BodyPart.Torso:
				Damage(0.0015f * internalBleedingIntensity);
				break;
			case BodyPart.Legs:
			case BodyPart.Arms:
				Numbness += 0.2f * internalBleedingIntensity;
				Damage(0.001f * internalBleedingIntensity);
				break;
			}
			if (NodeBehaviour.IsConnectedToRoot)
			{
				if (UnityEngine.Random.value > 0.9999f)
				{
					Person.Consciousness -= 0.001f * internalBleedingIntensity;
				}
				if ((double)UnityEngine.Random.value > 0.99)
				{
					Person.Wince(UnityEngine.Random.value * CirculationBehaviour.InternalBleedingIntensity * 60f);
				}
			}
		}
		CalculateJointStress();
		if (Mathf.Abs(InternalTemperature - BodyTemperature) >= 3f)
		{
			Numbness += 0.001f * UnityEngine.Random.value;
			Damage(0.005f * UnityEngine.Random.value);
		}
		if (InternalTemperature >= DiscomfortingHeatTemperature || InternalTemperature <= BodyTemperature - 3f)
		{
			if (InternalTemperature >= 100f)
			{
				CirculationBehaviour.HealBleeding();
			}
			if (HasBrain && !IsParalysed)
			{
				if (InternalTemperature > BodyTemperature)
				{
					Person.Consciousness *= 0.9995f;
					Person.AddPain(20f);
				}
				else
				{
					Person.Consciousness *= 0.999f;
				}
			}
			Damage(0.0015f);
		}
		if (PhysicalBehaviour.Charge > float.Epsilon && !IsZombie)
		{
			Damage(PhysicalBehaviour.Charge * 0.01f);
		}
		if (PhysicalBehaviour.Temperature > PhysicalBehaviour.Properties.BurningTemperatureThreshold)
		{
			Wince(0.1f);
			Damage(0.0015f + (PhysicalBehaviour.Temperature - PhysicalBehaviour.Properties.BurningTemperatureThreshold) / 1000f);
		}
		else if (PhysicalBehaviour.Temperature < FreezingTemperature)
		{
			if (SkinMaterialHandler.AcidProgress < 0.5f + randomOffset / 90000f)
			{
				SkinMaterialHandler.AcidProgress += 0.0005f;
			}
			Damage(0.05f);
		}
		if (PhysicalBehaviour.Wetness > 0.25f && IsAndroid)
		{
			PhysicalBehaviour.Charge += 0.5f;
		}
		if (CirculationBehaviour.HasCirculation)
		{
			float num = ((InternalTemperature <= BodyTemperature) ? 0.05f : 0.025f);
			if (PhysicalBehaviour.Temperature < FreezingTemperature)
			{
				num *= 0.03f;
			}
			InternalTemperature = Mathf.Lerp(InternalTemperature, BodyTemperature, num * BodyHeatProductionFactor);
		}
		if (IsConsideredAlive)
		{
			if (PhysicalBehaviour.OnFire)
			{
				SkinMaterialHandler.AcidProgress += Time.fixedDeltaTime * 0.01f;
				PhysicalBehaviour.BurnProgress += Time.fixedDeltaTime * 0.01f;
				Health -= Time.deltaTime * 0.5f * (IsZombie ? 0.01f : 1f);
				if (!IsZombie && NodeBehaviour.IsConnectedToRoot && !IsParalysed)
				{
					if (UserPreferenceManager.Current.StopAnimationOnDamage)
					{
						Person.OverridePoseIndex = -1;
					}
					Person.AddPain(PhysicalBehaviour.BurnIntensity * Time.deltaTime);
				}
			}
			if (UserPreferenceManager.Current.AutoHealWounds && RegenerateBurnProgressSpeed > float.Epsilon && PhysicalBehaviour.BurnProgress > 0f)
			{
				PhysicalBehaviour.BurnProgress -= Time.fixedDeltaTime * RegenerateBurnProgressSpeed * 0.001f;
				PhysicalBehaviour.BurnProgress = Mathf.Max(0f, PhysicalBehaviour.BurnProgress);
			}
		}
		if (CirculationBehaviour.BloodFlow > Mathf.Max(0.25f, 0.9f - Mathf.Clamp01(Person.AdrenalineLevel)) && Person.IsTouchingFloor && Person.ActivePose.ShouldStandUpright && IsCapable)
		{
			if (FakeUprightForce > 0.001f)
			{
				FakeStandUpright();
			}
			if (PhysicalBehaviour.rigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				PhysicalBehaviour.rigidbody.angularVelocity *= Mathf.Lerp(1f, 0.92f, Person.ActivePose.DragInfluence);
				PhysicalBehaviour.rigidbody.velocity *= Mathf.Lerp(1f, 0.94f, Person.ActivePose.DragInfluence);
			}
		}
		if (HasJoint)
		{
			if (Frozen || PhysicalBehaviour.Temperature <= FreezingTemperature)
			{
				SetMotorStrength(10f);
				InfluenceMotorSpeed(0f, 1f);
			}
			else
			{
				if (IsConsideredAlive && PhysicalBehaviour.Temperature <= BodyTemperature - 15f)
				{
					InfluenceMotorSpeed(UnityEngine.Random.Range(-45, 45));
				}
				SetMotorStrengthToMuscleStrength();
				if (!ApplyPoseOverrides() && IsActiveInCurrentPose && IsConsideredAlive)
				{
					MoveIntoPose(Person.ActivePose);
				}
			}
			if (IsConsideredAlive && PhysicalBehaviour.Charge > float.Epsilon)
			{
				InfluenceMotorSpeed(-50f * base.transform.root.localScale.x, PhysicalBehaviour.Charge * 0.5f);
			}
			if (!IsZombie || !IsConsideredAlive)
			{
				InfluenceMotorSpeed(0f, SkinMaterialHandler.RottenProgress);
			}
		}
		if (ImmuneToDamage || PhysicalBehaviour.rigidbody.bodyType != 0 || !(SpeciesIdentity == "Human") || !(GForcePassoutThreshold > float.Epsilon))
		{
			return;
		}
		float sqrMagnitude = gforce.SustainedAcceleration.sqrMagnitude;
		if (sqrMagnitude > GForcePassoutThreshold * GForcePassoutThreshold)
		{
			if (HasBrain)
			{
				Person.Consciousness *= 0.5f;
			}
			if (sqrMagnitude > GForceDamageThreshold * GForceDamageThreshold)
			{
				Damage(sqrMagnitude / 250f);
			}
		}
	}

	private bool ApplyPoseOverrides()
	{
		if (IsCapable && Person.IsTouchingFloor && Person.ActivePose.ShouldStumble)
		{
			float num = Mathf.Abs(Person.BalanceOffset * 1.5f);
			float num2 = Mathf.Clamp(MotorStrength, 0f, num * 2f);
			if (DoBalanceJerk)
			{
				InfluenceMotorSpeed(Mathf.DeltaAngle(Joint.jointAngle, 0f - Mathf.DeltaAngle(base.transform.eulerAngles.z, 0f)) * 1f * BalanceMuscleMovement, 0.5f * num2);
			}
			if (IsActiveInCurrentPose && Person.ShockLevel < 0.5f && DoStumble && num > 0.3f && num < 1f)
			{
				stumbleTime += Mathf.Clamp(Person.BalanceOffset * -0.1f, -1.7f, 1.7f);
				MoveIntoPoseAt(Person.LinkedPoses[PoseState.Stumbling], stumbleTime, Mathf.Pow(num, 1.5f) * 1.3f * num2);
				return true;
			}
		}
		return false;
	}

	public void ManagedUpdate()
	{
		SetJointFragility();
		float deltaTime = Time.deltaTime;
		bool flag = Health > float.Epsilon;
		if (HasBrain)
		{
			if (Person.Braindead)
			{
				Person.Consciousness = 0f;
				Person.ShockLevel = 0f;
				Person.PainLevel = 0f;
				Health = 0f;
			}
			else
			{
				Person.Braindead = !flag;
			}
		}
		if (flag && Health < InitialHealth)
		{
			Health += RegenerationSpeed * deltaTime;
		}
		HandleDrowning();
		Numbness -= deltaTime / 20f;
		if (!NodeBehaviour.IsConnectedToRoot && Health > 0f)
		{
			Health -= deltaTime / 20f * InitialHealth;
		}
		if ((bool)GripBehaviour && GripBehaviour.isHolding && UserPreferenceManager.Current.DropOnDeath && !IsConsideredAlive)
		{
			GripBehaviour.DropObject();
		}
		if (PhysicalBehaviour.isDisintegrated)
		{
			RegenerationSpeed = 0f;
			Health = 0f;
		}
		if (HasLungs && LungsPunctured && NodeBehaviour.IsConnectedToRoot)
		{
			if (!IsZombie)
			{
				Person.AddPain(2f);
			}
			Person.OxygenLevel -= deltaTime * 0.5f;
		}
		if (PhysicalBehaviour.IsBeingStabbed)
		{
			for (int i = 0; i < PhysicalBehaviour.beingStabbedBy.Count; i++)
			{
				PhysicalBehaviour physicalBehaviour = PhysicalBehaviour.beingStabbedBy[i];
				if ((bool)physicalBehaviour && physicalBehaviour.StabCausesWound && physicalBehaviour.GetRelativeStabSpeed(PhysicalBehaviour) > 0.05f)
				{
					physicalBehaviour.SendMessage("Decal", new DecalInstruction(BloodDecal, physicalBehaviour.GetGlobalStabPoint(PhysicalBehaviour), CirculationBehaviour.GetComputedColor(GetOriginalBloodType().Color)), SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		if (Health > float.Epsilon && previousHealth <= float.Epsilon)
		{
			KillShotParticlesEmitted = false;
		}
		previousHealth = Health;
	}

	private void HandleDrowning()
	{
		if (!NodeBehaviour.IsConnectedToRoot || IsAndroid)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (Person.OxygenLevel < 0.05f)
		{
			Damage(deltaTime * 4f);
		}
		if (HasBrain && !(PhysicalBehaviour.Wetness < 0.5f))
		{
			Person.OxygenLevel -= deltaTime / 40f;
			if (Person.OxygenLevel < 0.9f)
			{
				Person.Consciousness -= deltaTime / 40f * 0.5f;
				Person.AddPain(deltaTime * 0.9f);
			}
		}
	}

	private void CalculateJointStress()
	{
		if (HasJoint && !Broken && (bool)Joint && Joint.useLimits)
		{
			if (Joint.jointAngle - 5f > OriginalJointLimits.y || Joint.jointAngle + 5f < OriginalJointLimits.x)
			{
				JointStress += (IsAndroid ? 0.1f : 0.25f);
			}
			if (JointStress > BreakingThreshold * GetMassStrengthRatio())
			{
				BreakBone();
			}
			JointStress *= 0.982f;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		int contacts = collision.GetContacts(contactBuffer);
		float num = Utils.GetAverageImpulseRemoveOutliers(contactBuffer, contacts) / GetMassStrengthRatio() * UserPreferenceManager.Current.FragilityMultiplier * 0.8f;
		Vector2 normal = contactBuffer[0].normal;
		Vector2 point = contactBuffer[0].point;
		PhysicalBehaviour value;
		if (IsAndroid)
		{
			num *= 0.1f;
		}
		else if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.transform, out value) && value.SimulateTemperature && value.Temperature >= 70f)
		{
			Damage(value.Temperature / 140f);
			SkinMaterialHandler.AddDamagePoint(DamageType.Burn, point, value.Temperature * 0.01f);
			if (NodeBehaviour.IsConnectedToRoot && !IsParalysed)
			{
				Person.AddPain(1f);
			}
			Wince(150f);
			if (value.Temperature >= 100f)
			{
				CirculationBehaviour.HealBleeding();
			}
		}
		if (HasBrain && num > 0.6f && (double)UnityEngine.Random.value > 0.8)
		{
			CirculationBehaviour.InternalBleedingIntensity += num;
		}
		if (num < 2f)
		{
			return;
		}
		BruiseCount++;
		PropagateImpactDamage(num, normal, point, 0, this);
		if (!(num < 1f) && !IsAndroid && !(CirculationBehaviour.GetAmountOfBlood() < 0.2f))
		{
			if (UnityEngine.Random.value > 0.8f)
			{
				CirculationBehaviour.Cut(point, normal);
			}
			if (!(num < 3f) || !(Health > InitialHealth * 0.2f))
			{
				PhysicalBehaviour.CreateImpactEffect(point, normal, Mathf.Clamp(num / 4f, 1f, 2f));
				collision.gameObject.SendMessage("Decal", new DecalInstruction(BloodDecal, point, CirculationBehaviour.GetComputedColor(GetOriginalBloodType().Color)), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void PropagateImpactDamage(float impulse, Vector2 direction, Vector2 pos, int iteration, LimbBehaviour origin)
	{
		ActOnImpact(impulse, pos);
		if (iteration >= 8)
		{
			return;
		}
		for (int i = 0; i < ConnectedLimbs.Count; i++)
		{
			LimbBehaviour limbBehaviour = ConnectedLimbs[i];
			if (!(limbBehaviour == origin))
			{
				float num = Vector2.Dot((base.transform.position - limbBehaviour.transform.position).normalized, direction);
				if (num > 0f)
				{
					limbBehaviour.PropagateImpactDamage(num * impulse * 0.9f, direction, pos, iteration + 1, this);
				}
			}
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (!IsAndroid)
		{
			int contacts = collision.GetContacts(contactBuffer);
			ContactPoint2D firstValidContact = Utils.GetFirstValidContact(contactBuffer, contacts);
			float massStrengthRatio = GetMassStrengthRatio();
			if (collision.relativeVelocity.sqrMagnitude * UserPreferenceManager.Current.FragilityMultiplier / massStrengthRatio > FrictionBurnWoundMinSpeedSqrd)
			{
				Damage(1f);
				SkinMaterialHandler.AddDamagePoint(DamageType.Burn, firstValidContact.point, 2f);
			}
			if (UserPreferenceManager.Current.LimbCrushing && !(Utils.GetMinImpulse(contactBuffer, contacts) * UserPreferenceManager.Current.CrushForceMultiplier / massStrengthRatio * UserPreferenceManager.Current.FragilityMultiplier < Mathf.Max(10f, BreakingThreshold) * Mathf.Lerp((float)Physics2D.positionIterations / 16f, 1f, 0.2f)))
			{
				collision.gameObject.SendMessage("Decal", new DecalInstruction(BloodDecal, base.transform.position, CirculationBehaviour.GetComputedColor(GetOriginalBloodType().Color)), SendMessageOptions.DontRequireReceiver);
				Crush();
			}
		}
	}

	private void ActOnImpact(float impulse, Vector3 globalPosition)
	{
		if (ImmuneToDamage)
		{
			return;
		}
		float num = BreakingThreshold * GetMassStrengthRatio() / ImpactDamageMultiplier;
		if (impulse > num && UnityEngine.Random.value > 0.2f)
		{
			BreakBone();
		}
		float num2 = Mathf.Max(1f, Vitality);
		if (impulse > num * 0.5f / num2 && UnityEngine.Random.value > 0.8f / num2)
		{
			CirculationBehaviour.InternalBleedingIntensity += Mathf.Clamp(impulse * num2, 0f, 1f);
		}
		if (!IsAndroid)
		{
			if (UserPreferenceManager.Current.BrainDamage && HasBrain && UnityEngine.Random.value > 0.991f && impulse > 4f)
			{
				Person.BrainDamaged = true;
			}
			else
			{
				Damage(impulse * impulse * impulse * 2.8f * ImpactDamageMultiplier);
			}
		}
		SkinMaterialHandler.AddDamagePoint(DamageType.Blunt, globalPosition, impulse * 4f);
		float num3 = impulse * (Vitality + 1f) * 0.25f;
		if (!Person.BrainDamaged && HasBrain && num3 > 1f && !IsAndroid && UnityEngine.Random.value > 0.5f)
		{
			Person.Consciousness *= UnityEngine.Random.value * 0.8f;
		}
		if (NodeBehaviour.IsConnectedToRoot && num3 > 0.2f && !IsZombie)
		{
			Person.ShockLevel += num3 * 0.04f;
		}
	}

	private void FakeStandUpright()
	{
		if (PhysicalBehaviour.rigidbody.bodyType != 0 || !NodeBehaviour.IsConnectedToRoot || Vector2.Dot(base.transform.up, Vector2.down) > 0f)
		{
			return;
		}
		float num = Mathf.DeltaAngle(base.transform.eulerAngles.z, 0f) * FakeUprightForce * MotorStrength * Mathf.Lerp(GetMassStrengthRatio(), 1f, 0.6f);
		if (!float.IsNaN(num))
		{
			if (IsAndroid)
			{
				num *= 3.2f;
			}
			PhysicalBehaviour.rigidbody.AddTorque(num * 1.8f * Person.ActivePose.UprightForceMultiplier);
		}
	}

	private void SetupJoint()
	{
		Joint.useMotor = true;
		Joint.motor = new JointMotor2D
		{
			maxMotorTorque = MotorStrength,
			motorSpeed = 0f
		};
	}

	private void MoveIntoPose(RagdollPose activePose, float speedModifier = 1f, float rigidityMultiplier = 1f)
	{
		RagdollPose.LimbPose limbPose = activePose.AngleDictionary[this];
		float num = ((PhysicalBehaviour.Temperature <= FreezingTemperature + 10f || PhysicalBehaviour.Temperature >= DiscomfortingHeatTemperature) ? 0.5f : 1f);
		if (activePose.State == PoseState.Walking)
		{
			num *= (float)((Person.DesiredWalkingDirection >= 0f) ? 1 : (-1));
		}
		speedModifier /= Mathf.Lerp(GetMassStrengthRatio(), 1f, 0.9f);
		InfluenceMotorSpeed(Mathf.DeltaAngle(Joint.jointAngle, Joint.referenceAngle + limbPose.EvaluateAngle(activePose.AnimationSpeedMultiplier * speedModifier * num) * Person.AngleOffset) * rigidityMultiplier * Person.GlobalRigidityMultiplier * activePose.Rigidity * (1f + limbPose.PoseRigidityModifier));
	}

	private void MoveIntoPoseAt(RagdollPose activePose, float timeOverride, float rigidityMultiplier = 1f)
	{
		RagdollPose.LimbPose limbPose = activePose.AngleDictionary[this];
		InfluenceMotorSpeed(Mathf.DeltaAngle(Joint.jointAngle, Joint.referenceAngle + limbPose.EvaluateAngleAt(timeOverride) * Person.AngleOffset) * rigidityMultiplier * Person.GlobalRigidityMultiplier * activePose.Rigidity * (1f + limbPose.PoseRigidityModifier));
	}

	public void BreakBone()
	{
		if (!Broken)
		{
			BreakBoneInternal();
		}
	}

	private void BreakBoneInternal()
	{
		Broken = true;
		if (HasJoint)
		{
			if (IsLethalToBreak)
			{
				Damage(Health + 1f);
			}
			if (NodeBehaviour.IsConnectedToRoot)
			{
				Person.ShockLevel += UnityEngine.Random.value * 5f;
				Person.Wince(UnityEngine.Random.value * 150f);
			}
			if ((double)UnityEngine.Random.value > 0.9)
			{
				CirculationBehaviour.InternalBleedingIntensity += UnityEngine.Random.value;
			}
			PhysicalBehaviour.PlayClipOnce(Person.BoneBreakClips.PickRandom());
			if (Joint.useLimits)
			{
				JointAngleLimits2D limits = Joint.limits;
				limits.max = Mathf.Lerp(limits.max, 180f, 0.5f);
				limits.min = Mathf.Lerp(limits.min, -180f, 0.5f);
				Joint.limits = limits;
			}
		}
	}

	public void HealBone()
	{
		Broken = false;
		if ((bool)Joint && Joint.useLimits)
		{
			JointStress = 0f;
			JointAngleLimits2D limits = Joint.limits;
			limits.min = OriginalJointLimits.x;
			limits.max = OriginalJointLimits.y;
			Joint.limits = limits;
		}
	}

	private void SetMotorStrength(float strength)
	{
		JointMotor2D motor = Joint.motor;
		float num = 1f;
		if (IsActiveInCurrentPose)
		{
			num = Mathf.Clamp01(1f + Person.ActivePose.AngleDictionary[this].PoseRigidityModifier);
		}
		motor.maxMotorTorque = strength * (IsAndroid ? 6f : 1f) * num;
		Joint.motor = motor;
	}

	private void SetMotorStrengthToMuscleStrength()
	{
		float num = Mathf.Max(MotorStrength, SkinMaterialHandler.RottenProgress * 0.8f);
		if (StrengthMultipliers != null && StrengthMultipliers.Length != 0)
		{
			for (int i = 0; i < StrengthMultipliers.Length; i++)
			{
				num *= StrengthMultipliers[i];
				if (Mathf.Abs(num) <= float.Epsilon)
				{
					break;
				}
			}
		}
		SetMotorStrength(num);
	}

	private void SetJointFragility()
	{
		if (!HasJoint)
		{
			return;
		}
		if (PhysicalBehaviour.isDisintegrated)
		{
			Joint.breakForce = 0f;
			return;
		}
		float num = BreakingThreshold * 120f * Mathf.Clamp(1f - SkinMaterialHandler.RottenProgress, 0.1f, 1f) * GetMassStrengthRatio();
		num /= UserPreferenceManager.Current.FragilityMultiplier;
		if (!IsAndroid)
		{
			num *= Mathf.Clamp(Utils.MapRange(-100f, 0f, 0.1f, 1f, PhysicalBehaviour.Temperature), 0.1f, 1f);
		}
		if (float.IsInfinity(num))
		{
			num = float.MaxValue;
		}
		Joint.breakForce = num * (IsAndroid ? 80f : 2.5f);
		Joint.breakTorque = num * 5000f;
	}

	public void InfluenceMotorSpeed(float value, float influence = 0.5f)
	{
		if (HasJoint)
		{
			JointMotor2D motor = Joint.motor;
			if (Person.BrainDamaged && UnityEngine.Random.value > 0.95f)
			{
				value *= (float)UnityEngine.Random.Range(-1, 1);
			}
			motor.motorSpeed = Mathf.Lerp(motor.motorSpeed, value, influence);
			Joint.motor = motor;
		}
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		if (!(joint != Joint))
		{
			CirculationBehaviour.ActOnJointBreak2D(joint);
			if (IsLethalToBreak)
			{
				Health = 0f;
			}
			if (NodeBehaviour.IsConnectedToRoot && !IsParalysed)
			{
				Person.AddPain(15f);
			}
			if (Joint.connectedBody.TryGetComponent<ConnectedNodeBehaviour>(out var component))
			{
				NodeBehaviour.DisconnectFrom(component);
			}
			if (!UserPreferenceManager.Current.GorelessMode && UserPreferenceManager.Current.DismembermentLooseTissue && (bool)goreString && UnityEngine.Random.value > 0.5f && goreString.enabled && !goreString.Joint && SkinMaterialHandler.AcidProgress < 0.7f && PhysicalBehaviour.BurnProgress < 0.8f)
			{
				goreString.CreateJoint();
			}
			IsDismembered = true;
			Vector3 vector = base.transform.TransformPoint(Joint.anchor);
			PhysicalBehaviour.CreateImpactEffect(vector, base.transform.up, 2f);
			SkinMaterialHandler.AddDamagePoint(DamageType.Dismemberment, vector, 25f);
			PhysicalBehaviour.PlayClipOnce(Person.DismembermentClips.PickRandom());
			SynchroniseDismemberment();
		}
	}

	private void SynchroniseDismemberment()
	{
		if (!IsDismembered)
		{
			return;
		}
		if ((bool)Joint)
		{
			UnityEngine.Object.Destroy(Joint);
		}
		HasJoint = false;
		for (int i = 0; i < ConnectedLimbs.Count; i++)
		{
			LimbBehaviour limbBehaviour = ConnectedLimbs[i];
			limbBehaviour.ConnectedLimbs.Remove(this);
			if (limbBehaviour.CirculationBehaviour.Source == this)
			{
				limbBehaviour.CirculationBehaviour.Source = null;
			}
		}
		if ((bool)CirculationBehaviour.Source)
		{
			ConnectedLimbs.Remove(CirculationBehaviour.Source.Limb);
			CirculationBehaviour.Source.Limb.ConnectedLimbs.Remove(this);
		}
	}

	public void Damage(float damage)
	{
		if (UserPreferenceManager.Current.StopAnimationOnDamage && !IsZombie && damage > 15.5f && NodeBehaviour.IsConnectedToRoot)
		{
			Person.OverridePoseIndex = -1;
		}
		Health -= damage;
		if (Health <= 0f)
		{
			CirculationBehaviour.IsPump = false;
		}
	}

	public void Shot(Shot shot)
	{
		if (ImmuneToDamage)
		{
			return;
		}
		shot.damage /= GetMassStrengthRatio();
		shot.damage *= UserPreferenceManager.Current.FragilityMultiplier;
		if (IsAndroid)
		{
			if (shot.damage < 40f)
			{
				return;
			}
			shot.damage *= 0.2f;
		}
		else
		{
			shotHeat += 1f;
		}
		if (HasLungs && !IsAndroid && UnityEngine.Random.value > 0.9f)
		{
			LungsPunctured = true;
		}
		bool flag = IsWorldPointInVitalPart(shot.point) && UnityEngine.Random.value > 0.05f;
		float num = (flag ? 7f : 0.1f) * shot.damage;
		if (!UserPreferenceManager.Current.GorelessMode && UserPreferenceManager.Current.ChunkyShotParticles && (bool)Person.PoolableImpactEffect && (double)PhysicalBehaviour.BurnProgress < 0.6 && SkinMaterialHandler.AcidProgress < 0.7f && num > Person.ImpactEffectShotDamageThreshold && UnityEngine.Random.value > 0.8f)
		{
			GameObject gameObject = PoolGenerator.Instance.RequestPrefab(Person.PoolableImpactEffect, shot.point);
			if ((bool)gameObject)
			{
				gameObject.transform.right = shot.normal;
			}
		}
		if (ConnectedLimbs != null)
		{
			for (int i = 0; i < ConnectedLimbs.Count; i++)
			{
				LimbBehaviour limbBehaviour = ConnectedLimbs[i];
				if ((bool)limbBehaviour)
				{
					limbBehaviour.Numbness += 0.5f;
				}
			}
		}
		if (NodeBehaviour.IsConnectedToRoot)
		{
			Person.AdrenalineLevel += UnityEngine.Random.value;
			Person.Consciousness -= UnityEngine.Random.Range(0.02f, 0.1f);
			if (!IsAndroid && !IsZombie)
			{
				Person.ShockLevel += num * UnityEngine.Random.value * 0.0025f;
				Person.Wince(300f);
				Numbness = 1f;
				if (UnityEngine.Random.value * Vitality > 0.5f && UnityEngine.Random.value > 0.6f && !IsParalysed)
				{
					Person.AddPain(UnityEngine.Random.value * 2f);
				}
				if (num > 2f * UnityEngine.Random.value)
				{
					if (shot.normal.x > 0f == Person.transform.localScale.x > 0f)
					{
						Person.DesiredWalkingDirection -= UnityEngine.Random.value * 3f;
					}
					else
					{
						Person.DesiredWalkingDirection += UnityEngine.Random.value * 3f;
					}
				}
				Person.SendMessage("Shot", shot);
			}
			if (HasBrain && !IsAndroid)
			{
				if (IsZombie && UnityEngine.Random.value > 0.2f)
				{
					return;
				}
				float num2 = (flag ? 0.1f : 0.8f);
				if (UnityEngine.Random.value > num2)
				{
					Health = 0f;
				}
				if (UnityEngine.Random.value > num2)
				{
					Person.Consciousness = 0f;
				}
			}
		}
		if (shot.CanCrush && UserPreferenceManager.Current.LimbCrushing && shot.damage > 149f && Mathf.Clamp((shot.damage - 149f) * 0.005f, 0.3f, 0.8f) > UnityEngine.Random.value)
		{
			if (UserPreferenceManager.Current.StopAnimationOnDamage)
			{
				Person.OverridePoseIndex = -1;
			}
			StartCoroutine(CrushNextFrame());
		}
		else if (shotHeat > 5f && UnityEngine.Random.value > 0.35f)
		{
			if (HasJoint && UnityEngine.Random.value > 0.8f)
			{
				Slice();
			}
			else if (shot.CanCrush && UserPreferenceManager.Current.LimbCrushing)
			{
				if (UserPreferenceManager.Current.StopAnimationOnDamage)
				{
					Person.OverridePoseIndex = -1;
				}
				StartCoroutine(CrushNextFrame());
			}
		}
		if (IsZombie || UnityEngine.Random.value < 0.01f)
		{
			Damage(num * ShotDamageMultiplier * 0.01f);
		}
		else
		{
			if (!IsAndroid)
			{
				CirculationBehaviour.InternalBleedingIntensity += num;
				if (flag)
				{
					CirculationBehaviour.InternalBleedingIntensity += 5f;
				}
			}
			Damage(Mathf.Min(InitialHealth / 2f, num * ShotDamageMultiplier * 2f));
		}
		float b = shot.damage * 0.1f;
		SkinMaterialHandler.AddDamagePoint(DamageType.Bullet, shot.point, Mathf.Max(50f, b));
	}

	private IEnumerator CrushNextFrame()
	{
		SkinMaterialHandler.AddDamagePoint(DamageType.Dismemberment, base.transform.position, 15f);
		yield return new WaitForEndOfFrame();
		yield return new WaitForFixedUpdate();
		Crush();
	}

	public void ExitShot(Shot shot)
	{
		if (ImmuneToDamage)
		{
			return;
		}
		shot.damage /= GetMassStrengthRatio();
		shot.damage *= UserPreferenceManager.Current.FragilityMultiplier;
		SkinMaterialHandler.AddDamagePoint(DamageType.Bullet, shot.point, Mathf.Max(60f, shot.damage * 0.4f));
		if (!UserPreferenceManager.Current.GorelessMode && UserPreferenceManager.Current.ChunkyShotParticles && (bool)Person.PoolableImpactEffect && (double)PhysicalBehaviour.BurnProgress < 0.6 && SkinMaterialHandler.AcidProgress < 0.7f && shot.damage > Person.ImpactEffectShotDamageThreshold && UnityEngine.Random.value > 0.6f)
		{
			GameObject gameObject = PoolGenerator.Instance.RequestPrefab(Person.PoolableImpactEffect, shot.point);
			if ((bool)gameObject)
			{
				gameObject.transform.right = shot.normal;
			}
		}
		if (HasLungs && !IsAndroid && UnityEngine.Random.value > 0.9f)
		{
			LungsPunctured = true;
		}
		if (UserPreferenceManager.Current.StopAnimationOnDamage && NodeBehaviour.IsConnectedToRoot)
		{
			Person.OverridePoseIndex = -1;
		}
		if (!UserPreferenceManager.Current.GorelessMode && !KillShotParticlesEmitted && Health <= float.Epsilon && (bool)KillShotParticles && UnityEngine.Random.value > 0.7f)
		{
			KillShotParticles.transform.right = shot.normal;
			KillShotParticles.Play();
			PhysicalBehaviour.PlayClipOnce(Person.DismembermentClips.PickRandom());
			KillShotParticlesEmitted = true;
		}
		if (IsAndroid || CirculationBehaviour.GetAmountOfBlood() < 0.05f)
		{
			return;
		}
		Color computedColor = CirculationBehaviour.GetComputedColor(GetOriginalBloodType().Color);
		for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++)
		{
			RaycastHit2D raycastHit2D = Physics2D.Raycast(shot.point, shot.normal + UnityEngine.Random.insideUnitCircle * 0.4f, 3f);
			if ((bool)raycastHit2D && (bool)raycastHit2D.transform)
			{
				raycastHit2D.transform.gameObject.SendMessage("Decal", new DecalInstruction(BloodDecal, raycastHit2D.point, computedColor), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void WaterImpact(float magnitude)
	{
		if (ImmuneToDamage)
		{
			return;
		}
		magnitude *= UserPreferenceManager.Current.FragilityMultiplier;
		if (magnitude > 30f)
		{
			Damage(magnitude * 0.09f);
			if (!IsAndroid)
			{
				SkinMaterialHandler.AcidProgress = Mathf.Min(SkinMaterialHandler.AcidProgress + magnitude * 0.05f, 0.6f);
			}
		}
	}

	public void Wince(float intensity = 1f)
	{
		if (HasJoint && NodeBehaviour.IsConnectedToRoot && !(Health < 0.1f))
		{
			float value = 60f * intensity * (Mathf.PerlinNoise(Time.time * 8f, randomOffset) * 2f - 1f);
			InfluenceMotorSpeed(Mathf.Clamp(value, -450f, 450f));
		}
	}

	public void OnEMPHit()
	{
		if (IsAndroid)
		{
			Health = 0f;
		}
	}

	public bool IsWorldPointInVitalPart(Vector2 worldPoint, float mindistance = 4f / 35f)
	{
		if (VitalParts == null)
		{
			return false;
		}
		float num = mindistance * mindistance;
		Vector3 vector = base.transform.InverseTransformPoint(worldPoint);
		for (int i = 0; i < VitalParts.Length; i++)
		{
			if ((VitalParts[i].ClosestPoint(vector) - vector).sqrMagnitude <= num)
			{
				return true;
			}
		}
		return false;
	}

	public void Stabbed(Stabbing stab)
	{
		if (ImmuneToDamage || !stab.stabber.StabCausesWound)
		{
			return;
		}
		if (CirculationBehaviour.GetAmountOfBlood() > 0.2f)
		{
			stab.stabber.SendMessage("Decal", new DecalInstruction(BloodDecal, stab.point, CirculationBehaviour.GetComputedColor(GetOriginalBloodType().Color)), SendMessageOptions.DontRequireReceiver);
		}
		if (HasLungs && !IsAndroid && UnityEngine.Random.value > 0.9f)
		{
			LungsPunctured = true;
		}
		bool flag = IsWorldPointInVitalPart(stab.point) && UnityEngine.Random.value > 0.05f;
		Damage(Health * 0.5f * (IsZombie ? 0.1f : 1f) * (float)((!flag) ? 1 : 2) * UserPreferenceManager.Current.FragilityMultiplier);
		if (flag && !IsZombie)
		{
			CirculationBehaviour.InternalBleedingIntensity += 5f * UnityEngine.Random.value;
		}
		Wince(165f);
		if (!IsZombie && NodeBehaviour.IsConnectedToRoot)
		{
			Person.ShockLevel += UnityEngine.Random.value;
		}
		Numbness = 1f;
		Person.AdrenalineLevel += 1f;
		if (HasBrain && flag && (!IsZombie || !(UnityEngine.Random.value > 0.5f)))
		{
			Person.AddPain(90f);
			CirculationBehaviour.InternalBleedingIntensity += 5f * UnityEngine.Random.value;
			Health = 0f;
			if (UnityEngine.Random.value > 0.25f)
			{
				Person.Consciousness = 0f;
			}
		}
	}

	public void Slice()
	{
		if (!ImmuneToDamage)
		{
			if (UserPreferenceManager.Current.StopAnimationOnDamage && NodeBehaviour.IsConnectedToRoot)
			{
				Person.OverridePoseIndex = -1;
			}
			Person.AdrenalineLevel += 1f;
			if (HasJoint)
			{
				RegenerationSpeed = 0f;
				Health = 0f;
				ActOnImpact(15f, base.transform.TransformPoint(Joint.anchor));
				BreakingThreshold = 0f;
			}
		}
	}

	public void Crush()
	{
		if (PhysicalBehaviour.isDisintegrated || ImmuneToDamage)
		{
			return;
		}
		if (UserPreferenceManager.Current.StopAnimationOnDamage && NodeBehaviour.IsConnectedToRoot)
		{
			Person.OverridePoseIndex = -1;
		}
		if (!UserPreferenceManager.Current.GorelessMode)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Person.BloodExplosionPrefab, base.transform.position, Quaternion.identity);
			if (!IsAndroid)
			{
				gameObject.GetComponentInChildren<BloodExplosionBehaviour>().SetColor(CirculationBehaviour.GetComputedColor(GetOriginalBloodType().Color));
			}
			ShatterProcedurally(ShatterFlags.All);
		}
		if (!PhysicalBehaviour.isDisintegrated)
		{
			PhysicalBehaviour.Disintegrate();
		}
	}

	private void ShatterProcedurally(ShatterFlags shatterFlags)
	{
		if (!(ShatteredObjectChance < UnityEngine.Random.value) && UserPreferenceManager.Current.ProceduralFragments && !(ShatteredObjectGenerators == null))
		{
			float brightness = Mathf.Min(1f - PhysicalBehaviour.BurnProgress, 1f - SkinMaterialHandler.RottenProgress * 0.5f);
			ShatteredObjectGenerator boneGenerator = ShatteredObjectGenerators.BoneGenerator;
			ShatteredObjectGenerator fleshGenerator = ShatteredObjectGenerators.FleshGenerator;
			ShatteredObjectGenerator skinGenerator = ShatteredObjectGenerators.SkinGenerator;
			shatterFlags &= (ShatterFlags)(~(uint)CurrentlyShattered);
			GameObject[] array = ArrayPool<GameObject>.Shared.Rent(Mathf.Max(boneGenerator ? boneGenerator.PartCount : 0, Mathf.Max(fleshGenerator ? fleshGenerator.PartCount : 0, skinGenerator ? skinGenerator.PartCount : 0)));
			if (shatterFlags.HasFlag(ShatterFlags.Bone) && (bool)boneGenerator)
			{
				boneGenerator.ConnectToRange += UnityEngine.Random.Range(0f, 0.1f);
				boneGenerator.Brightness = brightness;
				PrepareGoreBits(array, boneGenerator.Generate(base.transform, PhysicalBehaviour.rigidbody, array));
				CurrentlyShattered |= ShatterFlags.Bone;
			}
			if (shatterFlags.HasFlag(ShatterFlags.Flesh) && (bool)fleshGenerator && SkinMaterialHandler.IsFleshVisible)
			{
				fleshGenerator.ConnectToRange += UnityEngine.Random.Range(0f, 0.1f);
				fleshGenerator.Brightness = brightness;
				PrepareGoreBits(array, fleshGenerator.Generate(base.transform, PhysicalBehaviour.rigidbody, array));
				CurrentlyShattered |= ShatterFlags.Flesh;
			}
			if (shatterFlags.HasFlag(ShatterFlags.Skin) && (bool)skinGenerator && SkinMaterialHandler.IsSkinVisible)
			{
				skinGenerator.ConnectToRange += UnityEngine.Random.Range(0f, 0.1f);
				skinGenerator.Brightness = brightness;
				PrepareGoreBits(array, skinGenerator.Generate(base.transform, PhysicalBehaviour.rigidbody, array));
				CurrentlyShattered |= ShatterFlags.Skin;
			}
			ArrayPool<GameObject>.Shared.Return(array, clearArray: true);
		}
	}

	private void PrepareGoreBits(GameObject[] g, int count)
	{
		DisintegrationCounterBehaviour componentInParent = GetComponentInParent<DisintegrationCounterBehaviour>();
		Color color = GetOriginalBloodType().Color;
		for (int i = 0; i < count; i++)
		{
			DestroyWith.Add(g[i]);
			componentInParent.RegisterPseudoChild(g[i].GetComponent<PhysicalBehaviour>());
			g[i].AddComponent<PseudoBloodImpactHelper>().Color = color;
		}
	}

	public void StunImpact()
	{
		if (IsAndroid)
		{
			PhysicalBehaviour.Charge += 10f;
			return;
		}
		PhysicalBehaviour.Charge += 1f;
		if (NodeBehaviour.IsConnectedToRoot && !IsParalysed)
		{
			Person.AddPain(150f);
			if (UserPreferenceManager.Current.StopAnimationOnDamage)
			{
				Person.OverridePoseIndex = -1;
			}
		}
		StartCoroutine(Utils.DelayCoroutine(UnityEngine.Random.value * 0.8f, delegate
		{
			Numbness = 1f;
			if (NodeBehaviour.IsConnectedToRoot)
			{
				Person.Consciousness = 0f;
			}
		}));
	}

	public void OnDestroy()
	{
		LimbBehaviourManager.Limbs.Remove(this);
		UnityEngine.Object.Destroy(myStatus);
		foreach (GameObject item in DestroyWith)
		{
			if ((bool)item)
			{
				UnityEngine.Object.Destroy(item);
			}
		}
	}

	public void ManagedLateUpdate()
	{
	}

	public bool ShouldUpdate()
	{
		return base.enabled;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Activations;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class PhysicalBehaviour : MonoBehaviour, Messages.IUse, Messages.IUseContinuous, Messages.IShot, Messages.IExitShot, Messages.IStabbed, Messages.IOnBeforeSerialise, Messages.IDecal, IManagedBehaviour, IUseEmitter
{
	private class ColliderBoolPair
	{
		public bool Active;

		public Collider2D Coll;

		public ColliderBoolPair(bool active, Collider2D collider)
		{
			Active = active;
			Coll = collider;
		}
	}

	[Serializable]
	public class Penetration
	{
		public PhysicalBehaviour Stabber;

		public PhysicalBehaviour Victim;

		public SliderJoint2D Joint;

		public float Duration;

		public SharpAxis Axis;

		public List<Collider2D> RelevantColliders = new List<Collider2D>();

		public Vector2 CurrentConnectedAnchor;

		public bool Active;

		public Vector2 GetDirection()
		{
			return Stabber.transform.TransformVector(Axis.Axis);
		}

		public Vector2 GetPoint()
		{
			return Joint.connectedBody.transform.TransformPoint(Joint.connectedAnchor);
		}

		public float GetCurrentDepth()
		{
			if (Axis.LooseLowerLimit && Axis.LooseUpperLimit)
			{
				return Joint.jointTranslation;
			}
			Transform transform = Joint.transform;
			Vector3 position = transform.position;
			Vector3 vector = transform.TransformDirection(Axis.Axis);
			if (Axis.LooseLowerLimit)
			{
				return Vector2.Distance(Vector2.LerpUnclamped(position, position + vector, Axis.LowerLimit), Victim.transform.TransformPoint(Joint.connectedAnchor));
			}
			if (Axis.LooseUpperLimit)
			{
				return Vector2.Distance(Vector2.LerpUnclamped(position, position + vector, Axis.UpperLimit), Victim.transform.TransformPoint(Joint.connectedAnchor));
			}
			return 0f;
		}
	}

	public PhysicalProperties Properties;

	private GameObject FireParticlePrefab;

	private GameObject ChargedParticlePrefab;

	private GameObject SmokeParticlePrefab;

	private GameObject BubbleParticlePrefab;

	private ParticleSystem FireParticleSystem;

	private ParticleSystem ChargedParticleSystem;

	private ParticleSystem SmokeParticleSystem;

	private ParticleSystem BubbleParticleSystem;

	[SkipSerialisation]
	public SortingGroup SortingGroup;

	[SkipSerialisation]
	public Dictionary<ushort, DeltaInt> ContinuousActivationTracker = new Dictionary<ushort, DeltaInt>();

	public bool SendUserPropagation = true;

	[SkipSerialisation]
	[HideInInspector]
	public ContextMenuOptionComponent ContextMenuOptions;

	public bool ReflectsLasers;

	public bool AbsorbsLasers = true;

	public bool PlaySliderSound = true;

	public bool Resizable = true;

	public bool Selectable = true;

	[SkipSerialisation]
	public LocalFirePropagationMap LocalFirePropagationMap;

	[SkipSerialisation]
	public float StabWoundSizeMultiplier = 1f;

	[SkipSerialisation]
	public Rigidbody2D rigidbody;

	public bool ChargeBurns = true;

	public float BuoyancyModifier = 1f;

	public bool SimulateTemperature = true;

	public bool AmbientTemperatureTransfer = true;

	public bool isSliding;

	public float EnergyWireResistance = 1f;

	public float ActivationPropagationDelay = 0.1f;

	public NodeBehaviour ActivationNode;

	public bool SpawnSpawnParticles = true;

	private List<Collider2D> ignoredColliders = new List<Collider2D>();

	[SkipSerialisation]
	[HideInInspector]
	public GameObject selectionOutlineObject;

	[SkipSerialisation]
	[HideInInspector]
	public bool BulletPenetration = true;

	[SkipSerialisation]
	public Vector3[] HoldingPositions = new Vector3[0];

	public bool DisplayBloodDecals = true;

	public bool HasOutline = true;

	public float SoundVolumeBoost;

	[HideInInspector]
	public bool IsWeightless;

	[HideInInspector]
	public float charge;

	public bool ConductOverride = true;

	public bool ForceNoCharge;

	public bool ForceNoChargeParticles;

	public bool Deletable = true;

	public int MaximumStabVictims = 64;

	public bool Disintegratable = true;

	public bool StabCausesWound = true;

	public float LooseEndSize = 0.49f;

	public float StabReleaseSpeedModifier = 1f;

	internal int underwaterMarkings;

	[NonSerialized]
	public bool IsInLava;

	[NonSerialized]
	public float CurrentWaterSurfaceLevel;

	private float seed;

	private int intSeed;

	[HideInInspector]
	public float burnIntensity;

	[SkipSerialisation]
	public Collider2D[] colliders;

	[HideInInspector]
	public List<Penetration> penetrations = new List<Penetration>();

	[HideInInspector]
	public List<PhysicalBehaviour> beingStabbedBy = new List<PhysicalBehaviour>();

	[HideInInspector]
	[SkipSerialisation]
	public List<Penetration> victimPenetrations = new List<Penetration>();

	[HideInInspector]
	public uint stabWoundCount;

	private AudioSource audioSource;

	[HideInInspector]
	[SkipSerialisation]
	public AudioSource slidingAudioSource;

	[SkipSerialisation]
	public AudioClip[] OverrideImpactSounds;

	[SkipSerialisation]
	public AudioClip[] OverrideShotSounds;

	private float slideIntensity;

	private Vector2 previousVelocity = Vector2.zero;

	public static float AmbientTemperature;

	public float Temperature;

	[HideInInspector]
	public bool beingHeldByGripper;

	public float Wetness;

	[SkipSerialisation]
	[HideInInspector]
	public SpriteRenderer spriteRenderer;

	private bool showOutline;

	public int GridRes = 4;

	[SkipSerialisation]
	public Vector2[] LocalColliderGridPoints;

	public Bounds InitialBounds;

	public bool ForceContinuous;

	private MaterialPropertyBlock propertyBlock;

	[SkipSerialisation]
	public float ObjectArea;

	public float TemperatureWhenBurningLerpMultiplier = 1f;

	public float BurningProgressionMultiplier = 1f;

	private static readonly ContactPoint2D[] contactBuffer = new ContactPoint2D[8];

	public float Circumference = -1f;

	private readonly HashSet<DecalControllerBehaviour> decalControllers = new HashSet<DecalControllerBehaviour>();

	[SkipSerialisation]
	private bool dirtyFire;

	[SkipSerialisation]
	private bool dirtyCharged;

	[SkipSerialisation]
	private bool dirtySmoke;

	[SkipSerialisation]
	private bool dirtyBubble;

	private const float MaxTimeOutOfBounds = 5f;

	private float outOfBoundsDuration;

	private float affectSurroundingTimer;

	private FixedIntervalDistributor fireEmissionInterval = new FixedIntervalDistributor
	{
		MaxCycles = 2
	};

	private readonly ColliderBoolPair[] onCollisionStayBuffer = new ColliderBoolPair[8];

	private float sizzleAudioHeat;

	[HideInInspector]
	public float TrueInitialMass;

	public float InitialMass;

	[HideInInspector]
	public float InitialGravityScale;

	[ReadOnly]
	public bool isDisintegrated;

	[HideInInspector]
	public Guid[] serialisable_victims = new Guid[0];

	[HideInInspector]
	public float[] serialisable_durations = new float[0];

	[HideInInspector]
	public SharpAxis[] serialisable_axes = new SharpAxis[0];

	[HideInInspector]
	public Vector2[] serialisable_anchors = new Vector2[0];

	[HideInInspector]
	public bool[] serialisable_active = new bool[0];

	[NonSerialized]
	[SkipSerialisation]
	internal SpriteRenderer outlineSpriteRenderer;

	private ParticleSystem scrapeParticles;

	public bool OnFire { get; private set; }

	public float BurnProgress { get; set; }

	public float Charge
	{
		get
		{
			if (!ForceNoCharge)
			{
				return charge;
			}
			return 0f;
		}
		set
		{
			charge = value;
		}
	}

	public bool ShowOutline
	{
		get
		{
			return showOutline;
		}
		set
		{
			if (!UserPreferenceManager.Current.ShowOutlines || !HasOutline)
			{
				value = false;
			}
			showOutline = value;
			if ((bool)selectionOutlineObject)
			{
				selectionOutlineObject.SetActive(showOutline);
			}
		}
	}

	public float BurnIntensity
	{
		get
		{
			return burnIntensity;
		}
		set
		{
			burnIntensity = value;
		}
	}

	[SkipSerialisation]
	public bool IsUnderWater { get; set; }

	[SkipSerialisation]
	public bool IsBeingStabbed => stabWoundCount != 0;

	[SkipSerialisation]
	public AudioSource MainAudioSource => audioSource;

	public int LocalGridPointLength
	{
		get
		{
			Vector2[] localColliderGridPoints = LocalColliderGridPoints;
			if (localColliderGridPoints == null)
			{
				return 0;
			}
			return localColliderGridPoints.Length;
		}
	}

	public UnityEvent<ActivationPropagation> OnSingleUse { get; } = new UnityEvent<ActivationPropagation>();


	public UnityEvent<ActivationPropagation> OnContinuousUse { get; } = new UnityEvent<ActivationPropagation>();


	public float ObjectAreaByMass => rigidbody.mass / TrueInitialMass * ObjectArea;

	public bool IsTouchingSomething
	{
		get
		{
			for (int i = 0; i < onCollisionStayBuffer.Length; i++)
			{
				ColliderBoolPair colliderBoolPair = onCollisionStayBuffer[i];
				if (colliderBoolPair != null && (bool)colliderBoolPair.Coll && colliderBoolPair.Active && colliderBoolPair.Coll.gameObject.activeSelf)
				{
					return true;
				}
			}
			return false;
		}
	}

	[SkipSerialisation]
	public event EventHandler OnDisintegration;

	public bool IsBeingUsedContinuously(ushort channel)
	{
		if (ContinuousActivationTracker.TryGetValue(channel, out var value))
		{
			return value.Value > 0;
		}
		return false;
	}

	public bool WasBeingUsedContinuously(ushort channel)
	{
		if (ContinuousActivationTracker.TryGetValue(channel, out var value))
		{
			return value.PreviousValue > 0;
		}
		return false;
	}

	public bool StartedBeingUsedContinuously(ushort channel)
	{
		if (ContinuousActivationTracker.TryGetValue(channel, out var value) && value.Value > 0)
		{
			return value.PreviousValue == 0;
		}
		return false;
	}

	public bool StoppedBeingUsedContinuously(ushort channel)
	{
		if (ContinuousActivationTracker.TryGetValue(channel, out var value) && value.Value == 0)
		{
			return value.PreviousValue > 0;
		}
		return false;
	}

	public bool IsBeingUsedContinuously()
	{
		for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
		{
			if (IsBeingUsedContinuously(ActivationPropagation.AllChannels[i]))
			{
				return true;
			}
		}
		return false;
	}

	public bool WasBeingUsedContinuously()
	{
		for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
		{
			if (WasBeingUsedContinuously(ActivationPropagation.AllChannels[i]))
			{
				return true;
			}
		}
		return false;
	}

	public bool StartedBeingUsedContinuously()
	{
		for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
		{
			if (StartedBeingUsedContinuously(ActivationPropagation.AllChannels[i]))
			{
				return true;
			}
		}
		return false;
	}

	public bool StoppedBeingUsedContinuously()
	{
		for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
		{
			if (StoppedBeingUsedContinuously(ActivationPropagation.AllChannels[i]))
			{
				return true;
			}
		}
		return false;
	}

	public float GetHeatCapacity()
	{
		if (!SimulateTemperature)
		{
			return 1f;
		}
		return ObjectArea * Mathf.Max(0.1f, 0.025f * rigidbody.mass);
	}

	public float GetChargeWithWireResistance()
	{
		return Charge * EnergyWireResistance;
	}

	public Vector2 GetPreviousVel()
	{
		return previousVelocity;
	}

	public float GetScaledCircumference()
	{
		return Mathf.Abs(Circumference) * Mathf.Sqrt(Mathf.Abs(base.transform.localScale.x) * Mathf.Abs(base.transform.localScale.y));
	}

	private void Awake()
	{
		ActivationNode = base.gameObject.GetOrAddComponent<NodeBehaviour>();
		SortingGroup = GetComponentInParent<SortingGroup>();
		ContinuousActivationTracker = new Dictionary<ushort, DeltaInt>();
		for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
		{
			ContinuousActivationTracker.Add(ActivationPropagation.AllChannels[i], new DeltaInt(0));
		}
		if (base.gameObject.isStatic)
		{
			PlaySliderSound = false;
		}
		if (!base.enabled || !base.gameObject.activeInHierarchy)
		{
			isDisintegrated = true;
		}
		Temperature = AmbientTemperature;
		spriteRenderer = GetComponent<SpriteRenderer>();
		LocalFirePropagationMap = GetComponent<LocalFirePropagationMap>();
		InitialBounds = spriteRenderer.bounds;
		rigidbody = GetComponent<Rigidbody2D>();
		TrueInitialMass = rigidbody.mass;
		rigidbody.useAutoMass = false;
		ContextMenuOptions = base.gameObject.AddComponent<ContextMenuOptionComponent>();
		colliders = (from c in GetComponents<Collider2D>()
			where !c.isTrigger
			select c).ToArray();
	}

	public void HaltNextPropagation()
	{
		StartCoroutine(waitOneFrame());
		IEnumerator waitOneFrame()
		{
			yield return new WaitForSeconds(0.016f);
			HaltPropagation();
		}
	}

	public void HaltPropagation()
	{
		ActivationPropagationDelay = -1f;
	}

	private void OnAfterDeserialise(List<GameObject> gameobjects)
	{
		IEnumerable<SerialisableIdentity> source = gameobjects.SelectMany((GameObject c) => c.GetComponentsInChildren<SerialisableIdentity>());
		int i;
		for (i = 0; i < serialisable_victims.Length; i++)
		{
			SerialisableIdentity serialisableIdentity = source.FirstOrDefault((SerialisableIdentity s) => s.UniqueIdentity == serialisable_victims[i]);
			if (!serialisableIdentity || !serialisableIdentity.TryGetComponent<PhysicalBehaviour>(out var component))
			{
				Debug.LogWarning($"Stab victim with ID {serialisable_victims[i]} does not exist");
				continue;
			}
			if (!component || component == null)
			{
				Debug.LogWarning("Stab victim is null");
				continue;
			}
			Penetration penetration = new Penetration
			{
				Active = serialisable_active[i],
				CurrentConnectedAnchor = serialisable_anchors[i],
				Axis = serialisable_axes[i],
				Duration = serialisable_durations[i],
				Stabber = this,
				Victim = component
			};
			DoStab(penetration.Victim, penetration.Axis, penetration);
		}
	}

	public void ResetColliderArray()
	{
		colliders = (from c in GetComponents<Collider2D>()
			where !c.isTrigger
			select c).ToArray();
	}

	public void ClearAllDecals()
	{
		if (decalControllers == null || !decalControllers.Any())
		{
			return;
		}
		foreach (DecalControllerBehaviour decalController in decalControllers)
		{
			decalController.Clear();
		}
	}

	public Vector2 GetClosestPointTo(Vector2 point)
	{
		float num = float.MaxValue;
		Vector3 vector = base.transform.position;
		for (int i = 0; i < colliders.Length; i++)
		{
			Collider2D collider2D = colliders[i];
			if ((bool)collider2D)
			{
				Vector2 vector2 = collider2D.ClosestPoint(point);
				float sqrMagnitude = (vector2 - point).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					vector = vector2;
				}
			}
		}
		return vector;
	}

	private void Start()
	{
		if (!GetComponentInParent<AudioSourceTimeScaleBehaviour>())
		{
			base.transform.root.gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
		}
		if (!Properties)
		{
			throw new UnityException(base.name + " does not have PhysicalProperties assigned");
		}
		if (rigidbody.bodyType == RigidbodyType2D.Dynamic)
		{
			Vector2[] localColliderGridPoints = LocalColliderGridPoints;
			if (localColliderGridPoints == null || localColliderGridPoints.Length == 0)
			{
				BakeColliderGridPoints();
			}
		}
		decalControllers.AddRange(GetComponents<DecalControllerBehaviour>());
		UnityEngine.Random.InitState(GetHashCode());
		seed = UnityEngine.Random.Range(-1500f, 1500f);
		intSeed = UnityEngine.Random.Range(-1500, 1500);
		affectSurroundingTimer = Utils.Mod(seed, 2f);
		InitialGravityScale = rigidbody.gravityScale;
		ObjectArea = Mathf.Sqrt(spriteRenderer.bounds.extents.sqrMagnitude * 4f);
		Bounds bounds = new Bounds(base.transform.position, Vector3.zero);
		Collider2D[] array = colliders;
		foreach (Collider2D collider2D in array)
		{
			bounds.Encapsulate(collider2D.bounds);
		}
		rigidbody.interpolation = Global.CurrentInterpolationMode;
		InitialMass = rigidbody.mass;
		if (!base.gameObject.isStatic)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.dopplerLevel = 0f;
			audioSource.playOnAwake = false;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.minDistance = 1f + SoundVolumeBoost;
			audioSource.maxDistance = 30f + SoundVolumeBoost;
			audioSource.spatialBlend = 1f;
			audioSource.enabled = false;
			if (PlaySliderSound)
			{
				slidingAudioSource = base.gameObject.AddComponent<AudioSource>();
				slidingAudioSource.dopplerLevel = 0f;
				slidingAudioSource.playOnAwake = false;
				slidingAudioSource.rolloffMode = AudioRolloffMode.Linear;
				slidingAudioSource.minDistance = 0.1f;
				slidingAudioSource.maxDistance = 20f;
				slidingAudioSource.volume = 0f;
				slidingAudioSource.spatialBlend = 1f;
				slidingAudioSource.loop = true;
				slidingAudioSource.enabled = false;
				slidingAudioSource.clip = Properties.SlidingLoop;
			}
			SetProperties();
			CreateOutlineObject();
		}
		ShowOutline = false;
		CollisionQuality collisionQuality = UserPreferenceManager.Current.CollisionQuality;
		if (collisionQuality != 0 && collisionQuality == CollisionQuality.Continuous)
		{
			rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		}
		else
		{
			rigidbody.collisionDetectionMode = (ForceContinuous ? CollisionDetectionMode2D.Continuous : CollisionDetectionMode2D.None);
		}
		Global.main.PhysicalObjectsInWorld.Add(this);
		if (!Global.main.PhysicalObjectsInWorldByTransform.ContainsKey(base.transform))
		{
			Global.main.PhysicalObjectsInWorldByTransform.Add(base.transform, this);
		}
		RecalculateMassBasedOnSize();
		if (IsWeightless)
		{
			MakeWeightless();
		}
		if (isDisintegrated)
		{
			Disintegrate();
		}
		SetupSpawnPrefab();
		ContextMenuOptions.Buttons.Add(new ContextMenuButton("setTemperature", "Set temperature", "Set object temperature", delegate
		{
			Utils.OpenFloatInputDialog(Utils.CelsiusToPreference((SelectionController.Main.SelectedObjects.Count > 1) ? AmbientTemperature : Temperature), this, delegate(PhysicalBehaviour phys, float temp)
			{
				phys.Temperature = Mathf.Clamp(Utils.PreferenceToCelsius(temp), -273f, 1E+09f);
			}, "Selection target temperature", "Target temperature in " + UserPreferenceManager.Current.TemperatureUnit);
		}));
		ContextMenuOptions.Buttons.Add(new ContextMenuButton("setAngle", "Set angle", "Set exact rotation", delegate
		{
			if (SelectionController.Main.SelectedObjects.Count > 1)
			{
				if (SelectionController.Main.SelectedObjects[0] == this)
				{
					PhysicalBehaviour[] old = SelectionController.Main.SelectedObjects.ToArray();
					DialogBox dialog2 = null;
					dialog2 = DialogBoxManager.TextEntry("Angle in degrees", "Angle in degrees", new DialogButton("Apply", true, delegate
					{
						PhysicalBehaviour[] array2 = old;
						foreach (PhysicalBehaviour phys2 in array2)
						{
							setAngle(dialog2, phys2);
						}
					}), new DialogButton("Cancel", true));
					dialog2.InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
					dialog2.InputField.textComponent.text = "0";
				}
			}
			else
			{
				DialogBox dialog = null;
				dialog = DialogBoxManager.TextEntry("Angle in degrees", "Angle in degrees", new DialogButton("Apply", true, delegate
				{
					setAngle(dialog, this);
				}), new DialogButton("Cancel", true));
				dialog.InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
				dialog.InputField.text = "0";
			}
		}));
		for (int j = 0; j < LocalGridPointLength; j++)
		{
			LocalColliderGridPoints[j] += UnityEngine.Random.insideUnitCircle * 0.02f;
		}
		if (Circumference < 0f)
		{
			CalculateCircumference();
		}
		if (Properties.SparksOnSlide)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Global.main.MetalScrapePrefab, base.transform);
			scrapeParticles = gameObject.GetComponent<ParticleSystem>();
		}
		void setAngle(DialogBox d, PhysicalBehaviour phys)
		{
			if (float.TryParse(d.InputField.text, out var result))
			{
				phys.transform.eulerAngles = new Vector3(0f, 0f, result);
			}
		}
	}

	[Button("Calculate circumference", EButtonEnableMode.Always)]
	public void CalculateCircumference()
	{
		if (LocalColliderGridPoints.Length == 0)
		{
			BakeColliderGridPoints();
		}
		List<Vector2> list = new List<Vector2>();
		if (LocalColliderGridPoints.Length == 0)
		{
			Circumference = 0f;
			return;
		}
		Utils.ComputeConvexHull(LocalColliderGridPoints, list);
		if (!list.Any())
		{
			Circumference = 0f;
			return;
		}
		float num = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			Vector2 a = list[i];
			Vector2 b = list[(i + 1) % list.Count];
			num += Vector2.Distance(a, b);
		}
		Circumference = num;
	}

	public void RecalculateMassBasedOnSize()
	{
		InitialMass = TrueInitialMass * Mathf.Abs(base.transform.lossyScale.x) * Mathf.Abs(base.transform.lossyScale.y);
		if (!IsWeightless)
		{
			rigidbody.mass = InitialMass;
		}
	}

	public void Decal(DecalInstruction instruction)
	{
		if (DisplayBloodDecals && UserPreferenceManager.Current.Decals && !decalControllers.Any((DecalControllerBehaviour e) => e.DecalDescriptor == instruction.type))
		{
			DecalControllerBehaviour decalControllerBehaviour = base.gameObject.AddComponent<DecalControllerBehaviour>();
			decalControllerBehaviour.DecalDescriptor = instruction.type;
			decalControllerBehaviour.Decal(instruction);
			decalControllers.Add(decalControllerBehaviour);
		}
	}

	public float GetRelativeStabSpeed(PhysicalBehaviour victim)
	{
		foreach (Penetration penetration in penetrations)
		{
			if (penetration.Victim == victim)
			{
				return Mathf.Abs(penetration.Joint.jointSpeed);
			}
		}
		return 0f;
	}

	public Vector2 GetGlobalStabPoint(PhysicalBehaviour victim)
	{
		foreach (Penetration penetration in penetrations)
		{
			if (penetration.Victim == victim)
			{
				return victim.transform.TransformPoint(penetration.Joint.connectedAnchor);
			}
		}
		return default(Vector2);
	}

	private void CreateOutlineObject()
	{
		selectionOutlineObject = new GameObject("Outline");
		selectionOutlineObject.AddComponent<Optout>();
		selectionOutlineObject.layer = 16;
		SpriteRenderer obj = selectionOutlineObject.AddComponent<SpriteRenderer>();
		obj.sortingLayerName = "Top";
		obj.sortingOrder = int.MaxValue;
		obj.transform.SetParent(base.transform, worldPositionStays: false);
		obj.sharedMaterial = Global.main.SelectionOutlineMaterial;
		RefreshOutline();
	}

	public void RefreshOutline()
	{
		if (!selectionOutlineObject)
		{
			Debug.LogWarning("Attempt to refresh outline before outline initialisation");
			return;
		}
		outlineSpriteRenderer = selectionOutlineObject.GetComponent<SpriteRenderer>();
		outlineSpriteRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
		propertyBlock = new MaterialPropertyBlock();
		outlineSpriteRenderer.GetPropertyBlock(propertyBlock);
		Vector2 vector = new Vector2(outlineSpriteRenderer.sprite.texture.width, outlineSpriteRenderer.sprite.texture.height);
		Vector2 min = Utils.GetMin(outlineSpriteRenderer.sprite.uv, (Vector2 v) => v.sqrMagnitude);
		Vector2 vector2 = new Vector2(outlineSpriteRenderer.sprite.rect.width, outlineSpriteRenderer.sprite.rect.height);
		Vector4 value = new Vector4(min.x, min.y, vector2.x / vector.x, vector2.y / vector.y);
		propertyBlock.SetVector(ShaderProperties.Get("_AtlasTransform"), value);
		propertyBlock.SetVector(ShaderProperties.Get("_ObjectScale"), Vector4.one);
		propertyBlock.SetTexture(ShaderProperties.Get("_MainTex"), outlineSpriteRenderer.sprite.texture);
		outlineSpriteRenderer.SetPropertyBlock(propertyBlock);
	}

	private void SetupFirePrefab()
	{
		dirtyFire = true;
		FireParticlePrefab = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/FireParticle"), base.transform.position, Quaternion.identity, base.transform);
		FireParticlePrefab.AddComponent<Optout>();
		FireParticleSystem = FireParticlePrefab.GetComponent<ParticleSystem>();
		ParticleSystem.ShapeModule shape = FireParticleSystem.shape;
		if ((bool)LocalFirePropagationMap)
		{
			shape.shapeType = ParticleSystemShapeType.Circle;
		}
		else
		{
			shape.spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}

	private void SetupChargedPrefab()
	{
		dirtyCharged = true;
		ChargedParticlePrefab = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/ChargedParticles"), base.transform.position, Quaternion.identity, base.transform);
		ChargedParticlePrefab.AddComponent<Optout>();
		ChargedParticleSystem = ChargedParticlePrefab.GetComponent<ParticleSystem>();
		ParticleSystem.ShapeModule shape = ChargedParticleSystem.shape;
		shape.spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void SetupSmokePrefab()
	{
		dirtySmoke = true;
		SmokeParticlePrefab = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/SmokeParticle"), base.transform.position, Quaternion.identity, base.transform);
		SmokeParticlePrefab.AddComponent<Optout>();
		SmokeParticleSystem = SmokeParticlePrefab.GetComponent<ParticleSystem>();
		ParticleSystem.ShapeModule shape = SmokeParticleSystem.shape;
		shape.spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void SetupSpawnPrefab()
	{
		if (SpawnSpawnParticles && !base.gameObject.isStatic)
		{
			GameObject obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/SpawnParticles"), base.transform.position, Quaternion.identity, base.transform);
			obj.AddComponent<Optout>();
			ParticleSystem.ShapeModule shape = obj.GetComponent<ParticleSystem>().shape;
			shape.spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}

	private void SetupBubblePrefab()
	{
		dirtyBubble = true;
		BubbleParticlePrefab = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/BubbleVFX"), base.transform.position, Quaternion.identity, base.transform);
		BubbleParticlePrefab.AddComponent<Optout>();
		BubbleParticleSystem = BubbleParticlePrefab.GetComponent<ParticleSystem>();
		ParticleSystem.ShapeModule shape = BubbleParticleSystem.shape;
		shape.spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void SetProperties()
	{
		if (!rigidbody.sharedMaterial)
		{
			rigidbody.sharedMaterial = Properties.PhysicMaterial;
		}
	}

	public void ManagedUpdate()
	{
		float deltaTime = Time.deltaTime;
		if ((bool)audioSource && audioSource.enabled && !audioSource.isPlaying)
		{
			audioSource.enabled = false;
		}
		if (sizzleAudioHeat > 0f)
		{
			sizzleAudioHeat -= deltaTime;
		}
		if (base.gameObject.isStatic)
		{
			return;
		}
		if (BurnProgress < 0.3f && ChargeBurns)
		{
			BurnProgress += Properties.Burnrate * Charge * 0.01f * Time.timeScale;
		}
		Wetness = Mathf.Clamp01(Wetness - deltaTime);
		if (OnFire)
		{
			BurnIntensity += deltaTime * 0.1f;
			if (BurnProgress >= 0.995f)
			{
				OnFire = false;
			}
			BurnProgress = Mathf.Clamp01(BurnProgress);
			if ((double)Wetness > 0.1)
			{
				Extinguish();
				BurnIntensity = 0f;
			}
		}
		else
		{
			BurnIntensity -= deltaTime * 0.1f;
		}
		BurnIntensity = Mathf.Clamp01(BurnIntensity);
		SetParticleEmission();
		affectSurroundingTimer += deltaTime;
		if (affectSurroundingTimer > 1f)
		{
			affectSurroundingTimer = 0f;
			AffectSurroundings();
			if (Deletable && rigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				Vector3 position = base.transform.position;
				if (Global.main.CameraControlBehaviour.BoundingBox.ContainsExpanded(new Vector3(position.x, position.y, 0f), 50f))
				{
					outOfBoundsDuration = 0f;
				}
				else
				{
					outOfBoundsDuration += 1f;
					if (outOfBoundsDuration >= 5f)
					{
						if ((bool)base.transform.parent)
						{
							if (!isDisintegrated && Disintegratable)
							{
								Disintegrate();
							}
						}
						else
						{
							UnityEngine.Object.Destroy(base.transform.root.gameObject);
						}
					}
				}
			}
		}
		if (penetrations.Count > 0)
		{
			for (int num = penetrations.Count - 1; num >= 0; num--)
			{
				if (!penetrations[num].Active)
				{
					penetrations.RemoveAt(num);
				}
			}
			foreach (Penetration penetration in penetrations)
			{
				penetration.Duration += deltaTime;
			}
		}
		HandleSlidingSounds();
	}

	public void ManagedLateUpdate()
	{
		if (!base.gameObject.isStatic)
		{
			previousVelocity = rigidbody.velocity;
		}
	}

	public void ManagedFixedUpdate()
	{
		if (base.gameObject.isStatic)
		{
			return;
		}
		Charge *= 0.95f;
		if (UserPreferenceManager.Current.CollisionQuality == CollisionQuality.Dynamic)
		{
			if (rigidbody.velocity.sqrMagnitude > Global.main.DynamicSqrdVelocityThreshold)
			{
				if (rigidbody.collisionDetectionMode != CollisionDetectionMode2D.Continuous)
				{
					rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
				}
			}
			else if (rigidbody.collisionDetectionMode != 0)
			{
				rigidbody.collisionDetectionMode = CollisionDetectionMode2D.None;
			}
		}
		if (SimulateTemperature)
		{
			if (AmbientTemperatureTransfer)
			{
				if (UserPreferenceManager.Current.AmbientTemperatureTransfer)
				{
					AmbientTemperatureGridBehaviour.Instance.TransferHeat(this);
				}
				else
				{
					Temperature = Mathf.Lerp(Temperature, AmbientTemperature, 0.0001f / GetHeatCapacity());
				}
			}
			if (OnFire && Temperature < Properties.BurningTemperatureThreshold * 3f)
			{
				Temperature = Mathf.Lerp(Temperature, Properties.BurningTemperatureThreshold * 3f, 0.01f * TemperatureWhenBurningLerpMultiplier);
			}
			bool flag = BurnProgress <= 0.98f;
			if (flag && Temperature > Properties.BurningTemperatureThreshold && Properties.Flammability > float.Epsilon)
			{
				if (BurnProgress < 0.2f)
				{
					BurnProgress += 0.001f;
				}
				if ((double)UnityEngine.Random.value > 0.9995 && Properties.Flammability > float.Epsilon)
				{
					Ignite(ignoreFlammability: true, base.transform.TransformPoint(GetRandomGridPoint()));
				}
			}
			if (OnFire)
			{
				BurnProgress += Mathf.Max(0f, Temperature - Properties.BurningTemperatureThreshold) * 6E-05f * (Properties.Burnrate / 2f) * BurningProgressionMultiplier;
			}
			if (Temperature < Properties.BurningTemperatureThreshold)
			{
				if (flag && OnFire)
				{
					BurnProgress += 2.5E-05f * (Properties.Burnrate / 2f) * BurningProgressionMultiplier;
				}
				if (OnFire && UnityEngine.Random.value > 0.995f)
				{
					Extinguish();
				}
			}
			Temperature += Charge * 0.0008f / Properties.HeatTransferSpeedMultiplier;
			Temperature = Mathf.Clamp(-273.15f, Temperature, 5000000f);
		}
		OnCollisionStayWithoutSleep();
		if (IsUnderWater && UnityEngine.Random.value > 0.95f && decalControllers.Any() && rigidbody.velocity.sqrMagnitude > 2f)
		{
			foreach (DecalControllerBehaviour decalController in decalControllers)
			{
				decalController.Clear();
			}
		}
		for (int i = 0; i < penetrations.Count; i++)
		{
			Penetration penetration = penetrations[i];
			if (penetration.Duration > Time.maximumDeltaTime)
			{
				HandleStabRelease(penetration);
			}
			if (!penetration.Stabber || !penetration.Victim)
			{
				continue;
			}
			if (penetration.Stabber.SimulateTemperature && penetration.Victim.SimulateTemperature)
			{
				if (!penetration.Active)
				{
					continue;
				}
				Utils.AverageTemperature(penetration.Victim, penetration.Stabber, 0.3f);
			}
			if (penetration.Victim.Charge * 0.95f > Charge)
			{
				Charge = penetration.Victim.Charge * 0.95f;
			}
			else if (Charge * 0.95f > penetration.Victim.Charge)
			{
				penetration.Victim.Charge = Charge * 0.95f;
			}
		}
		if (OnFire)
		{
			if (UnityEngine.Random.value > 0.9993f)
			{
				Extinguish();
			}
			Global.main.FireLoopSoundControllerBehaviour.FuzzySetVolumeAt(base.transform.position, Mathf.Clamp01(BurnIntensity));
		}
	}

	public void ForceSendUse()
	{
		ushort[] allChannels = ActivationPropagation.AllChannels;
		foreach (ushort channel in allChannels)
		{
			BroadcastMessage("Use", new ActivationPropagation(base.transform.root, channel, base.transform.root.gameObject), SendMessageOptions.DontRequireReceiver);
		}
	}

	public void Use(ActivationPropagation activation)
	{
		ModAPI.InvokeItemActivated(this, this);
	}

	private void HandleStabRelease(Penetration penetration)
	{
		if (!penetration.Active)
		{
			return;
		}
		if (!penetration.Victim)
		{
			DestroyStabJoint(penetration);
			return;
		}
		switch (GetCustomLimitState(penetration))
		{
		case JointLimitState2D.Inactive:
			penetration.Joint.breakForce = float.MaxValue;
			penetration.Joint.breakTorque = float.MaxValue;
			break;
		case JointLimitState2D.LowerLimit:
			if (penetration.Axis.LooseLowerLimit)
			{
				DestroyStabJoint(penetration);
			}
			break;
		case JointLimitState2D.UpperLimit:
			if (penetration.Axis.LooseUpperLimit)
			{
				DestroyStabJoint(penetration);
			}
			break;
		case JointLimitState2D.EqualLimits:
			Debug.LogWarning(Properties.name + " has a sharp axis with identical limits");
			break;
		}
	}

	private JointLimitState2D GetCustomLimitState(Penetration penetration)
	{
		if (!penetration.Active || !penetration.Joint)
		{
			return JointLimitState2D.Inactive;
		}
		float num = penetration.Joint.jointSpeed / 1000f * StabReleaseSpeedModifier;
		float num2 = num * num * num * num * num * num * num * num * 100f;
		if (num < 0f)
		{
			num2 *= -1f;
		}
		float num3 = penetration.Joint.jointTranslation * StabReleaseSpeedModifier * 1.05f + num2 * Time.fixedUnscaledDeltaTime;
		if (num3 > penetration.Axis.UpperLimit)
		{
			return JointLimitState2D.UpperLimit;
		}
		if (num3 < penetration.Axis.LowerLimit)
		{
			return JointLimitState2D.LowerLimit;
		}
		if (penetration.Axis.LowerLimit == penetration.Axis.UpperLimit)
		{
			return JointLimitState2D.EqualLimits;
		}
		return JointLimitState2D.Inactive;
	}

	private void DestroyStabJoint(Penetration penetration)
	{
		base.gameObject.SendMessage("Dislodged", penetration, SendMessageOptions.DontRequireReceiver);
		if ((bool)penetration.Victim)
		{
			Vector2 point = penetration.GetPoint();
			Vector2 normal = -penetration.GetDirection();
			penetration.Victim.SendMessage("Unstabbed", new Stabbing(penetration.Stabber, penetration.Victim, normal, point), SendMessageOptions.DontRequireReceiver);
		}
		penetration.Active = false;
		if ((bool)penetration.Joint)
		{
			penetration.Joint.breakForce = 0f;
			penetration.Joint.breakTorque = 0f;
			penetration.Joint.useLimits = false;
			penetration.Joint.useMotor = false;
			UndoPenetrationNoCollision(penetration);
		}
	}

	private void UndoPenetrationNoCollision(Penetration pen)
	{
		Collider2D b = pen.Stabber.colliders[0];
		foreach (Collider2D relevantCollider in pen.RelevantColliders)
		{
			if ((bool)relevantCollider)
			{
				IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(relevantCollider, b, ignore: false);
				ignoredColliders.Remove(relevantCollider);
			}
		}
	}

	internal Vector2 GetSharpEnd(SharpAxis sharpAxis)
	{
		Vector3 vector = base.transform.TransformVector(sharpAxis.Axis);
		if (sharpAxis.LooseLowerLimit)
		{
			return Vector2.LerpUnclamped(base.transform.position, base.transform.position + vector, sharpAxis.LowerLimit);
		}
		if (sharpAxis.LooseUpperLimit)
		{
			return Vector2.LerpUnclamped(base.transform.position, base.transform.position + vector, sharpAxis.UpperLimit);
		}
		Debug.LogError("Attempt to get sharp end on an object with no sharp ends...");
		return default(Vector2);
	}

	private bool IsPointNearLooseSharpEnd(Vector2 point)
	{
		SharpAxis[] sharpAxes = Properties.SharpAxes;
		for (int i = 0; i < sharpAxes.Length; i++)
		{
			SharpAxis sharpAxis = sharpAxes[i];
			Vector3 vector = base.transform.TransformVector(sharpAxis.Axis);
			if (sharpAxis.LooseLowerLimit && (Vector2.LerpUnclamped(base.transform.position, base.transform.position + vector, sharpAxis.LowerLimit) - point).sqrMagnitude < LooseEndSize * LooseEndSize)
			{
				return true;
			}
			if (sharpAxis.LooseUpperLimit && (Vector2.LerpUnclamped(base.transform.position, base.transform.position + vector, sharpAxis.UpperLimit) - point).sqrMagnitude < LooseEndSize * LooseEndSize)
			{
				return true;
			}
		}
		return false;
	}

	private void HandleStabbing(Collision2D collision, ContactPoint2D contact)
	{
		if (!Properties.Sharp || penetrations.Count >= MaximumStabVictims || !Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.transform, out var value) || (value.Properties.Softness <= 0.01f && !Properties.IgnoreStabResistance) || (value.Properties.IgnoreStabResistance && Properties.IgnoreStabResistance) || collision.relativeVelocity.magnitude / Properties.SharpForceThresholdMultiplier < Utils.MapRange(0.01f, 1f, 20f, 1.25f, Properties.IgnoreStabResistance ? 1f : Mathf.Sqrt(value.Properties.Softness)) || !IsPointNearLooseSharpEnd(contact.point))
		{
			return;
		}
		Vector2 normalized = collision.relativeVelocity.normalized;
		if (Vector2.Dot(contact.normal, normalized) < -0.98f)
		{
			return;
		}
		Penetration penetration = new Penetration();
		penetration.Stabber = this;
		bool flag = false;
		SharpAxis incomingSharpAxis = default(SharpAxis);
		SharpAxis[] sharpAxes = Properties.SharpAxes;
		for (int i = 0; i < sharpAxes.Length; i++)
		{
			SharpAxis axis = sharpAxes[i];
			Vector3 vector = base.transform.TransformVector(axis.Axis);
			Debug.DrawRay(contact.point, normalized, Color.red, 10f);
			if (Vector2.Dot(normalized, vector) < -0.92f && Vector2.Dot(contact.normal, vector) < -0.3f)
			{
				incomingSharpAxis = (penetration.Axis = axis);
				flag = true;
				break;
			}
		}
		if (flag && !penetration.Active)
		{
			penetration.Active = true;
			penetration.Duration = 0f;
			rigidbody.velocity = previousVelocity;
			if (value.rigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				value.rigidbody.velocity = value.previousVelocity;
			}
			Vector2 point = contact.point;
			point = collision.collider.ClosestPoint(point);
			Stabbing stabbing = new Stabbing(this, value, contact.normal, point);
			value.SendMessage("Stabbed", stabbing, SendMessageOptions.DontRequireReceiver);
			base.gameObject.SendMessage("Lodged", stabbing, SendMessageOptions.DontRequireReceiver);
			penetration.CurrentConnectedAnchor = value.transform.InverseTransformPoint(point);
			DoStab(value, incomingSharpAxis, penetration);
		}
	}

	internal void DoStab(PhysicalBehaviour other, SharpAxis incomingSharpAxis, Penetration penetration)
	{
		penetration.Active = true;
		penetration.Victim = other;
		penetration.Victim.stabWoundCount++;
		penetration.Victim.beingStabbedBy.Add(this);
		penetration.Victim.victimPenetrations.Add(penetration);
		SliderJoint2D sliderJoint2D = base.gameObject.AddComponent<SliderJoint2D>();
		sliderJoint2D.autoConfigureConnectedAnchor = false;
		sliderJoint2D.autoConfigureAngle = false;
		sliderJoint2D.anchor = Vector2.zero;
		sliderJoint2D.breakForce = 9999999f;
		sliderJoint2D.breakTorque = 9999999f;
		sliderJoint2D.connectedAnchor = penetration.CurrentConnectedAnchor;
		sliderJoint2D.connectedBody = other.rigidbody;
		sliderJoint2D.enableCollision = false;
		sliderJoint2D.useMotor = true;
		JointMotor2D motor = sliderJoint2D.motor;
		motor.maxMotorTorque = Properties.SharpForceThresholdMultiplier * ((1f - other.Properties.Softness) * 5f + 5f) * Properties.LodgeStrengthMultiplier;
		sliderJoint2D.motor = motor;
		sliderJoint2D.angle = Vector2.Angle(new Vector2(base.transform.root.localScale.x, 0f), incomingSharpAxis.Axis);
		sliderJoint2D.useLimits = true;
		sliderJoint2D.limits = new JointTranslationLimits2D
		{
			min = incomingSharpAxis.LowerLimit,
			max = incomingSharpAxis.UpperLimit
		};
		penetration.Joint = sliderJoint2D;
		Collider2D collider2D = colliders[0];
		Collider2D[] componentsInChildren = other.transform.root.GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider2D2 in componentsInChildren)
		{
			if (!(collider2D2 == null) && !(collider2D == null))
			{
				IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(collider2D2, collider2D);
				ignoredColliders.Add(collider2D2);
				penetration.RelevantColliders.Add(collider2D2);
			}
		}
		penetrations.Add(penetration);
	}

	private void AffectSurroundings()
	{
		if (base.gameObject.isStatic || (!OnFire && !(Charge > 0.001f)))
		{
			return;
		}
		foreach (PhysicalBehaviour item in Global.main.GetPhysicsObjectsNear(this))
		{
			if (item.IsUnderWater && IsUnderWater && Charge > item.Charge)
			{
				item.Charge = Charge;
			}
			if (OnFire && !item.OnFire && UnityEngine.Random.value > 0.7f)
			{
				item.Ignite();
			}
		}
	}

	private void SetParticleEmission()
	{
		if (base.gameObject.isStatic)
		{
			return;
		}
		if (dirtyFire && (bool)FireParticleSystem)
		{
			ParticleSystem.EmissionModule emission = FireParticleSystem.emission;
			ParticleSystem.MainModule main = FireParticleSystem.main;
			if ((bool)LocalFirePropagationMap)
			{
				emission.enabled = false;
				if (BurnIntensity > 0.01f)
				{
					float num = Mathf.Sqrt(spriteRenderer.bounds.extents.sqrMagnitude * 4f);
					float num2 = (1f - BurnProgress) * BurnIntensity * 2f * num;
					fireEmissionInterval.RateHz = 15f * (float)Mathf.Max(LocalFirePropagationMap.Points.Length, 25) * 0.06f;
					for (int i = 0; i < fireEmissionInterval.CalculateCycleCount(Time.deltaTime); i++)
					{
						int randomFireyIndex = LocalFirePropagationMap.GetRandomFireyIndex(0.05f);
						if (randomFireyIndex != -1)
						{
							float fireIntensity = LocalFirePropagationMap.Points[randomFireyIndex].FireIntensity;
							float num3 = fireIntensity * fireIntensity * fireIntensity * fireIntensity * fireIntensity * fireIntensity * num2;
							if (UnityEngine.Random.value < num3)
							{
								FireParticleSystem.Emit(new ParticleSystem.EmitParams
								{
									position = base.transform.TransformPoint(LocalFirePropagationMap.Points[randomFireyIndex].LocalPoint),
									startSize = Mathf.Min(num3 * 2f, 2.5f),
									startColor = Color.Lerp(Color.black, Color.white, num3)
								}, 1);
							}
						}
					}
				}
			}
			else if (BurnIntensity < 0.05f)
			{
				if (emission.enabled)
				{
					emission.enabled = false;
				}
			}
			else
			{
				emission.enabled = true;
				float num4 = (1f - BurnProgress) * BurnIntensity * 2f;
				float num5 = Mathf.Sqrt(spriteRenderer.bounds.extents.sqrMagnitude * 4f);
				num4 *= num5;
				float num6 = Mathf.Min(num4 * 2f, 2.5f);
				main.startSize = num6;
				main.startColor = Color.Lerp(Color.black, Color.white, num4);
				emission.rateOverTime = 12f * num5;
				emission.rateOverDistance = 1f * num5;
			}
		}
		else if (BurnIntensity > 0.05f)
		{
			SetupFirePrefab();
		}
		if (!dirtyBubble && (double)Wetness > 0.01)
		{
			SetupBubblePrefab();
		}
		if ((bool)BubbleParticleSystem)
		{
			if (Wetness > 0.1f)
			{
				if (!BubbleParticleSystem.isPlaying)
				{
					BubbleParticleSystem.Play();
				}
				ParticleSystem.EmissionModule emission2 = BubbleParticleSystem.emission;
				emission2.rateOverDistance = Wetness * 2f;
			}
			else if (BubbleParticleSystem.isPlaying)
			{
				BubbleParticleSystem.Stop();
			}
		}
		if (!ForceNoCharge && !ForceNoChargeParticles)
		{
			if (!dirtyCharged && Charge > 0.001f)
			{
				SetupChargedPrefab();
			}
			if (!dirtySmoke && Charge * Properties.Flammability > 0.001f)
			{
				SetupSmokePrefab();
			}
			if ((bool)ChargedParticleSystem)
			{
				ParticleSystem.EmissionModule emission3 = ChargedParticleSystem.emission;
				emission3.rateOverTime = Mathf.Min(Charge * 2f, 30f);
			}
			if ((bool)SmokeParticleSystem)
			{
				ParticleSystem.EmissionModule emission4 = SmokeParticleSystem.emission;
				emission4.rateOverTime = Mathf.Min(Charge * Properties.Flammability, 10f);
			}
		}
	}

	[ContextMenu("Ignite")]
	public void Ignite(bool ignoreFlammability = false)
	{
		IgniteInternal(ignoreFlammability);
		if (OnFire && (bool)LocalFirePropagationMap)
		{
			LocalFirePropagationMap.Points[UnityEngine.Random.Range(0, LocalFirePropagationMap.Points.Length)].FireIntensity = 1f;
		}
	}

	public void Ignite(bool ignoreFlammability, Vector2 position)
	{
		IgniteInternal(ignoreFlammability);
		if (OnFire && (bool)LocalFirePropagationMap)
		{
			LocalFirePropagationMap.IgniteAt(position);
		}
	}

	public void Ignite(Vector2 position)
	{
		Ignite(ignoreFlammability: false, position);
	}

	private void IgniteInternal(bool ignoreFlammability)
	{
		if (!base.gameObject.isStatic && (ignoreFlammability || !(Mathf.PerlinNoise(Time.time * 100f, seed + 235.42f) > Properties.Flammability)) && !(Properties.Flammability <= float.Epsilon))
		{
			OnFire = true;
		}
	}

	public void Extinguish()
	{
		Temperature = Mathf.Min(Temperature, Properties.BurningTemperatureThreshold);
		OnFire = false;
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (base.gameObject.isStatic || collision.transform == base.transform)
		{
			return;
		}
		CalculateSliding(collision);
		if (SimulateTemperature)
		{
			float num = 0.0001f * (Properties.PhysicMaterial ? (Properties.PhysicMaterial.friction * 2f) : 0.5f);
			Temperature += collision.relativeVelocity.sqrMagnitude * num;
		}
		if (UserPreferenceManager.Current.FancyEffects && !IsUnderWater && Properties.SparksOnSlide && (bool)spriteRenderer && (bool)scrapeParticles && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.transform, out var value) && value.Properties.Softness < 0.025f)
		{
			if (spriteRenderer.isVisible && collision.relativeVelocity.sqrMagnitude > 8f)
			{
				ContactPoint2D contact = collision.GetContact(0);
				scrapeParticles.transform.position = contact.point;
				scrapeParticles.Play(withChildren: true);
			}
			else
			{
				scrapeParticles.Stop(withChildren: true);
			}
		}
	}

	private bool TryGetFreeCollisionBufferIndex(out int index)
	{
		index = -1;
		for (int i = 0; i < onCollisionStayBuffer.Length; i++)
		{
			if (onCollisionStayBuffer[i] == null || !onCollisionStayBuffer[i].Active || !onCollisionStayBuffer[i].Coll.gameObject.activeSelf)
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	[Obsolete]
	public IEnumerable<(Collider2D coll, Vector2 point)> GetCurrentContacts()
	{
		if (onCollisionStayBuffer == null || onCollisionStayBuffer.Length == 0)
		{
			yield break;
		}
		Vector2 p = rigidbody.worldCenterOfMass;
		for (int i = 0; i < onCollisionStayBuffer.Length; i++)
		{
			ColliderBoolPair colliderBoolPair = onCollisionStayBuffer[i];
			if (colliderBoolPair != null && colliderBoolPair.Active && colliderBoolPair.Coll.gameObject.activeSelf)
			{
				Vector2 position = ((!colliderBoolPair.Coll.attachedRigidbody) ? ((p + (Vector2)colliderBoolPair.Coll.transform.position) * 0.5f) : ((p + colliderBoolPair.Coll.attachedRigidbody.centerOfMass) * 0.5f));
				yield return (colliderBoolPair.Coll, colliderBoolPair.Coll.ClosestPoint(position));
			}
		}
	}

	private void OnCollisionStayWithoutSleep()
	{
		if (base.gameObject.isStatic)
		{
			return;
		}
		for (int i = 0; i < onCollisionStayBuffer.Length; i++)
		{
			ColliderBoolPair colliderBoolPair = onCollisionStayBuffer[i];
			if (colliderBoolPair == null || !colliderBoolPair.Active)
			{
				continue;
			}
			if (!colliderBoolPair.Coll || !colliderBoolPair.Coll.gameObject.activeSelf)
			{
				colliderBoolPair.Active = false;
				continue;
			}
			Collider2D coll = colliderBoolPair.Coll;
			if (!Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(coll.transform, out var value))
			{
				break;
			}
			if (SimulateTemperature && value.SimulateTemperature)
			{
				Utils.AverageTemperature(this, value);
			}
			if (!ForceNoCharge && Charge > value.Charge && ConductOverride && value.ConductOverride && value.Properties.Conducting && Properties.Conducting)
			{
				value.Charge = Mathf.Lerp(value.Charge, Charge, 0.8f);
			}
		}
	}

	private void HandleSlidingSounds()
	{
		if (!slidingAudioSource)
		{
			return;
		}
		if (base.gameObject.isStatic || rigidbody.bodyType != 0)
		{
			stopSource();
		}
		else if (isSliding)
		{
			if (!slidingAudioSource.enabled)
			{
				startSource();
			}
			if (PlaySliderSound && slidingAudioSource.isPlaying)
			{
				slidingAudioSource.volume = Mathf.Lerp(slidingAudioSource.volume, isSliding ? slideIntensity : 0f, 0.5f);
			}
		}
		else if (slidingAudioSource.enabled)
		{
			stopSource();
		}
		void startSource()
		{
			slidingAudioSource.enabled = true;
			slidingAudioSource.Play();
		}
		void stopSource()
		{
			slidingAudioSource.Stop();
			slidingAudioSource.enabled = false;
		}
	}

	private void CalculateSliding(Collision2D collision)
	{
		if (base.gameObject.isStatic)
		{
			return;
		}
		isSliding = false;
		if (rigidbody.bodyType != 0)
		{
			return;
		}
		float num = ((!collision.rigidbody) ? rigidbody.velocity.magnitude : (rigidbody.velocity - collision.rigidbody.velocity).magnitude);
		float num2 = Mathf.Abs(rigidbody.angularVelocity) * 0.025f;
		if (!(Mathf.Max(num, num2) <= 0.05f))
		{
			float a = ((num2 > 0.05f && num < 2f) ? num2 : 0f);
			float b = ((num2 < 2f && num > 0.05f) ? num : 0f);
			slideIntensity = Mathf.Max(a, b) / 40f;
			if (!((double)slideIntensity < 0.1))
			{
				isSliding = true;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (base.gameObject.isStatic)
		{
			return;
		}
		if (collision.transform != base.transform && TryGetFreeCollisionBufferIndex(out var index))
		{
			onCollisionStayBuffer[index] = new ColliderBoolPair(active: true, collision.collider);
		}
		int contacts = collision.GetContacts(contactBuffer);
		if (contacts == 0)
		{
			return;
		}
		ContactPoint2D firstValidContact = Utils.GetFirstValidContact(contactBuffer, contacts);
		float averageImpulseRemoveOutliers = Utils.GetAverageImpulseRemoveOutliers(contactBuffer, contacts);
		CameraShakeBehaviour.main.Shake(averageImpulseRemoveOutliers * 0.01f, base.transform.position);
		HandleSounds(collision, averageImpulseRemoveOutliers);
		HandleStabbing(collision, firstValidContact);
		if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.transform, out var value))
		{
			if (Properties.SizzleSounds != null && value.SimulateTemperature && SimulateTemperature && Mathf.Abs(value.Temperature - Temperature) > 10f && value.Temperature > Temperature && value.Temperature >= Properties.BurningTemperatureThreshold)
			{
				Sizzle();
			}
			if (Application.isMobilePlatform)
			{
				Application.Quit();
			}
			if (!(UnityEngine.Random.value > 0.5f) && Charge > value.Charge && ConductOverride && value.ConductOverride && value.Properties.Conducting && Properties.Conducting && Mathf.Abs(Charge - value.Charge) > 15f)
			{
				value.rigidbody.AddForce(0.2f * Mathf.Abs(Charge - value.Charge) * (value.transform.position - base.transform.position).normalized, ForceMode2D.Impulse);
				value.Charge = Charge;
				UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/BigZap"), firstValidContact.point, Quaternion.identity);
			}
		}
	}

	public void Sizzle(bool withSound = true)
	{
		if (!dirtySmoke)
		{
			SetupSmokePrefab();
		}
		SmokeParticleSystem.Emit(5);
		if (withSound && Properties.SizzleSounds != null && Properties.SizzleSounds.Length != 0 && sizzleAudioHeat <= float.Epsilon)
		{
			PlayClipOnce(Properties.SizzleSounds.PickRandom(), 0.5f);
			sizzleAudioHeat = 1f;
		}
	}

	public Vector2 GetWorldCenterOfMass()
	{
		if (rigidbody.bodyType == RigidbodyType2D.Dynamic)
		{
			return rigidbody.worldCenterOfMass;
		}
		return base.transform.position;
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if ((bool)scrapeParticles)
		{
			scrapeParticles.Stop();
		}
		if (base.gameObject.isStatic)
		{
			return;
		}
		if (collision != null && (bool)collision.rigidbody)
		{
			ColliderBoolPair[] array = onCollisionStayBuffer;
			foreach (ColliderBoolPair colliderBoolPair in array)
			{
				if (colliderBoolPair != null && colliderBoolPair.Coll == collision.collider)
				{
					colliderBoolPair.Active = false;
				}
			}
		}
		isSliding = false;
		if (PlaySliderSound && (bool)slidingAudioSource)
		{
			slidingAudioSource.Stop();
		}
	}

	private void HandleSounds(Collision2D collision, float impulse)
	{
		if (!rigidbody || !audioSource)
		{
			return;
		}
		float num = impulse * Properties.ImpactIntensityMutliplier;
		if (num < 0.002f)
		{
			return;
		}
		float num2 = Mathf.Max(1f, rigidbody.mass);
		bool flag = num > 0.5f / num2;
		float num3 = num * 0.3f / num2 * Properties.HitVolumeMultiplier;
		if (UserPreferenceManager.Current.ClampVolume)
		{
			num3 = Mathf.Clamp01(num3);
		}
		AudioClip[] overrideImpactSounds = OverrideImpactSounds;
		if (overrideImpactSounds != null && overrideImpactSounds.Length != 0)
		{
			PlayClipOnce(OverrideImpactSounds.PickRandom(), num3);
		}
		else if (flag)
		{
			if ((bool)audioSource)
			{
				AudioClip[] hardImpact = Properties.HardImpact;
				if (hardImpact == null || hardImpact.Length != 0)
				{
					PlayClipOnce(Properties.HardImpact.PickRandom(), num3);
				}
			}
		}
		else if ((bool)audioSource)
		{
			AudioClip[] softImpact = Properties.SoftImpact;
			if (softImpact == null || softImpact.Length != 0)
			{
				PlayClipOnce(Properties.SoftImpact.PickRandom(), num3);
			}
		}
	}

	private void Shocked(Zap zap)
	{
		Charge += Time.deltaTime * 25f;
		if (UnityEngine.Random.value > 0.9999f)
		{
			Ignite(zap.position);
		}
	}

	public void Shot(Shot shot)
	{
		if (OverrideShotSounds != null && OverrideShotSounds.Length != 0)
		{
			PlayClipOnce(OverrideShotSounds.PickRandom(), 1f);
		}
		if ((bool)Properties && (bool)Properties.ShotImpact)
		{
			CreateImpactEffect(shot, Mathf.Clamp(Mathf.Pow(shot.damage, 0.6f) / 6f, 0.1f, 3f) / 2f);
		}
	}

	public void ExitShot(Shot shot)
	{
		if ((bool)Properties && (bool)Properties.ShotImpact)
		{
			CreateImpactEffect(shot, Mathf.Clamp(Mathf.Pow(shot.damage, 0.6f) / 3f, 0.1f, 4f));
		}
	}

	public void CreateImpactEffect(Shot shot, float size)
	{
		CreateImpactEffect(shot.point, shot.normal, size);
	}

	public void CreateImpactEffect(Vector2 position, Vector2 direction, float size)
	{
		float z = Mathf.Atan2(direction.y, direction.x) * 57.29578f - 90f;
		GameObject gameObject = (Properties.ShotImpact.CompareTag("Poolable") ? PoolGenerator.Instance.RequestPrefab(Properties.ShotImpact, position) : UnityEngine.Object.Instantiate(Properties.ShotImpact, position, Quaternion.identity));
		if ((bool)gameObject)
		{
			gameObject.transform.localScale *= size;
			gameObject.transform.eulerAngles = new Vector3(0f, 0f, z);
			base.gameObject.BroadcastMessage("OnImpactCreated", gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void Stabbed(Stabbing stabbing)
	{
		if (Properties.StabSound != null && Properties.StabSound.Length != 0)
		{
			PlayClipOnce(Properties.StabSound.PickRandom(), 1f);
		}
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		foreach (Penetration penetration in penetrations)
		{
			if (!(penetration.Joint != joint) && (bool)penetration.Joint)
			{
				penetration.Victim.stabWoundCount--;
				penetration.Active = false;
				penetration.Victim.beingStabbedBy.Remove(this);
				penetration.Victim.victimPenetrations.Remove(penetration);
				UndoPenetrationNoCollision(penetration);
			}
		}
	}

	public void PlayClipOnce(AudioClip clip, float volume)
	{
		if (!isDisintegrated && (bool)clip && (bool)audioSource)
		{
			audioSource.enabled = true;
			audioSource.PlayOneShot(clip, UserPreferenceManager.Current.ClampVolume ? Mathf.Clamp01(volume) : volume);
		}
	}

	public void PlayClipOnce(AudioClip clip)
	{
		PlayClipOnce(clip, 1f);
	}

	private void OnDestroy()
	{
		foreach (Penetration penetration in penetrations)
		{
			DestroyStabJoint(penetration);
		}
		Global.main.PhysicalObjectsInWorld.Remove(this);
		Global.main.PhysicalObjectsInWorldByTransform.Remove(base.transform);
		if (Global.main.CameraControlBehaviour.CurrentlyFollowing.Contains(this))
		{
			Global.main.CameraControlBehaviour.CurrentlyFollowing.Remove(this);
		}
		ColliderBoolPair[] array = onCollisionStayBuffer;
		foreach (ColliderBoolPair colliderBoolPair in array)
		{
			if (colliderBoolPair != null && colliderBoolPair.Active && (bool)colliderBoolPair.Coll)
			{
				colliderBoolPair.Coll.SendMessage("OnCollisionExit2D", new Collision2D(), SendMessageOptions.DontRequireReceiver);
			}
		}
		InvokeOnItemRemoved();
	}

	private void OnDisable()
	{
		ColliderBoolPair[] array = onCollisionStayBuffer;
		foreach (ColliderBoolPair colliderBoolPair in array)
		{
			if (colliderBoolPair != null && colliderBoolPair.Active && (bool)colliderBoolPair.Coll)
			{
				colliderBoolPair.Coll.SendMessage("OnCollisionExit2D", new Collision2D(), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	[ContextMenu("Bake collider grid points")]
	[Button("Bake collider grid points", EButtonEnableMode.Always)]
	public void BakeColliderGridPoints()
	{
		List<Vector2> list = new List<Vector2>();
		HashSet<Vector2> hashSet = new HashSet<Vector2>();
		Collider2D[] array = (from c in GetComponents<Collider2D>()
			where !c.isTrigger
			select c).ToArray();
		Bounds bounds = new Bounds(base.transform.position, Vector3.zero);
		Collider2D[] array2 = array;
		foreach (Collider2D collider2D in array2)
		{
			bounds.Encapsulate(collider2D.bounds);
		}
		bounds.size *= 0.999f;
		int num = Mathf.Max(1, GridRes);
		Vector2 vector = new Vector2(bounds.size.x / (float)num, bounds.size.y / (float)num);
		Vector2 vector2 = bounds.min;
		for (int j = 0; j < num + 1; j++)
		{
			for (int k = 0; k < num + 1; k++)
			{
				Vector2 pos = vector2 + new Vector2((float)k * vector.x, (float)j * vector.y);
				if (!hashSet.Any((Vector2 p) => Vector2.Distance(p, pos) < 0.025f))
				{
					hashSet.Add(pos);
					if (IsInsideOf(array, pos))
					{
						list.Add(base.transform.InverseTransformPoint(pos));
					}
				}
			}
		}
		array2 = array;
		foreach (Collider2D collider2D2 in array2)
		{
			PolygonCollider2D poll = collider2D2 as PolygonCollider2D;
			if ((object)poll != null)
			{
				list.AddRange(poll.points.Select((Vector2 p) => p + poll.offset));
				continue;
			}
			BoxCollider2D boxCollider2D = collider2D2 as BoxCollider2D;
			if ((object)boxCollider2D != null)
			{
				Vector2 vector3 = boxCollider2D.size / 2f;
				list.Add(new Vector2(vector3.x, vector3.y) + boxCollider2D.offset);
				list.Add(new Vector2(0f - vector3.x, vector3.y) + boxCollider2D.offset);
				list.Add(new Vector2(0f - vector3.x, 0f - vector3.y) + boxCollider2D.offset);
				list.Add(new Vector2(vector3.x, 0f - vector3.y) + boxCollider2D.offset);
				continue;
			}
			CircleCollider2D circleCollider2D = collider2D2 as CircleCollider2D;
			if ((object)circleCollider2D != null)
			{
				int num2 = Mathf.Clamp(Mathf.CeilToInt((float)Math.PI * 2f * circleCollider2D.radius / 0.25f), 6, 32);
				for (int l = 0; l < num2; l++)
				{
					float f = (float)Math.PI * 2f * ((float)l / (float)num2 - 1f);
					Vector2 vector4 = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * circleCollider2D.radius;
					list.Add(vector4 + circleCollider2D.offset);
				}
			}
		}
		LocalColliderGridPoints = list.ToArray();
		Debug.DrawLine(bounds.center - bounds.size / 2f, bounds.center + bounds.size / 2f, Color.green, 5f);
	}

	private bool IsInsideOf(Collider2D[] colls, Vector2 globalPoint)
	{
		for (int i = 0; i < colls.Length; i++)
		{
			if (colls[i].OverlapPoint(globalPoint))
			{
				return true;
			}
		}
		return false;
	}

	public Vector2 GetRandomGridPoint()
	{
		if (LocalColliderGridPoints == null)
		{
			Debug.LogWarning(base.gameObject.name + " hasn't baked their local collider grid points");
			return default(Vector2);
		}
		return LocalColliderGridPoints.PickRandom();
	}

	public Vector2 GetLowestGlobalGridPoint()
	{
		float num = float.MaxValue;
		Vector2 result = Vector2.zero;
		if (LocalColliderGridPoints == null)
		{
			Debug.LogWarning(base.gameObject.name + " hasn't baked their local collider grid points");
			return result;
		}
		Vector2[] localColliderGridPoints = LocalColliderGridPoints;
		for (int i = 0; i < localColliderGridPoints.Length; i++)
		{
			Vector2 vector = localColliderGridPoints[i];
			base.transform.TransformPoint(vector);
			if (vector.y < num)
			{
				result = vector;
				num = vector.y;
			}
		}
		return result;
	}

	public Vector2 GetNearestLocalHoldingPoint(Vector2 worldPoint, out float distance)
	{
		Vector2 vector = base.transform.InverseTransformPoint(worldPoint);
		Vector2 result = Vector2.zero;
		distance = float.MaxValue;
		Vector3[] holdingPositions = HoldingPositions;
		foreach (Vector2 vector2 in holdingPositions)
		{
			float sqrMagnitude = (vector - vector2).sqrMagnitude;
			if (!(distance < sqrMagnitude))
			{
				distance = sqrMagnitude;
				result = vector2;
			}
		}
		return result;
	}

	public void MakeWeightless()
	{
		rigidbody.gravityScale = 0f;
		InitialMass = rigidbody.mass;
		rigidbody.mass = Mathf.Max(Mathf.Min(rigidbody.mass, 0.02f), rigidbody.mass * 0.02f);
		IsWeightless = true;
		BroadcastMessage("OnMakeWeightless", SendMessageOptions.DontRequireReceiver);
	}

	public void MakeWeightful()
	{
		RecalculateMassBasedOnSize();
		rigidbody.mass = InitialMass;
		rigidbody.gravityScale = InitialGravityScale;
		IsWeightless = false;
		BroadcastMessage("OnMakeWeightful", SendMessageOptions.DontRequireReceiver);
	}

	public void Disintegrate()
	{
		if (base.gameObject.isStatic || !Disintegratable || !Deletable)
		{
			return;
		}
		foreach (Penetration penetration in penetrations)
		{
			DestroyStabJoint(penetration);
		}
		foreach (Penetration victimPenetration in victimPenetrations)
		{
			victimPenetration.Active = false;
			stabWoundCount--;
			beingStabbedBy.Remove(this);
			victimPenetration.Stabber.penetrations.Remove(victimPenetration);
			UndoPenetrationNoCollision(victimPenetration);
		}
		victimPenetrations.Clear();
		this.OnDisintegration?.Invoke(this, EventArgs.Empty);
		Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		Renderer[] componentsInChildren2 = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = false;
		}
		Rigidbody2D[] componentsInChildren3 = GetComponentsInChildren<Rigidbody2D>();
		for (int i = 0; i < componentsInChildren3.Length; i++)
		{
			componentsInChildren3[i].simulated = false;
		}
		if (TryGetComponent<BloodTankBehaviour>(out var component))
		{
			component.enabled = false;
		}
		base.gameObject.SetActive(value: false);
		isDisintegrated = true;
		InvokeOnItemRemoved();
	}

	public void OnBeforeSerialise()
	{
		serialisable_victims = penetrations.Select(delegate(Penetration p)
		{
			try
			{
				return p.Victim.GetComponent<SerialisableIdentity>().UniqueIdentity;
			}
			catch (Exception)
			{
				Debug.LogWarning("Stab victim with invalid or non-existent ID");
				return default(Guid);
			}
		}).ToArray();
		serialisable_durations = penetrations.Select((Penetration p) => p.Duration).ToArray();
		serialisable_axes = penetrations.Select((Penetration p) => p.Axis).ToArray();
		serialisable_anchors = penetrations.Select((Penetration p) => p.CurrentConnectedAnchor).ToArray();
		serialisable_active = penetrations.Select((Penetration p) => p.Active).ToArray();
	}

	private void InvokeOnItemRemoved()
	{
		SerialiseInstructions component = GetComponent<SerialiseInstructions>();
		ModAPI.InvokeItemRemoved(this, new UserSpawnEventArgs(base.gameObject, component ? component.OriginalSpawnableAsset : null));
	}

	public bool ShouldUpdate()
	{
		if ((bool)this && base.enabled)
		{
			return !isDisintegrated;
		}
		return false;
	}

	public void UseContinuous(ActivationPropagation activation)
	{
		ContinuousActivationBehaviour.AssertState();
		ContinuousActivationTracker[activation.Channel].Increment();
	}
}

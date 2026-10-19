using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Steamworks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Global : MonoBehaviour
{
	private class PhysicalBehaviourChunk
	{
		public PhysicalBehaviour[] Array;

		public int Count;

		public void Clear()
		{
			Count = 0;
			for (int i = 0; i < Array.Length; i++)
			{
				Array[i] = null;
			}
		}

		public void Add(PhysicalBehaviour phys)
		{
			if (Count < Array.Length)
			{
				Array[Count] = phys;
				Count++;
			}
		}
	}

	private const int maxChunkCapacity = 128;

	public AudioMixerGroup SoundEffects;

	public AudioMixerGroup Ambience;

	public AudioMixerGroup UserInterface;

	public Material SelectionOutlineMaterial;

	public Material FrozenOutlineMaterial;

	public List<PhysicalBehaviour> PhysicalObjectsInWorld = new List<PhysicalBehaviour>();

	public Dictionary<Transform, PhysicalBehaviour> PhysicalObjectsInWorldByTransform = new Dictionary<Transform, PhysicalBehaviour>();

	public CameraControlBehaviour CameraControlBehaviour;

	public Camera camera;

	public FireLoopSoundControllerBehaviour FireLoopSoundControllerBehaviour;

	public EventSystem eventSystem;

	public GameObject PauseMenu;

	public bool ShowLimbStatus;

	public static Global main;

	public VideoSettingsInitBehaviour VideoSettingsInitBehaviour;

	private readonly Dictionary<Vector2Int, PhysicalBehaviourChunk> physicalBehaviourChunks = new Dictionary<Vector2Int, PhysicalBehaviourChunk>();

	public BellClipContainer BellClips;

	public MixerControllerBehaviour MixerControllerBehaviour;

	public ResizeHandles ResizeHandles;

	public Vector3 MousePosition;

	public Vector3 CameraSpeedCompensatingMousePosition;

	public float SlowmotionTimescale = 0.5f;

	private float timeScaleTarget = 1f;

	private bool chunksUpToDate;

	public int ActiveUiBlockers;

	public WorkshopUploaderDialog WorkshopUploaderDialog;

	public ToolControllerBehaviour ToolControllerBehaviour;

	public LayerMask EnergyLayers;

	public Material GlobalFoliageMaterial;

	public Material GlobalParticleFoliageMaterial;

	public CatalogToggleBehaviour CatalogToggleBehaviour;

	[SerializeField]
	internal GameObject HolleManPrefab;

	public AudioClip[] WoodStrutStressClips;

	public AudioClip[] WoodStutSnapClips;

	public AudioClip[] BulletCrackClips;

	public AudioSource CameraAudioSource;

	public float DynamicSqrdVelocityThreshold = 2500f;

	public GameObject MetalScrapePrefab;

	public const float MetricMultiplier = 220f / 267f;

	private readonly List<AudioSource> audioSources = new List<AudioSource>();

	private bool paused;

	private bool pausedMenu;

	private bool hasBeenPausedByOverlay;

	private bool alreadyUnlockedPacifist;

	private Vector3 oldMousePos;

	public bool UiEnabled = true;

	public UnityEvent OnUiToggle = new UnityEvent();

	public CanvasScaler CanvasScaler;

	public Color BloodColor;

	public DecalDescriptor BlastMarkDecal;

	private bool isActivePlayer;

	private float inactivity;

	internal static bool allowedToRegisterExternalLiquids;

	public const float ChunkSize = 1.5f;

	public const float InverseChunkSize = 2f / 3f;

	public bool UILock
	{
		get
		{
			if (!eventSystem || !eventSystem.IsPointerOverGameObject())
			{
				if ((bool)ResizeHandles)
				{
					return ResizeHandles.IsHovering;
				}
				return false;
			}
			return true;
		}
	}

	public static bool ActiveUiBlock => main.ActiveUiBlockers > 0;

	public bool SlowMotion { get; private set; }

	public bool Paused
	{
		get
		{
			return paused;
		}
		private set
		{
			paused = value;
			foreach (PhysicalBehaviour item in PhysicalObjectsInWorld)
			{
				item.rigidbody.interpolation = CurrentInterpolationMode;
			}
		}
	}

	public static RigidbodyInterpolation2D CurrentInterpolationMode
	{
		get
		{
			if (!main.paused)
			{
				return RigidbodyInterpolation2D.Interpolate;
			}
			return RigidbodyInterpolation2D.None;
		}
	}

	public static Vector3 CameraPosition
	{
		get
		{
			return main.camera.transform.localPosition;
		}
		set
		{
			main.camera.transform.localPosition = value;
		}
	}

	public static Vector2 MouseDelta { get; private set; }

	public static float LastKillTime { get; internal set; }

	public event EventHandler<bool> LimbStatusToggled;

	public bool GetPausedMenu()
	{
		return pausedMenu;
	}

	public void SetPausedMenu(bool value, string page = "Main")
	{
		if (!VideoSettingsInitBehaviour || !MixerControllerBehaviour || !PauseMenu)
		{
			UISoundBehaviour.Main.Error();
			DialogBoxManager.Notification($"You should never have seen this dialog box. Something went horribly wrong.\n<i>{VideoSettingsInitBehaviour},{MixerControllerBehaviour},{PauseMenu}</i>");
			return;
		}
		pausedMenu = value;
		if ((bool)VideoSettingsInitBehaviour)
		{
			VideoSettingsInitBehaviour.Sync();
		}
		if ((bool)MixerControllerBehaviour)
		{
			MixerControllerBehaviour.Sync();
		}
		PauseMenu.SetActive(pausedMenu);
		if (pausedMenu)
		{
			CatalogToggleBehaviour.Canvas.enabled = true;
		}
		else
		{
			CatalogToggleBehaviour.UpdateUiVisibility();
		}
		MenuController.SetPage(page);
		if (!value && (bool)EnvironmentSettingsController.Main)
		{
			EnvironmentSettingsController.Main.RegenerateControls();
		}
	}

	public void ClosePauseMenu()
	{
		SetPausedMenu(value: false);
	}

	public void OpenPauseMenu()
	{
		SetPausedMenu(value: true);
	}

	public void ToggleLimbStatus()
	{
		ShowLimbStatus = !ShowLimbStatus;
		this.LimbStatusToggled?.Invoke(this, ShowLimbStatus);
	}

	public void AddAudioSource(AudioSource a, bool isAmbience = false)
	{
		audioSources.Add(a);
		a.pitch = Time.timeScale;
		if (!a.outputAudioMixerGroup)
		{
			a.outputAudioMixerGroup = (isAmbience ? Ambience : SoundEffects);
		}
	}

	public void RemoveAudioSource(AudioSource audioSource)
	{
		audioSources.Remove(audioSource);
	}

	private void Awake()
	{
		AliveBehaviour.AliveByTransform.Clear();
		MapLightBehaviour.StartEnabled = false;
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		InputSystem.CurrentUniverse = ActionUniverse.Game;
		LiquidMixingController.MixInstructions.Clear();
		Liquid.ClearLiquidRegistry();
		allowedToRegisterExternalLiquids = true;
		Liquid.Register("BLOOD", new Blood());
		Liquid.Register("GORSE BLOOD", new GorseBlood());
		Liquid.Register("OIL", new Oil());
		Liquid.Register("NITRO", new Nitroglycerine());
		Liquid.Register("EXOTIC LIQUID", new Chemistry.ExoticLiquid());
		Liquid.Register("TRITIUM", new Chemistry.Tritium());
		Liquid.Register("INERT LIQUID", new Chemistry.InertLiquid());
		Liquid.Register("BEVERAGE M04", new Chemistry.BeverageM04());
		Liquid.Register("COOLANT", new Chemistry.Coolant());
		Liquid.Register("REANIMATION AGENT", new ZombieSyringe.ZombiePoisonLiquid());
		Liquid.Register("ADRENALINE", new AdrenalineSyringe.AdrenalineLiquid());
		Liquid.Register("ACID", new AcidSyringe.AcidLiquid());
		Liquid.Register("BONE EATING POISON", new BoneEatingPoisonSyringe.BoneHurtingJuiceLiquid());
		Liquid.Register("COAGULATION SERUM", new CoagulationSyringe.CoagulationLiquid());
		Liquid.Register("INSTANT DEATH POISON", new DeathSyringe.InstantDeathPoisonLiquid());
		Liquid.Register("FREEZE POISON", new FreezeSyringe.FreezePoisonLiquid());
		Liquid.Register("KNOCKOUT POISON", new KnockoutSyringe.KnockoutPoisonLiquid());
		Liquid.Register("LIFE SERUM", new LifeSyringe.LifeSerumLiquid());
		Liquid.Register("ULTRA STRENGTH SERUM", new UltraStrengthSyringe.UltraStrengthSerumLiquid());
		Liquid.Register("MENDING SERUM", new MendingSyringe.MendingSerum());
		Liquid.Register("WATER BREATHING SERUM", new WaterBreathingSyringe.WaterBreathingSerum());
		Liquid.Register("OSTEOMORPHOSIS AGENT", new OsteomorphosisAgent());
		Liquid.Register("IMMORTALITY SERUM", new ImmortalitySerum());
		Liquid.Register("PAIN KILLER", new PainKillerSyringe.PainKillerLiquid());
		Liquid.Register("INERT PINK LIQUID", new PinkSyringe.PinkDormantLiquid());
		Liquid.Register("VESTIBULAR POISON", new PinkSyringe.VestibularPoison());
		Liquid.Register("MUSCLE POISON", new PinkSyringe.MusclePoison());
		Liquid.Register("NUMBING POISON", new PinkSyringe.NumbingPoison());
		Liquid.Register("MIRRORISING AGENT", new PinkSyringe.ReflectionPoison());
		Liquid.Register("TRANSPARENCY AGENT", new PinkSyringe.TransparencyPoison());
		Liquid.Register("MASS AGENT", new PinkSyringe.MassManipulationPoison());
		Liquid.Register("EXPLOSION POISON", new PinkSyringe.ExplosionPoison());
		Liquid.Register("CRUSHING POISON", new PinkSyringe.CrushingPoison());
		Liquid.Register("DURABILITY SERUM", new PinkSyringe.DurabilitySerum());
		Liquid.Register("REGENERATION SERUM", new PinkSyringe.RegenerationSerum());
		Liquid.Register("DISTORTION POISON", new PinkSyringe.SizeManipulationPoison());
		Liquid.Register("CIRCULATION POISON", new PinkSyringe.CirculationPoison());
		Liquid.Register("COMBUSTION AGENT", new PinkSyringe.CombustionAgent());
		Liquid.Register("ENHANCING SERUM", new PinkSyringe.MuscleEnhancementSerum());
		Liquid.Register("TISSUE DECONSTRUCTION AGENT", new PinkSyringe.TissueDeconstructionAgent());
		Liquid.Register("DEBUG LIQUID 001", new Chemistry.DebugDiscolorationLiquid());
		LiquidMixingController.MixInstructions.Add(new LiquidMixInstructions(Liquid.GetLiquid("TRITIUM"), Liquid.GetLiquid("MENDING SERUM"), Liquid.GetLiquid("IMMORTALITY SERUM"), 0.01f)
		{
			ContainerFilter = (BloodContainer c) => !(c is CirculationBehaviour)
		});
		LiquidMixingController.MixInstructions.Add(new LiquidMixInstructions(Liquid.GetLiquid("ACID"), Liquid.GetLiquid("MENDING SERUM"), Liquid.GetLiquid("OSTEOMORPHOSIS AGENT"))
		{
			ContainerFilter = (BloodContainer c) => !(c is CirculationBehaviour)
		});
		LiquidMixingController.MixInstructions.Add(new LiquidMixInstructions(Liquid.GetLiquid("WATER BREATHING SERUM"), Liquid.GetLiquid("TRITIUM"), Liquid.GetLiquid("EXOTIC LIQUID")));
		LiquidMixingController.MixInstructions.Add(new LiquidMixInstructions(Liquid.GetLiquid("ADRENALINE"), Liquid.GetLiquid("MENDING SERUM"), Liquid.GetLiquid("INERT LIQUID"), Liquid.GetLiquid("BEVERAGE M04")));
		ModAPI.Draw = new ModDebugDrawer(new Stack<Action>());
		ModResourceCache.Clean();
		InputSystem.Load();
		main = this;
		ContraptionSerialiser.Initialise();
		camera = Camera.main;
		CameraControlBehaviour = UnityEngine.Object.FindObjectOfType<CameraControlBehaviour>();
		IgnoreCollisionStackController.Clear();
	}

	private void Start()
	{
		ActiveUiBlockers = 0;
		SetPausedMenu(value: false);
		eventSystem = EventSystem.current;
		CanvasScaler = UnityEngine.Object.FindObjectOfType<CanvasScaler>();
		FireLoopSoundControllerBehaviour = GetComponent<FireLoopSoundControllerBehaviour>();
		timeScaleTarget = 1f;
		UpdatePitch(timeScaleTarget);
		StartCoroutine(Utils.DelayCoroutine(0.05f, delegate
		{
			UpdatePitch(timeScaleTarget);
		}));
		if (SteamworksInitialiser.IsInitialised)
		{
			SteamFriends.OnGameOverlayActivated += OnGameOverlayActivated;
			NonSteamStatManager.Stats.SetStat("BODY_COUNT", Mathf.Max(StatManager.GetInt(StatManager.Stat.BODY_COUNT), NonSteamStatManager.Stats.GetStat("BODY_COUNT")));
			NonSteamStatManager.Stats.SetStat("TOTAL_SPAWNED_ITEMS", Mathf.Max(StatManager.GetInt(StatManager.Stat.TOTAL_SPAWNED_ITEMS), NonSteamStatManager.Stats.GetStat("TOTAL_SPAWNED_ITEMS")));
		}
	}

	private void OnGameOverlayActivated(bool active)
	{
		if (active)
		{
			if (!Paused)
			{
				Paused = true;
				hasBeenPausedByOverlay = true;
			}
		}
		else if (hasBeenPausedByOverlay)
		{
			Paused = false;
			hasBeenPausedByOverlay = false;
		}
	}

	private void Update()
	{
		if (SteamworksInitialiser.IsInitialised && !alreadyUnlockedPacifist)
		{
			isActivePlayer = inactivity < 300f;
			if (!isActivePlayer)
			{
				LastKillTime = Time.timeSinceLevelLoad;
			}
			else if (Time.timeSinceLevelLoad - LastKillTime > 3600f)
			{
				StatManager.IncrementInteger(StatManager.Stat.PACIFIST);
				alreadyUnlockedPacifist = true;
			}
		}
		if (!TriggerEditorBehaviour.IsBeingEdited && !ActiveUiBlock)
		{
			if (InputSystem.Down("pause") && DialogBox.OpenDialogboxCount == 0 && (InputSystem.Actions["pause"].Key != KeyCode.Escape || (!ResizeHandles.gameObject.activeInHierarchy && !LayerEditorController.Instance.IsOpen)) && (!GetPausedMenu() || MenuController.main.CurrentPage.name == "Main"))
			{
				SetPausedMenu(!GetPausedMenu());
			}
			if (Input.GetKeyDown(KeyCode.F1))
			{
				SetPausedMenu(value: true, "Controls");
			}
			ProcessSnappingShortcuts();
		}
		else if (InputSystem.Down("pause") && EventSystem.current != null)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		oldMousePos = MousePosition;
		UpdateMousePosition();
		float num = ((Paused || GetPausedMenu()) ? 0f : timeScaleTarget);
		float timeScale = Time.timeScale;
		Time.timeScale = Utils.SmoothApproach(Time.timeScale, num, 15f, unscaledDeltaTime);
		if (Mathf.Abs(num - timeScale) > 0.0001f)
		{
			UpdatePitch(timeScale);
		}
		if (GetPausedMenu() || DialogBox.IsAnyDialogboxOpen || ActiveUiBlock)
		{
			return;
		}
		if (InputSystem.Down("toggleLimbStatus"))
		{
			ToggleLimbStatus();
		}
		if (InputSystem.Down("slowmo"))
		{
			ToggleSlowmotion();
		}
		if (InputSystem.Down("time"))
		{
			TogglePaused();
		}
		if (InputSystem.Down("freezeSelection"))
		{
			ContextMenuBehaviour instance = ContextMenuBehaviour.Instance;
			if (instance.HasObject)
			{
				instance.Show(Input.mousePosition);
				instance.FreezeAction();
				instance.Hide();
				UISoundBehaviour.Main.Blip();
			}
		}
		if (InputSystem.Down("igniteSelection"))
		{
			ContextMenuBehaviour instance2 = ContextMenuBehaviour.Instance;
			if (instance2.HasObject)
			{
				instance2.Show(Input.mousePosition);
				instance2.IgniteAction();
				instance2.Hide();
				UISoundBehaviour.Main.Blip();
			}
		}
		if (InputSystem.Down("resizeSelection"))
		{
			ContextMenuBehaviour instance3 = ContextMenuBehaviour.Instance;
			if (instance3.ShouldShowResizeButton)
			{
				instance3.Show(Input.mousePosition);
				instance3.ResizeAction();
				instance3.Hide();
				UISoundBehaviour.Main.Blip();
			}
		}
		if (InputSystem.Down("eyedrop") && (bool)SelectionController.Main.CurrentlyUnderMouse)
		{
			SerialiseInstructions componentInParent = SelectionController.Main.CurrentlyUnderMouse.GetComponentInParent<SerialiseInstructions>();
			if ((bool)componentInParent)
			{
				SpawnableAsset originalSpawnableAsset = componentInParent.OriginalSpawnableAsset;
				if ((bool)originalSpawnableAsset)
				{
					CatalogBehaviour catalogBehaviour = CatalogBehaviour.Main;
					catalogBehaviour.SetItem(originalSpawnableAsset);
					UISoundBehaviour.Main.Blip();
					if ((bool)originalSpawnableAsset.Category && catalogBehaviour.Catalog.Categories.Contains(originalSpawnableAsset.Category))
					{
						catalogBehaviour.SetCategory(originalSpawnableAsset.Category);
					}
				}
			}
		}
		if (InputSystem.Down("toggleHoveringHighlights"))
		{
			UserPreferenceManager.Current.ShowOutlines = !UserPreferenceManager.Current.ShowOutlines;
			NotificationControllerBehaviour.Show("Hovering highlights have been " + (UserPreferenceManager.Current.ShowOutlines ? "enabled" : "disabled"));
			UserPreferenceManager.Save();
		}
	}

	private void ProcessSnappingShortcuts()
	{
		if (InputSystem.Down("incrementGrid"))
		{
			changeGridSnap(0.1f);
		}
		if (InputSystem.Down("decrementGrid"))
		{
			changeGridSnap(-0.1f);
		}
		if (InputSystem.Down("incrementAngleSnap"))
		{
			changeAngleSnap(5);
		}
		if (InputSystem.Down("decrementAngleSnap"))
		{
			changeAngleSnap(-5);
		}
		void changeAngleSnap(int delta)
		{
			UserPreferenceManager.Current.AngleSnap = Mathf.Clamp(UserPreferenceManager.Current.AngleSnap + delta, 5, 90);
			UserPreferenceManager.Save();
			NotificationControllerBehaviour.Show($"Angle snap set to {UserPreferenceManager.Current.AngleSnap} degrees");
		}
		void changeGridSnap(float delta)
		{
			UserPreferenceManager.Current.GridSnap = Mathf.Clamp(UserPreferenceManager.Current.GridSnap + delta, 0.1f, 2f);
			UserPreferenceManager.Save();
			NotificationControllerBehaviour.Show($"Grid size set to {Math.Round(UserPreferenceManager.Current.GridSnap, 2)} m");
		}
	}

	public void AddUiBlocker()
	{
		ActiveUiBlockers++;
	}

	public void RemoveUiBlocker()
	{
		ActiveUiBlockers--;
	}

	private void UpdatePitch(float timeScale)
	{
		for (int i = 0; i < audioSources.Count; i++)
		{
			AudioSource audioSource = audioSources[i];
			if ((bool)audioSource)
			{
				audioSource.pitch = timeScale;
			}
		}
	}

	private void LateUpdate()
	{
		if (Time.frameCount % 2 == 0)
		{
			chunksUpToDate = false;
		}
		MouseDelta = MousePosition - oldMousePos;
		if (MouseDelta.sqrMagnitude < 0.01f)
		{
			inactivity += Time.deltaTime;
		}
		else
		{
			inactivity = 0f;
		}
		if ((bool)CameraControlBehaviour)
		{
			CameraSpeedCompensatingMousePosition = camera.ScreenToWorldPoint(Input.mousePosition) + CameraControlBehaviour.MovementDelta;
			CameraSpeedCompensatingMousePosition.z = 0f;
		}
	}

	public void TogglePaused()
	{
		Paused = !Paused;
		NotificationControllerBehaviour.Show(Paused ? "Paused" : "Unpaused");
		Utils.DelayCoroutine(0.05f, delegate
		{
			Physics2D.Simulate(Time.fixedDeltaTime);
			Physics2D.SyncTransforms();
		});
	}

	private void UpdateMousePosition()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = 0f - camera.transform.position.z;
		MousePosition = camera.ScreenToWorldPoint(mousePosition);
	}

	public void ToggleSlowmotion()
	{
		SlowMotion = !SlowMotion;
		NotificationControllerBehaviour.Show(SlowMotion ? "Slow motion enabled" : "Slow motion disabled");
		timeScaleTarget = (SlowMotion ? (UserPreferenceManager.Current.SlowMotionSpeed / 100f) : 1f);
	}

	private void UpdatePhysicalBehaviourChunks()
	{
		foreach (KeyValuePair<Vector2Int, PhysicalBehaviourChunk> physicalBehaviourChunk2 in physicalBehaviourChunks)
		{
			physicalBehaviourChunk2.Value.Clear();
		}
		for (int i = 0; i < PhysicalObjectsInWorld.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = PhysicalObjectsInWorld[i];
			if ((bool)physicalBehaviour && !physicalBehaviour.isDisintegrated)
			{
				Vector2Int chunkOf = GetChunkOf(physicalBehaviour);
				if (physicalBehaviourChunks.TryGetValue(chunkOf, out var value))
				{
					value.Add(physicalBehaviour);
					continue;
				}
				PhysicalBehaviourChunk physicalBehaviourChunk = new PhysicalBehaviourChunk
				{
					Array = new PhysicalBehaviour[128],
					Count = 0
				};
				physicalBehaviourChunk.Add(physicalBehaviour);
				physicalBehaviourChunks.Add(chunkOf, physicalBehaviourChunk);
			}
		}
		chunksUpToDate = true;
	}

	private Vector2Int GetChunkOf(PhysicalBehaviour t)
	{
		return GetChunkNear(t.transform.position, t.ObjectArea / 2f);
	}

	private Vector2Int GetChunkNear(Vector3 position, float radius)
	{
		float num = radius / 2f;
		Vector3 vector = (position + (Vector3)UnityEngine.Random.insideUnitCircle * num) * (2f / 3f);
		return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
	}

	public IEnumerable<PhysicalBehaviour> GetPhysicsObjectsNear(PhysicalBehaviour me)
	{
		if (!chunksUpToDate)
		{
			UpdatePhysicalBehaviourChunks();
		}
		Vector2Int chunkOf = GetChunkOf(me);
		if (!physicalBehaviourChunks.TryGetValue(chunkOf, out var c))
		{
			yield break;
		}
		for (int i = 0; i < c.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = c.Array[i];
			if ((bool)physicalBehaviour && physicalBehaviour != me && !physicalBehaviour.isDisintegrated)
			{
				yield return physicalBehaviour;
			}
		}
	}

	public IEnumerable<PhysicalBehaviour> GetPhysicsObjectsNearPosition(Vector2 position, float radius)
	{
		if (!chunksUpToDate)
		{
			UpdatePhysicalBehaviourChunks();
		}
		Vector2Int chunkNear = GetChunkNear(position, radius);
		if (!physicalBehaviourChunks.TryGetValue(chunkNear, out var c))
		{
			yield break;
		}
		for (int i = 0; i < c.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = c.Array[i];
			if (!physicalBehaviour.isDisintegrated)
			{
				yield return physicalBehaviour;
			}
		}
	}

	public IEnumerable<PhysicalBehaviour> GetPhysicsObjectsNearPositionAccurate(Vector2 position, float radius, float accuracy = 1f)
	{
		if (!chunksUpToDate)
		{
			UpdatePhysicalBehaviourChunks();
		}
		int res = Mathf.CeilToInt(radius);
		float halfRes = (float)res * 0.5f;
		for (int x = 0; x < res; x++)
		{
			for (int y = 0; y < res; y++)
			{
				if (accuracy < 0.999f && UnityEngine.Random.value > accuracy)
				{
					continue;
				}
				Vector2 vector = new Vector2(position.x - halfRes + (float)x, position.y - halfRes + (float)y);
				Vector2Int chunkNear = GetChunkNear(vector, 0f);
				if (!physicalBehaviourChunks.TryGetValue(chunkNear, out var c))
				{
					continue;
				}
				for (int i = 0; i < c.Count; i++)
				{
					PhysicalBehaviour physicalBehaviour = c.Array[i];
					if (!physicalBehaviour.isDisintegrated)
					{
						yield return physicalBehaviour;
					}
				}
				c = null;
			}
		}
	}

	private void OnDestroy()
	{
		NonSteamStatManager.SaveToFile("stats");
		ItemPersistence.Serialise();
		allowedToRegisterExternalLiquids = false;
	}
}

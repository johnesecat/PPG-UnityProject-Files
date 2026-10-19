using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class PersonBehaviour : AliveBehaviour, Messages.IShot
{
	[SkipSerialisation]
	[HideInInspector]
	public LimbBehaviour[] Limbs;

	[ProgressBar("Consciousness", 1f, EColor.Blue)]
	public float Consciousness = 1f;

	[ProgressBar("Shock", 1f, EColor.Orange)]
	public float ShockLevel;

	[ProgressBar("Pain", 1f, EColor.Red)]
	public float PainLevel;

	[ProgressBar("Adrenaline", 1f, EColor.Violet)]
	public float AdrenalineLevel;

	[ProgressBar("Oxygen", 1f, EColor.White)]
	public float OxygenLevel = 1f;

	[Space]
	[SkipSerialisation]
	public GameObject BloodExplosionPrefab;

	[SkipSerialisation]
	public GameObject BleedingParticlePrefab;

	[Space]
	[SkipSerialisation]
	public List<RagdollPose> Poses = new List<RagdollPose>();

	[HideInInspector]
	public RagdollPose ActivePose;

	[HideInInspector]
	[SkipSerialisation]
	public float AngleOffset;

	public float GlobalRigidityMultiplier = 1f;

	public int OverridePoseIndex = -1;

	[HideInInspector]
	public Dictionary<PoseState, RagdollPose> LinkedPoses = new Dictionary<PoseState, RagdollPose>();

	[SkipSerialisation]
	public float Heartbeat;

	[SkipSerialisation]
	[FormerlySerializedAs("DefaultMateral")]
	public Material DefaultMaterial;

	[SkipSerialisation]
	public Material LegacyMaterial;

	[SkipSerialisation]
	public Material GorelessMaterial;

	[SkipSerialisation]
	public GameObject PoolableImpactEffect;

	[SkipSerialisation]
	public float ImpactEffectShotDamageThreshold = 5f;

	[SkipSerialisation]
	public AudioClip[] DismembermentClips;

	[SkipSerialisation]
	public AudioClip[] BoneBreakClips;

	private Material chosenMaterial;

	private LimbBehaviour Head;

	public bool RandomisedSize = true;

	public bool CountDeathAsStat = true;

	private bool dead;

	private float totalAirtime;

	[HideInInspector]
	public bool HasAlreadyBeenDeadOnce;

	private float seed;

	public bool Braindead;

	public bool BrainDamaged;

	[HideInInspector]
	public float BrainDamagedTime = -1f;

	[HideInInspector]
	public float SeizureTime;

	public float DesiredWalkingDirection;

	[SkipSerialisation]
	public float BalanceOffset { get; private set; }

	[SkipSerialisation]
	public float AverageHealth { get; private set; }

	[SkipSerialisation]
	public float AverageSpeed { get; private set; }

	[Obsolete]
	public float AverageAngle { get; private set; }

	[SkipSerialisation]
	public float AverageFireIntensity { get; private set; }

	[SkipSerialisation]
	public float AverageWetness { get; private set; }

	[SkipSerialisation]
	public bool IsTouchingFloor { get; private set; }

	public Material ChosenMaterial => chosenMaterial;

	[ContextMenu("Create new pose")]
	[Button("Create new pose", EButtonEnableMode.Always)]
	public void CreatePose()
	{
		Poses.Add(GenerateBasePose());
	}

	private RagdollPose GenerateBasePose()
	{
		return new RagdollPose
		{
			Name = "Pose " + (Poses.Count + 1),
			Rigidity = 5f,
			Angles = new List<RagdollPose.LimbPose>(from l in GetComponentsInChildren<LimbBehaviour>()
				where l.GetComponent<HingeJoint2D>()
				select new RagdollPose.LimbPose(l, l.GetComponent<HingeJoint2D>().jointAngle))
		};
	}

	[ContextMenu("Initialise all poses")]
	[Button("Initialise all poses", EButtonEnableMode.Always)]
	public void InitialiseAllPoses()
	{
		Poses.Clear();
		foreach (PoseState value in Enum.GetValues(typeof(PoseState)))
		{
			RagdollPose ragdollPose = GenerateBasePose();
			ragdollPose.Name = value.ToString();
			ragdollPose.State = value;
			Poses.Add(ragdollPose);
		}
	}

	private void Awake()
	{
		seed = UnityEngine.Random.value * 1000f;
		AddToDictionary();
		Limbs = GetComponentsInChildren<LimbBehaviour>();
		CreateLinkedPoseDictionary();
		Consciousness = 1f;
		OxygenLevel = 1f;
		chosenMaterial = null;
		if (UserPreferenceManager.Current.GorelessMode)
		{
			chosenMaterial = GorelessMaterial;
		}
		else
		{
			switch (UserPreferenceManager.Current.GoreMode)
			{
			case GoreShaderMode.Default:
				chosenMaterial = DefaultMaterial;
				break;
			case GoreShaderMode.Legacy:
				chosenMaterial = LegacyMaterial;
				break;
			}
		}
		LimbBehaviour[] limbs = Limbs;
		foreach (LimbBehaviour limbBehaviour in limbs)
		{
			limbBehaviour.Person = this;
			if (limbBehaviour.HasBrain)
			{
				Head = limbBehaviour;
			}
		}
		if (RandomisedSize)
		{
			base.transform.localScale = Vector2.one * UnityEngine.Random.Range(1.59f, 2f) / 2f / (220f / 267f);
		}
	}

	private void CreateLinkedPoseDictionary()
	{
		foreach (PoseState poseState in Enum.GetValues(typeof(PoseState)))
		{
			RagdollPose[] array = Poses.Where((RagdollPose l) => l.State == poseState).ToArray();
			if (!array.Any())
			{
				throw new Exception("Missing pose for " + poseState);
			}
			LinkedPoses.Add(poseState, array.PickRandom());
		}
	}

	private void Start()
	{
		IsTouchingFloor = false;
		AngleOffset = ((!(base.transform.localScale.x < 0f)) ? 1 : (-1));
		foreach (RagdollPose pose in Poses)
		{
			pose.ConstructDictionary();
		}
		SetCollisionIgnores();
		SetPose(Poses[0]);
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (BrainDamaged)
		{
			if (!UserPreferenceManager.Current.BrainDamage)
			{
				BrainDamaged = false;
			}
			BrainDamagedTime += deltaTime;
		}
		else
		{
			BrainDamagedTime = 0f;
		}
		AllLimbLoopChecks();
		AttachedLimbLoopChecks();
		if (Head.Health < 0.01f || !Head.NodeBehaviour.IsConnectedToRoot)
		{
			LimbBehaviour[] limbs = Limbs;
			for (int i = 0; i < limbs.Length; i++)
			{
				limbs[i].CirculationBehaviour.IsPump = false;
			}
		}
		BalanceOffset = Vector2.Dot((Head.CirculationBehaviour.InternalBleedingIntensity > 0.5f) ? (Head.transform.up + Utils.GetPerlin2Mapped(Time.time, seed) * 0.7f).normalized : Head.transform.up, Vector2.right) * base.transform.localScale.x;
		if (IsAlive())
		{
			Consciousness += deltaTime / 120f;
			if (Head.PhysicalBehaviour.Wetness < 0.5f)
			{
				OxygenLevel += Mathf.Pow(Mathf.Sin(Time.time * 3f), 16f) * 0.5f / 40f * (deltaTime * 60f);
			}
		}
		else
		{
			Consciousness -= deltaTime / 60f;
			OxygenLevel -= deltaTime / 40f;
		}
		OxygenLevel = Mathf.Clamp01(OxygenLevel);
		Consciousness = Mathf.Clamp01(Consciousness);
		ShockLevel -= deltaTime / 25f;
		ShockLevel = Mathf.Clamp01(ShockLevel);
		PainLevel -= deltaTime / 5f;
		PainLevel = Mathf.Clamp01(PainLevel);
		AdrenalineLevel -= deltaTime;
		AdrenalineLevel = Mathf.Max(0f, Mathf.Min(20f, AdrenalineLevel));
		totalAirtime += Time.deltaTime;
		if (IsAlive() && totalAirtime > StatManager.GetFloat(StatManager.Stat.RAGDOLL_AIR_TIME))
		{
			StatManager.SetFloat(StatManager.Stat.RAGDOLL_AIR_TIME, totalAirtime);
		}
		HandleDeathCounter();
		if (DesiredWalkingDirection > 0f)
		{
			DesiredWalkingDirection -= deltaTime;
		}
		else
		{
			DesiredWalkingDirection += deltaTime;
		}
		DesiredWalkingDirection = Mathf.Clamp(DesiredWalkingDirection, -4f, 4f);
	}

	private void FixedUpdate()
	{
		if (BrainDamaged && IsAlive() && UnityEngine.Random.value > 0.99f)
		{
			Wince(120f);
			if (SeizureTime <= float.Epsilon && UnityEngine.Random.value > 0.95f)
			{
				SeizureTime = UnityEngine.Random.Range(2, 30);
			}
		}
		if (SeizureTime > 0f)
		{
			SeizureTime -= Time.deltaTime;
			Wince(16f * UnityEngine.Random.value);
		}
	}

	private void HandleDeathCounter()
	{
		if (!CountDeathAsStat)
		{
			return;
		}
		if (!IsAlive() && !dead)
		{
			dead = true;
			ModAPI.InvokeDeath(this, this);
			StatManager.IncrementInteger(StatManager.Stat.BODY_COUNT);
			NonSteamStatManager.Stats.Increment("BODY_COUNT");
			Global.LastKillTime = Time.timeSinceLevelLoad;
			if ((bool)Head && Head.IsZombie && !HasAlreadyBeenDeadOnce && UnityEngine.Random.value < 0.02f)
			{
				UnityEngine.Object.Instantiate(Global.main.HolleManPrefab, Head.transform.position, Quaternion.identity);
			}
			HasAlreadyBeenDeadOnce = true;
		}
		else if (IsAlive() && dead)
		{
			dead = false;
			NonSteamStatManager.Stats.Increment("ENTITIES_REVIVED");
		}
	}

	private void LateUpdate()
	{
		DetermineActivePose();
	}

	private void DetermineActivePose()
	{
		if (dead)
		{
			SetPose(LinkedPoses[PoseState.Rest]);
			return;
		}
		bool flag = SeizureTime > float.Epsilon || (BrainDamaged && BrainDamagedTime < Utils.MapRange(0f, 1000f, 0.1f, 30f, seed));
		if (OverridePoseIndex != -1 && !flag)
		{
			DesiredWalkingDirection = 1f;
			SetPose(Poses[OverridePoseIndex % Poses.Count]);
		}
		else if (flag)
		{
			Consciousness = Mathf.Max(Consciousness, 0.8f);
			SetPose(LinkedPoses[PoseState.BrainDamage]);
		}
		else if (Mathf.Abs(DesiredWalkingDirection) > 0.5f && IsTouchingFloor && PainLevel < 0.5f)
		{
			SetPose(LinkedPoses[PoseState.Walking]);
		}
		else if (PainLevel > 0.8f && !Head.IsZombie)
		{
			SetPose(LinkedPoses[PoseState.WrithingInPain]);
		}
		else if (IsTouchingFloor)
		{
			if (AverageSpeed > 25f || ShockLevel > 0.5f)
			{
				SetPose(LinkedPoses[PoseState.Protective]);
			}
			else if (AverageSpeed < 15f || ShockLevel < 0.2f)
			{
				SetPose(LinkedPoses[PoseState.Rest]);
			}
		}
		else if (AverageSpeed < 10f && ShockLevel <= 0.5f)
		{
			SetPose(LinkedPoses[(!(AverageWetness < 0.2f)) ? PoseState.Swimming : PoseState.Rest]);
		}
		else if (AverageSpeed < 80f || ShockLevel > 0.5f)
		{
			SetPose(LinkedPoses[PoseState.Protective]);
		}
		else if (AverageWetness < 0.2f)
		{
			SetPose(LinkedPoses[PoseState.Flailing]);
		}
	}

	private void AllLimbLoopChecks()
	{
		float num = 0f;
		int val = 0;
		LimbBehaviour[] limbs = Limbs;
		foreach (LimbBehaviour limbBehaviour in limbs)
		{
			num += limbBehaviour.Health / limbBehaviour.InitialHealth;
		}
		val = Math.Max(1, val);
		num = (AverageHealth = num / (float)val);
	}

	private void AttachedLimbLoopChecks()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = 0;
		IsTouchingFloor = false;
		Heartbeat = 0f;
		LimbBehaviour[] limbs = Limbs;
		foreach (LimbBehaviour limbBehaviour in limbs)
		{
			if (limbBehaviour.NodeBehaviour.IsConnectedToRoot)
			{
				Heartbeat = Mathf.Max(Heartbeat, limbBehaviour.CirculationBehaviour.GetHeartRate());
				num += limbBehaviour.PhysicalBehaviour.BurnIntensity;
				num2 += limbBehaviour.PhysicalBehaviour.rigidbody.velocity.sqrMagnitude;
				num3 += limbBehaviour.PhysicalBehaviour.Wetness;
				if (limbBehaviour.IsOnFloor)
				{
					IsTouchingFloor = true;
				}
				num4++;
				if ((bool)Head && Head.CirculationBehaviour.InternalBleedingIntensity > float.Epsilon)
				{
					limbBehaviour.Numbness += Head.CirculationBehaviour.InternalBleedingIntensity * Time.deltaTime;
				}
			}
		}
		if (IsTouchingFloor)
		{
			totalAirtime = 0f;
		}
		num4 = Math.Max(1, num4);
		num /= (float)num4;
		num2 /= (float)num4;
		num3 /= (float)num4;
		AverageFireIntensity = num;
		AverageSpeed = num2;
		AverageWetness = num3;
	}

	private void SetCollisionIgnores()
	{
		LimbBehaviour[] limbs = Limbs;
		foreach (LimbBehaviour limbBehaviour in limbs)
		{
			LimbBehaviour[] limbs2 = Limbs;
			foreach (LimbBehaviour limbBehaviour2 in limbs2)
			{
				if (limbBehaviour != limbBehaviour2)
				{
					IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(limbBehaviour.Collider, limbBehaviour2.Collider);
				}
			}
		}
	}

	public void SetPose(RagdollPose pose)
	{
		if (pose != ActivePose)
		{
			ActivePose = pose;
			LimbBehaviour[] limbs = Limbs;
			foreach (LimbBehaviour limbBehaviour in limbs)
			{
				limbBehaviour.IsActiveInCurrentPose = ActivePose.AngleDictionary.ContainsKey(limbBehaviour);
			}
		}
	}

	public void AddPain(float intensity)
	{
		PainLevel += intensity;
	}

	public void Shot(Shot shot)
	{
		LimbBehaviour[] limbs = Limbs;
		for (int i = 0; i < limbs.Length; i++)
		{
			limbs[i].Wince(165f);
		}
	}

	public void Wince(float i = 150f)
	{
		LimbBehaviour[] limbs = Limbs;
		for (int j = 0; j < limbs.Length; j++)
		{
			limbs[j].Wince(i);
		}
	}

	public override bool IsAlive()
	{
		if ((bool)Head)
		{
			return Head.IsConsideredAlive;
		}
		return false;
	}

	private void OnDestroy()
	{
		RemoveFromDictionary();
	}

	public void SetBodyTextures(Texture2D skin, Texture2D flesh = null, Texture2D bone = null, float scale = 1f)
	{
		LimbBehaviour[] limbs = Limbs;
		foreach (LimbBehaviour obj in limbs)
		{
			SpriteRenderer renderer = obj.SkinMaterialHandler.renderer;
			Sprite sprite = renderer.sprite;
			LimbSpriteCache.LimbSprites sprites = LimbSpriteCache.Instance.LoadFor(sprite, skin, flesh, bone, scale);
			renderer.sprite = sprites.Skin;
			if ((bool)flesh)
			{
				renderer.material.SetTexture(ShaderProperties.Get("_FleshTex"), flesh);
			}
			if ((bool)bone)
			{
				renderer.material.SetTexture(ShaderProperties.Get("_BoneTex"), bone);
			}
			if (obj.TryGetComponent<ShatteredObjectSpriteInitialiser>(out var component))
			{
				component.UpdateSprites(in sprites);
			}
		}
	}

	public void SetBruiseColor(byte red, byte green, byte blue)
	{
		SetShaderColour("_BruiseColor", red, green, blue);
	}

	public void SetSecondBruiseColor(byte red, byte green, byte blue)
	{
		SetShaderColour("_SecondBruiseColor", red, green, blue);
	}

	public void SetThirdBruiseColor(byte red, byte green, byte blue)
	{
		SetShaderColour("_ThirdBruiseColor", red, green, blue);
	}

	public void SetBloodColour(byte red, byte green, byte blue)
	{
		SetShaderColour("_BloodColor", red, green, blue);
	}

	public void SetRottenColour(byte red, byte green, byte blue)
	{
		SetShaderColour("_Zombie", red, green, blue);
	}

	private void SetShaderColour(string id, byte red, byte green, byte blue)
	{
		int nameID = ShaderProperties.Get(id);
		Color value = new Color((float)(int)red / 255f, (float)(int)green / 255f, (float)(int)blue / 255f);
		LimbBehaviour[] limbs = Limbs;
		for (int i = 0; i < limbs.Length; i++)
		{
			limbs[i].SkinMaterialHandler.renderer.material.SetColor(nameID, value);
		}
	}

	internal void AddPain(object painIntensity)
	{
		throw new NotImplementedException();
	}
}

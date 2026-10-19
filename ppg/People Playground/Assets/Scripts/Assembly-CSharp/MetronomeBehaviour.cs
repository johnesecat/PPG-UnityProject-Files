using UnityEngine;

public class MetronomeBehaviour : MonoBehaviour, Messages.IUse
{
	[HideInInspector]
	public float TempoModifier = 1f;

	[SkipSerialisation]
	public AnimationCurve HandCurve;

	[SkipSerialisation]
	public Transform Hand;

	[SkipSerialisation]
	public float TickAngleBound = 11f;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	public AudioClip Tick;

	private MuteBehaviour muteBehaviour;

	private const float MinTempo = 0.125f;

	private const float MaxTempo = 32f;

	private float animationTimer;

	private float seed;

	private readonly FixedIntervalDistributor distributor = new FixedIntervalDistributor();

	public bool Activated;

	[SkipSerialisation]
	private float CurrentTempo
	{
		get
		{
			float num = ((PhysicalBehaviour.Charge > 0.01f) ? Mathf.Lerp(1f, 25f * Mathf.PerlinNoise(seed, Time.time * 4f), PhysicalBehaviour.Charge * 0.05f) : 1f);
			return Mathf.Clamp(TempoModifier * num, 0.125f, 32f);
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled && !activation.Contains(base.transform.root))
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}

	private void UpdateActivation()
	{
		if (Activated)
		{
			distributor.ResetAccumulator(0.5f / CurrentTempo);
			animationTimer = 0f;
		}
	}

	private void Awake()
	{
		muteBehaviour = GetComponent<MuteBehaviour>();
	}

	private void Start()
	{
		seed = Random.value * 1000f;
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("setMetronomeFrequency", "Set metronome rate", "Set metronome rate in Hz", delegate
		{
			Utils.OpenFloatInputDialog(TempoModifier, this, delegate(MetronomeBehaviour t, float v)
			{
				t.TempoModifier = Mathf.Clamp(v, 0.125f, 32f);
			}, "Set metronome rate", $"Target rate in Hertz from {0.125f} Hz to {32f} Hz");
		}));
		UpdateActivation();
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		animationTimer += deltaTime * CurrentTempo;
		distributor.RateHz = CurrentTempo;
		if (Activated)
		{
			for (int i = 0; i < distributor.CalculateCycleCount(deltaTime); i++)
			{
				ushort[] allChannels = ActivationPropagation.AllChannels;
				foreach (ushort channel in allChannels)
				{
					SendMessage("IsolatedActivation", new ActivationPropagation(base.transform.root, channel), SendMessageOptions.DontRequireReceiver);
				}
				if (!muteBehaviour.Muted)
				{
					PhysicalBehaviour.PlayClipOnce(Tick);
				}
			}
		}
		float t = HandCurve.Evaluate(animationTimer);
		float target = (Activated ? Mathf.Lerp(0f - TickAngleBound, TickAngleBound, t) : 0f);
		Vector3 localEulerAngles = Hand.localEulerAngles;
		localEulerAngles.z += Mathf.DeltaAngle(localEulerAngles.z, target) * deltaTime * 6f;
		Hand.localEulerAngles = localEulerAngles;
	}

	private void OnDisable()
	{
		Activated = false;
		StopAllCoroutines();
	}
}

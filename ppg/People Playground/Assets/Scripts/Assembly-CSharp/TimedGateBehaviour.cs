using System.Collections;
using UnityEngine;

public class TimedGateBehaviour : LagboxBehaviour
{
	[SkipSerialisation]
	public SpriteRenderer GreenLight;

	[SkipSerialisation]
	public SpriteRenderer RedLight;

	public AudioClip RedlightSound;

	private const float MinDuration = 0.04f;

	private const float MaxDuration = 16f;

	protected override void Awake()
	{
		base.Awake();
		PhysicalBehaviour.ActivationPropagationDelay = 0f;
	}

	protected override void Start()
	{
		GreenLight.enabled = true;
		RedLight.enabled = false;
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("setTimedGateDuration", "Set timed gate duration", "Set timed gate duration in seconds", delegate
		{
			Utils.OpenFloatInputDialog(DelayModifier, this, delegate(TimedGateBehaviour t, float v)
			{
				t.DelayModifier = Mathf.Clamp(v, 0.04f, 16f);
			}, "Set timed gate duration", $"Target duration in seconds from {0.04f}s to {16f}s");
		}));
		if (isBusy)
		{
			StartCoroutine(DoLag());
		}
	}

	public override void Use(ActivationPropagation activation)
	{
		if (base.enabled && !isBusy)
		{
			t = 0f;
			if (!MuteBehaviour.Muted)
			{
				PhysicalBehaviour.PlayClipOnce(RedlightSound);
			}
			StartCoroutine(DoLag());
		}
	}

	protected override void OnDisable()
	{
		StopAllCoroutines();
	}

	protected override void OnEnable()
	{
		if (isBusy)
		{
			StartCoroutine(DoLag());
		}
	}

	protected override IEnumerator DoLag()
	{
		Knob.eulerAngles = new Vector3(0f, 0f, 0f);
		GreenLight.enabled = false;
		RedLight.enabled = true;
		isBusy = true;
		if (PhysicalBehaviour.ActivationPropagationDelay >= 0f)
		{
			yield return new WaitForEndOfFrame();
		}
		PhysicalBehaviour.ActivationPropagationDelay = -1f;
		while (t < 1f)
		{
			t += Time.deltaTime / DelayModifier;
			Knob.eulerAngles = new Vector3(0f, 0f, t * 180f);
			yield return new WaitForEndOfFrame();
		}
		GreenLight.enabled = true;
		RedLight.enabled = false;
		PhysicalBehaviour.ActivationPropagationDelay = 0f;
		isBusy = false;
		Knob.eulerAngles = new Vector3(0f, 0f, 180f);
		if (!MuteBehaviour.Muted)
		{
			PhysicalBehaviour.PlayClipOnce(Ding);
		}
	}
}

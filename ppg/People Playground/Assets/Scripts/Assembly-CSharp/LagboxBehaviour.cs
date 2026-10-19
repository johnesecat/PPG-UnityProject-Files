using System.Collections;
using UnityEngine;

public class LagboxBehaviour : MonoBehaviour, Messages.IUse
{
	[HideInInspector]
	public float DelayModifier = 1f;

	private const float MinDelay = 0.04f;

	private const float MaxDelay = 60f;

	[SkipSerialisation]
	public Transform Knob;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	public AudioClip Ding;

	[HideInInspector]
	public bool isBusy;

	[HideInInspector]
	public float t;

	protected MuteBehaviour MuteBehaviour;

	protected virtual void Awake()
	{
		MuteBehaviour = GetComponent<MuteBehaviour>();
	}

	protected virtual void Start()
	{
		PhysicalBehaviour.ActivationPropagationDelay = DelayModifier;
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("setLagboxDelay", "Set lagbox delay", "Set lagbox delay in seconds", delegate
		{
			Utils.OpenFloatInputDialog(DelayModifier, this, delegate(LagboxBehaviour t, float v)
			{
				t.DelayModifier = Mathf.Clamp(v, 0.04f, 60f);
				t.PhysicalBehaviour.ActivationPropagationDelay = t.DelayModifier;
			}, "Set lagbox delay", $"Target delay in seconds from {0.04f}s to {60f}s");
		}));
		if (isBusy)
		{
			StartCoroutine(DoLag());
		}
	}

	private void Update()
	{
		Knob.localEulerAngles = new Vector3(0f, 0f, Mathf.Clamp(t * 180f, 0f, 180f));
	}

	public virtual void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			t = 0f;
			StartCoroutine(DoLag());
		}
	}

	protected virtual void OnDisable()
	{
		StopAllCoroutines();
		PhysicalBehaviour.ActivationPropagationDelay = 0.1f;
	}

	protected virtual void OnEnable()
	{
		PhysicalBehaviour.ActivationPropagationDelay = DelayModifier;
	}

	protected virtual IEnumerator DoLag()
	{
		isBusy = true;
		Knob.eulerAngles = new Vector3(0f, 0f, 0f);
		while (t < 1f)
		{
			t += Time.deltaTime / PhysicalBehaviour.ActivationPropagationDelay;
			Knob.eulerAngles = new Vector3(0f, 0f, t * 180f);
			yield return new WaitForEndOfFrame();
		}
		isBusy = false;
		Knob.eulerAngles = new Vector3(0f, 0f, 180f);
		if (!MuteBehaviour.Muted)
		{
			PhysicalBehaviour.PlayClipOnce(Ding);
		}
	}
}

using UnityEngine;

public class ActivatorElectrodeBehaviour : MonoBehaviour, Messages.IUse, Messages.IUseContinuous
{
	[SkipSerialisation]
	public Vector2 EmissionPoint = new Vector2(0.1f, 0f);

	[SkipSerialisation]
	public float EmissionRadius = 0.05f;

	[SkipSerialisation]
	public AudioClip StartUseSound;

	[SkipSerialisation]
	public LayerMask LayersToConsider;

	[SkipSerialisation]
	public ColorAnimationBehaviour[] Lights;

	[SkipSerialisation]
	public SpriteRenderer EmitterGlow;

	private PhysicalBehaviour phys;

	private static readonly Collider2D[] buffer = new Collider2D[4];

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(ProcessEmitContinuous);
	}

	private void Start()
	{
		StopLights();
		EmitterGlow.enabled = false;
	}

	private void ProcessEmitContinuous(float dt)
	{
	}

	public void Use(ActivationPropagation activation)
	{
		if (!base.enabled)
		{
			return;
		}
		int num = Physics2D.OverlapCircleNonAlloc(base.transform.TransformPoint(EmissionPoint), EmissionRadius, buffer, LayersToConsider);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if ((bool)collider2D && collider2D.gameObject != base.gameObject && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value) && !activation.Contains(base.transform.root))
			{
				ActivationPropagation activationPropagation = activation.Branch(base.transform.root);
				value.gameObject.BroadcastMessage("Use", activationPropagation, SendMessageOptions.DontRequireReceiver);
				Utils.Activations.SendOnce(activationPropagation, value);
			}
		}
		Glow();
		StartLights();
		phys.PlayClipOnce(StartUseSound);
	}

	public void UseContinuous(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Collider2D collider2D = Physics2D.OverlapCircle(base.transform.TransformPoint(EmissionPoint), EmissionRadius, LayersToConsider);
			if ((bool)collider2D && collider2D.gameObject != base.gameObject && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value) && !activation.Contains(base.transform.root))
			{
				ActivationPropagation activationPropagation = activation.Branch(base.transform.root);
				value.gameObject.BroadcastMessage("UseContinuous", activationPropagation, SendMessageOptions.DontRequireReceiver);
				Utils.Activations.SendContinuous(activationPropagation, value);
			}
			Glow();
		}
	}

	private void Glow()
	{
		EmitterGlow.enabled = true;
		Color color = EmitterGlow.color;
		color.a = 1f;
		EmitterGlow.color = color;
	}

	private void Update()
	{
		if (EmitterGlow.enabled)
		{
			Color color = EmitterGlow.color;
			color.a -= Time.deltaTime * 4f;
			EmitterGlow.color = color;
			if (color.a <= float.Epsilon)
			{
				EmitterGlow.enabled = false;
			}
		}
	}

	private void OnDestroy()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.RemoveListener(ProcessEmitContinuous);
	}

	public void StopLights()
	{
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].Stop();
			Lights[i].ForceSetProgress(0f);
		}
	}

	public void StartLights()
	{
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].ForceSetProgress(0f);
			Lights[i].Play();
		}
	}
}

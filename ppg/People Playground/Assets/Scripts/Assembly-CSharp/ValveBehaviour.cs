using System.Collections;
using UnityEngine;

public class ValveBehaviour : BloodContainer, Messages.IUse
{
	private PhysicalBehaviour phys;

	[SkipSerialisation]
	public AudioClip TurnSound;

	[SkipSerialisation]
	public Transform ValveTransform;

	[SkipSerialisation]
	public SpriteRenderer DisplayRenderer;

	[SkipSerialisation]
	public float TurnDuration = 0.25f;

	[SkipSerialisation]
	public float MinAngle;

	[SkipSerialisation]
	public float MaxAngle = 90f;

	[SkipSerialisation]
	public AnimationCurve ValveTurnCurve;

	private MaterialPropertyBlock displayProperties;

	private float displayOffset;

	public float ValvePosition;

	public bool LastTarget;

	public override bool AllowsOverflow => false;

	public override bool AllowsTransfer => ValvePosition > 0.5f;

	public override bool AllowPressureTransfer => AllowsTransfer;

	public override Vector2 Limits => new Vector2(0f, 0.5f);

	private void Awake()
	{
		displayProperties = new MaterialPropertyBlock();
		DisplayRenderer.GetPropertyBlock(displayProperties);
		phys = GetComponent<PhysicalBehaviour>();
	}

	protected virtual void Start()
	{
		SetValveRotationToValue();
	}

	public void Use(ActivationPropagation activation)
	{
		switch (activation.Channel)
		{
		case 1:
			StartCoroutine(TurnRoutine(open: true));
			break;
		case 2:
			StartCoroutine(TurnRoutine(open: false));
			break;
		default:
			StartCoroutine(TurnRoutine(!LastTarget));
			break;
		}
	}

	private IEnumerator TurnRoutine(bool open)
	{
		LastTarget = open;
		phys.PlayClipOnce(TurnSound);
		float target = (open ? 1 : 0);
		float t = (open ? ValvePosition : (1f - ValvePosition));
		while (!Mathf.Approximately(ValvePosition, target))
		{
			ValvePosition = ValveTurnCurve.Evaluate(open ? t : (1f - t));
			SetValveRotationToValue();
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime / TurnDuration;
		}
		ValvePosition = (open ? 1 : 0);
		SetValveRotationToValue();
	}

	private void SetValveRotationToValue()
	{
		float z = Mathf.LerpAngle(MinAngle, MaxAngle, ValvePosition);
		ValveTransform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	public void OpenValve()
	{
		StopCoroutine("TurnRoutine");
		StartCoroutine(TurnRoutine(open: true));
	}

	public void CloseValve()
	{
		StopCoroutine("TurnRoutine");
		StartCoroutine(TurnRoutine(open: false));
	}

	protected override void Update()
	{
		base.Update();
		displayOffset = ValvePosition;
	}

	private void OnWillRenderObject()
	{
		displayProperties.SetFloat(ShaderProperties.Get("_Offset"), displayOffset);
		DisplayRenderer.SetPropertyBlock(displayProperties);
	}
}

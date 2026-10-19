using UnityEngine;

public class PistonBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public SliderJoint2D Slider;

	[SkipSerialisation]
	public Transform PistonHead;

	[SkipSerialisation]
	public DamagableMachineryBehaviour DamagableMachinery;

	public float MotorSpeedMutliplier = 5f;

	public float CorrectionSpeedMultiplier = 3f;

	private bool alreadyReached;

	private JointTranslationLimits2D originalLimits;

	private void Start()
	{
		originalLimits = Slider.limits;
		UpdateLimits();
		PistonHead.transform.Translate(0f, 0f, 0.05f);
		if (base.transform.localScale.x < 0f)
		{
			Slider.angle = 180f;
		}
		alreadyReached = false;
	}

	public void Use(ActivationPropagation activation)
	{
		if (!DamagableMachinery.Destroyed)
		{
			if ((bool)AudioSource && AudioSource.enabled)
			{
				AudioSource.Stop();
				AudioSource.Play();
			}
			Activated = !Activated;
			alreadyReached = false;
		}
	}

	private void MovePiston(float direction)
	{
		Slider.useMotor = !DamagableMachinery.Destroyed;
		if (DamagableMachinery.Destroyed)
		{
			return;
		}
		JointLimitState2D jointLimitState2D = ((!(direction > 0f)) ? JointLimitState2D.LowerLimit : JointLimitState2D.UpperLimit);
		if (Slider.limitState != jointLimitState2D)
		{
			float num = 1f;
			if (alreadyReached)
			{
				float num2 = Slider.jointTranslation - ((direction > 0f) ? Slider.limits.max : Slider.limits.min);
				num = Mathf.Clamp01(num2 * num2 * CorrectionSpeedMultiplier);
			}
			SetMotorSpeed(direction * Mathf.Abs(base.transform.localScale.x) * num);
		}
		else
		{
			alreadyReached = true;
			SetMotorSpeed(0f);
		}
	}

	private void Update()
	{
		UpdateLimits();
	}

	private void FixedUpdate()
	{
		MovePiston(Activated ? 1 : (-1));
	}

	private void UpdateLimits()
	{
		JointTranslationLimits2D limits = default(JointTranslationLimits2D);
		float num = Mathf.Abs(base.transform.localScale.x);
		limits.max = originalLimits.max * num;
		limits.min = originalLimits.min * num;
		Slider.limits = limits;
	}

	private void SetMotorSpeed(float v)
	{
		JointMotor2D motor = Slider.motor;
		motor.motorSpeed = v * MotorSpeedMutliplier * Mathf.Max(1f, PhysicalBehaviour.Charge);
		Slider.motor = motor;
	}
}

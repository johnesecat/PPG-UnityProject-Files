using System.Collections;
using UnityEngine;

public class AerialFaithPlateBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public HingeJoint2D launchJoint;

	[SkipSerialisation]
	public PhysicalBehaviour phys;

	[SkipSerialisation]
	public AudioSource audio;

	private bool isBusy;

	private void Start()
	{
		isBusy = false;
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			StartCoroutine(Launch());
		}
	}

	private IEnumerator Launch()
	{
		if (!isBusy)
		{
			isBusy = true;
			launchJoint.useMotor = true;
			audio.Play();
			SetMotorSpeed((-800f - phys.Charge * 20f) * base.transform.root.localScale.x);
			yield return new WaitForSeconds(2f);
			launchJoint.useMotor = false;
			yield return new WaitForSeconds(1f);
			isBusy = false;
		}
	}

	private void SetMotorSpeed(float speed)
	{
		JointMotor2D motor = launchJoint.motor;
		motor.motorSpeed = speed;
		launchJoint.motor = motor;
	}
}

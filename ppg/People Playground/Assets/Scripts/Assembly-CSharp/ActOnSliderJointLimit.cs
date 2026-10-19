using UnityEngine;
using UnityEngine.Events;

public class ActOnSliderJointLimit : MonoBehaviour
{
	[SkipSerialisation]
	public SliderJoint2D[] Joints;

	[SkipSerialisation]
	public JointLimitState2D TriggeringState;

	[SkipSerialisation]
	public UnityEvent OnTrigger = new UnityEvent();

	private void FixedUpdate()
	{
		for (int i = 0; i < Joints.Length; i++)
		{
			if (Joints[i].limitState == TriggeringState)
			{
				OnTrigger.Invoke();
				break;
			}
		}
	}
}

using System;
using UnityEngine;

public class JointMotorSoundBehaviour : MonoBehaviour
{
	public Joint2D Joint;

	public LoopSoundBehaviour LoopSoundBehaviour;

	public float SpeedThreshold = 0.5f;

	private void FixedUpdate()
	{
		if (Joint == null)
		{
			if (LoopSoundBehaviour.IsPlaying)
			{
				LoopSoundBehaviour.StopLoop();
			}
			return;
		}
		Joint2D joint = Joint;
		SliderJoint2D sliderJoint2D = joint as SliderJoint2D;
		JointMotor2D motor;
		if ((object)sliderJoint2D == null)
		{
			HingeJoint2D hingeJoint2D = joint as HingeJoint2D;
			if ((object)hingeJoint2D == null)
			{
				WheelJoint2D wheelJoint2D = joint as WheelJoint2D;
				if ((object)wheelJoint2D == null)
				{
					throw new Exception("Provided joint without motor");
				}
				motor = wheelJoint2D.motor;
			}
			else
			{
				motor = hingeJoint2D.motor;
			}
		}
		else
		{
			motor = sliderJoint2D.motor;
		}
		bool flag = Mathf.Abs(motor.motorSpeed) > SpeedThreshold;
		if (flag && !LoopSoundBehaviour.IsPlaying)
		{
			LoopSoundBehaviour.StartLoop();
		}
		else if (!flag && LoopSoundBehaviour.IsPlaying)
		{
			LoopSoundBehaviour.StopLoop();
		}
	}
}

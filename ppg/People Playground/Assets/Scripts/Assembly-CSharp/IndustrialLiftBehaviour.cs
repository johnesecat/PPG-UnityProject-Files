using NaughtyAttributes;
using UnityEngine;

public class IndustrialLiftBehaviour : MonoBehaviour
{
	public SliderJoint2D A;

	public SliderJoint2D B;

	public int Speed;

	[MinMaxSlider(0f, 100f)]
	public Vector2 MinMaxDistance = new Vector2(0f, 100f);

	public float SpeedMultiplier = 50f;

	public float AccelerationSpeed = 0.1f;

	private float dist;

	public void SetSpeed(int v)
	{
		Speed = v;
	}

	private void Awake()
	{
		if ((bool)A || (bool)B)
		{
			dist = (A ? A.jointTranslation : B.jointTranslation);
		}
	}

	private void FixedUpdate()
	{
		dist += (float)Speed * 0.04f;
		dist = Mathf.Clamp(dist, MinMaxDistance.x, MinMaxDistance.y);
		if ((bool)A)
		{
			ProcessJoint(A);
		}
		if ((bool)B)
		{
			ProcessJoint(B);
		}
	}

	private void ProcessJoint(SliderJoint2D j)
	{
		JointMotor2D motor = j.motor;
		motor.motorSpeed = Mathf.Lerp(motor.motorSpeed, SpeedMultiplier * Mathf.Clamp(dist - j.jointTranslation, -1f, 1f), AccelerationSpeed);
		j.motor = motor;
	}
}

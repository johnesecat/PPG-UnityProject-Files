using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapLiftBehaviour : MonoBehaviour
{
	public List<float> Floors = new List<float>();

	public int TargetFloorIndex;

	public float MotorSpeedMultiplier = 2f;

	private readonly List<int> requestIndexList = new List<int>();

	[SerializeField]
	private SliderJoint2D joint;

	[SerializeField]
	private AudioClip startMoveClip;

	[SerializeField]
	private AudioClip stopMoveClip;

	[SerializeField]
	private AudioSource liftAudioSource;

	[SerializeField]
	private AudioSource liftLoopAudioSource;

	[SerializeField]
	private DamagableMachineryBehaviour damagableMachinery;

	[SerializeField]
	private AudioSource liftFailureLoopAudioSource;

	private float waitAtFloorTimer;

	private const float FloorWaitDuration = 4f;

	private bool isEmergencyStopped;

	private void FixedUpdate()
	{
		if ((bool)joint)
		{
			List<float> floors = Floors;
			if (floors == null || floors.Count != 0)
			{
				if (damagableMachinery.Destroyed)
				{
					liftLoopAudioSource.volume = 0f;
					joint.useMotor = false;
				}
				else if (!isEmergencyStopped)
				{
					TargetFloorIndex = Mathf.Clamp(TargetFloorIndex, 0, Floors.Count);
					float num = Floors[TargetFloorIndex] - joint.jointTranslation;
					if (Mathf.Abs(num) < 0.1f && joint.jointSpeed < 0.5f)
					{
						if (waitAtFloorTimer + float.Epsilon >= 4f)
						{
							liftAudioSource.PlayOneShot(stopMoveClip);
						}
						waitAtFloorTimer -= Time.deltaTime;
						if (requestIndexList.Count > 0 && waitAtFloorTimer <= float.Epsilon)
						{
							int targetFloorIndex = TargetFloorIndex;
							while (targetFloorIndex == TargetFloorIndex && requestIndexList.Count > 0)
							{
								TargetFloorIndex = PopNextNearestTargetFloorIndex(joint.jointTranslation);
							}
							waitAtFloorTimer = 4f;
							if (Mathf.Abs(Floors[TargetFloorIndex] - joint.jointTranslation) > 1f)
							{
								liftAudioSource.PlayOneShot(startMoveClip);
							}
						}
					}
					JointMotor2D motor = joint.motor;
					motor.motorSpeed = Mathf.Lerp(motor.motorSpeed, Mathf.Clamp(num * 0.8f, -1f, 1f) * MotorSpeedMultiplier, 0.1f * Utils.MapRange(0f, 1f, 0.25f, 1f, Mathf.PerlinNoise(Time.time, 3294.1233f)));
					joint.motor = motor;
					liftLoopAudioSource.volume = Mathf.Clamp(Mathf.Abs(motor.motorSpeed * 0.33f), 0f, 1f);
					joint.useMotor = true;
				}
				liftFailureLoopAudioSource.volume = Mathf.Clamp(Mathf.Abs(joint.jointSpeed * 0.001f), 0f, 1f);
				return;
			}
		}
		liftFailureLoopAudioSource.volume = 0f;
		liftLoopAudioSource.volume = 0f;
	}

	private int PopNextNearestTargetFloorIndex(float currentHeight)
	{
		if (!requestIndexList.Any())
		{
			return TargetFloorIndex;
		}
		requestIndexList.Sort((int a, int b) => (Mathf.Abs(currentHeight - Floors[a]) - Mathf.Abs(currentHeight - Floors[b]) > 0f) ? 1 : (-1));
		int result = requestIndexList[0];
		requestIndexList.RemoveAt(0);
		return result;
	}

	public void SetFloor(int i)
	{
		if (i < 0 || i >= Floors.Count)
		{
			return;
		}
		isEmergencyStopped = false;
		if (!joint || requestIndexList.Contains(i))
		{
			return;
		}
		bool flag = TargetFloorIndex >= 0 && TargetFloorIndex < Floors.Count && Mathf.Approximately(waitAtFloorTimer, 4f);
		bool flag2 = Mathf.Abs(joint.jointTranslation - Floors[Mathf.Clamp(TargetFloorIndex, 0, Floors.Count)]) > Mathf.Abs(joint.jointTranslation - Floors[i]);
		bool num = flag && flag2 && joint.jointTranslation - Floors[i] > 0f == joint.jointTranslation - Floors[TargetFloorIndex] > 0f;
		if (requestIndexList.Count == 0 && !flag)
		{
			if (TargetFloorIndex != i)
			{
				liftAudioSource.PlayOneShot(startMoveClip);
			}
			TargetFloorIndex = i;
		}
		if (num)
		{
			requestIndexList.Add(TargetFloorIndex);
			TargetFloorIndex = i;
		}
		else
		{
			requestIndexList.Add(i);
		}
	}

	public void EmergencyStop()
	{
		if (base.enabled && !damagableMachinery.Destroyed && (bool)joint && !(Mathf.Abs(joint.jointSpeed) < 0.1f))
		{
			liftAudioSource.PlayOneShot(stopMoveClip);
			isEmergencyStopped = true;
			JointMotor2D motor = joint.motor;
			motor.motorSpeed = 0f;
			joint.motor = motor;
			liftLoopAudioSource.volume = 0f;
			joint.useMotor = true;
			waitAtFloorTimer = 0f;
			requestIndexList.Clear();
		}
	}
}

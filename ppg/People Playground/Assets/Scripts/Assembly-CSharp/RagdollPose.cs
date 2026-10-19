using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RagdollPose
{
	[Serializable]
	public struct LimbPose
	{
		public string Name;

		public LimbBehaviour Limb;

		public bool Animated;

		public float PoseRigidityModifier;

		[Space]
		[Header("Basic")]
		[Tooltip("Ignored if animation is enabled")]
		public float Angle;

		[Space]
		[Header("Animation")]
		public float StartAngle;

		public float EndAngle;

		public float AnimationDuration;

		[Range(0f, 1f)]
		public float RandomInfluence;

		[Min(1f)]
		public float RandomSpeed;

		[Range(-1f, 1f)]
		public float TimeOffset;

		public AnimationCurve AnimationCurve;

		public float EvaluateAngle(float speedMultiplier = 1f)
		{
			if (!Animated)
			{
				return Angle;
			}
			float num = speedMultiplier * Time.time;
			float num2 = TimeOffset + num / AnimationDuration;
			if (RandomInfluence > 0.01f)
			{
				num2 = Mathf.Lerp(num2, GetPerlinNoise(num), RandomInfluence);
			}
			return Mathf.Lerp(StartAngle, EndAngle, AnimationCurve.Evaluate(num2));
		}

		private float GetPerlinNoise(float t)
		{
			return Mathf.PerlinNoise(Limb.randomOffset, t);
		}

		public float EvaluateAngleAt(float timeOverride)
		{
			if (!Animated)
			{
				return Angle;
			}
			float time = TimeOffset + timeOverride / AnimationDuration;
			return Mathf.Lerp(StartAngle, EndAngle, AnimationCurve.Evaluate(time));
		}

		public LimbPose(LimbBehaviour limb, float angle)
		{
			Limb = limb;
			Angle = angle;
			Name = limb.name;
			Animated = false;
			StartAngle = angle;
			EndAngle = angle;
			TimeOffset = 0f;
			PoseRigidityModifier = 0f;
			RandomInfluence = 0f;
			RandomSpeed = 1f;
			AnimationDuration = 1f;
			AnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			AnimationCurve.postWrapMode = WrapMode.PingPong;
			AnimationCurve.preWrapMode = WrapMode.PingPong;
		}
	}

	public string Name;

	public float Rigidity = 5f;

	public float ForceMultiplier = 1f;

	public bool ShouldStandUpright;

	public float DragInfluence = 1f;

	public float UprightForceMultiplier = 1f;

	public bool ShouldStumble;

	public PoseState State;

	public float AnimationSpeedMultiplier = 1f;

	public List<LimbPose> Angles;

	[HideInInspector]
	public Dictionary<LimbBehaviour, LimbPose> AngleDictionary;

	public void ConstructDictionary()
	{
		AngleDictionary = new Dictionary<LimbBehaviour, LimbPose>();
		foreach (LimbPose angle in Angles)
		{
			AngleDictionary.Add(angle.Limb, angle);
		}
	}
}

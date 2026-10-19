using UnityEngine;

public class RotationAnimationBehaviour : TrivialAnimationBehaviour
{
	public AnimationCurve DegreeOverTime;

	public Vector3 Offset;

	public Vector3 Multiplier = new Vector3(0f, 0f, 1f);

	protected override void AnimationTick(float progress)
	{
		float num = DegreeOverTime.Evaluate(progress);
		base.transform.localEulerAngles = Offset + Multiplier * num;
	}
}

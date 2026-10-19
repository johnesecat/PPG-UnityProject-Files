using UnityEngine;

public class TranslationAnimationBehaviour : TrivialAnimationBehaviour
{
	public AnimationCurve DistanceOverTime;

	public RectTransform.Axis Axis;

	public float Multiplier = 1f;

	private Vector3 startPos;

	private void Awake()
	{
		startPos = base.transform.localPosition;
	}

	protected override void AnimationTick(float progress)
	{
		float num = DistanceOverTime.Evaluate(progress) * Multiplier;
		switch (Axis)
		{
		case RectTransform.Axis.Horizontal:
			base.transform.localPosition = new Vector3(startPos.x + num, startPos.y, startPos.z);
			break;
		case RectTransform.Axis.Vertical:
			base.transform.localPosition = new Vector3(startPos.x, startPos.y + num, startPos.z);
			break;
		}
	}
}

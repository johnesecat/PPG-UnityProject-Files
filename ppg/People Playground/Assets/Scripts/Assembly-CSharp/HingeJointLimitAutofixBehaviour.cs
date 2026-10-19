using UnityEngine;

public class HingeJointLimitAutofixBehaviour : MonoBehaviour
{
	public bool IsFlipped { get; private set; }

	private void Start()
	{
		IsFlipped = false;
		if (!(base.transform.localScale.x > 0f))
		{
			IsFlipped = true;
			FixHinges();
			FixSlider();
			FixedFixed();
		}
	}

	private void FixedFixed()
	{
		FixedJoint2D[] componentsInChildren = GetComponentsInChildren<FixedJoint2D>();
		foreach (FixedJoint2D obj in componentsInChildren)
		{
			Rigidbody2D connectedBody = obj.connectedBody;
			obj.connectedBody = null;
			obj.connectedBody = connectedBody;
		}
	}

	private void FixSlider()
	{
		SliderJoint2D[] componentsInChildren = GetComponentsInChildren<SliderJoint2D>();
		foreach (SliderJoint2D sliderJoint2D in componentsInChildren)
		{
			if (sliderJoint2D.useLimits)
			{
				JointTranslationLimits2D limits = sliderJoint2D.limits;
				limits.max = 0f - sliderJoint2D.limits.min;
				limits.min = 0f - sliderJoint2D.limits.max;
				sliderJoint2D.limits = limits;
			}
		}
	}

	private void FixHinges()
	{
		HingeJoint2D[] componentsInChildren = GetComponentsInChildren<HingeJoint2D>();
		foreach (HingeJoint2D hingeJoint2D in componentsInChildren)
		{
			if (hingeJoint2D.useLimits)
			{
				JointAngleLimits2D limits = hingeJoint2D.limits;
				limits.max = 0f - hingeJoint2D.limits.min;
				limits.min = 0f - hingeJoint2D.limits.max;
				hingeJoint2D.limits = limits;
			}
		}
	}
}

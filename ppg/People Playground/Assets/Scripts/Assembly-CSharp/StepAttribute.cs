using System;

internal class StepAttribute : Attribute
{
	public float Step = 0.1f;

	public StepAttribute(float step)
	{
		Step = step;
	}
}

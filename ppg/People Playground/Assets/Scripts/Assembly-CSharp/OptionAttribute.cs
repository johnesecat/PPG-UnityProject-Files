using System;

internal class OptionAttribute : Attribute
{
	public float[] Options;

	public OptionAttribute(params float[] options)
	{
		Options = options;
	}
}

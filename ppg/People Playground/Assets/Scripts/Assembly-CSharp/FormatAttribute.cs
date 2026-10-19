using System;

public class FormatAttribute : Attribute
{
	public readonly string Format = "";

	public readonly float Multiplier;

	public readonly int DigitCount;

	public FormatAttribute(string format, float multiplier = 1f, int digitCount = 0)
	{
		Format = format;
		Multiplier = multiplier;
		DigitCount = digitCount;
	}

	public string FormatString(float value)
	{
		return string.Format(Format, Math.Round(value * Multiplier, DigitCount));
	}

	public string FormatString(int value)
	{
		return string.Format(Format, (float)value * Multiplier);
	}
}

using System;

[Serializable]
public struct OptionalFloat
{
	public bool Active;

	public float Value;

	public OptionalFloat(float value)
	{
		this = default(OptionalFloat);
		Value = value;
		Active = true;
	}
}

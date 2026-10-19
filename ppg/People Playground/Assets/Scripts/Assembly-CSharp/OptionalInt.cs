using System;

[Serializable]
public struct OptionalInt
{
	public bool Active;

	public int Value;

	public OptionalInt(int value)
	{
		this = default(OptionalInt);
		Value = value;
		Active = true;
	}
}

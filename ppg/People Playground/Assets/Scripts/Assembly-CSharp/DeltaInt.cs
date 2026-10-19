using UnityEngine;

public class DeltaInt
{
	public int Value;

	public int PreviousValue;

	public int PreviousValueFixed;

	public DeltaInt(int value)
	{
		Value = (PreviousValueFixed = (PreviousValue = value));
	}

	public void Increment(int v = 1)
	{
		Value += v;
	}

	public void Decrement()
	{
		Value = Mathf.Max(0, Value - 1);
	}

	public void UpdatePreviousValue()
	{
		PreviousValue = Value;
	}
}

using System;
using UnityEngine;

[Serializable]
public struct SharpAxis
{
	public Vector2 Axis;

	public float LowerLimit;

	public float UpperLimit;

	public bool LooseUpperLimit;

	public bool LooseLowerLimit;

	public SharpAxis(Vector2 axis = default(Vector2), float lowerLimit = 0f, float higherLimit = 100f, bool looseUpper = true, bool looseLower = false)
	{
		Axis = axis;
		LowerLimit = lowerLimit;
		UpperLimit = higherLimit;
		LooseUpperLimit = looseUpper;
		LooseLowerLimit = looseLower;
	}
}

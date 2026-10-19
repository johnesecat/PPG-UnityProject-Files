using System;
using UnityEngine;

[Serializable]
public struct TransformPrototype
{
	public Vector2 RelativePosition;

	public Vector3 LocalScale;

	public float RelativeRotation;

	public TransformPrototype(Vector2 relativePosition, float relativeRotation, Vector3 localScale)
	{
		RelativePosition = relativePosition;
		RelativeRotation = relativeRotation;
		LocalScale = localScale;
	}
}

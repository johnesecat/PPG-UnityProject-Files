using System;
using UnityEngine;

public class ResetTransformBehaviour : MonoBehaviour
{
	[Flags]
	public enum TransformReset
	{
		Position = 0x0,
		Rotation = 0x1,
		Scale = 0x2
	}

	public bool ResetOnStart = true;

	public TransformReset ToReset;

	private void Start()
	{
		if (ResetOnStart)
		{
			ResetTransform();
		}
	}

	public void ResetTransform()
	{
		if (ToReset.HasFlag(TransformReset.Position))
		{
			base.transform.localPosition = Vector3.zero;
		}
		if (ToReset.HasFlag(TransformReset.Rotation))
		{
			base.transform.localRotation = Quaternion.identity;
		}
		if (ToReset.HasFlag(TransformReset.Scale))
		{
			base.transform.localScale = Vector3.one;
		}
	}
}

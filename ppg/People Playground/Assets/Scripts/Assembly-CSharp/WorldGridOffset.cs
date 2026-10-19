using System;
using UnityEngine;

public class WorldGridOffset : MonoBehaviour
{
	public Vector2 GridOffset = new Vector2(-426f / (565f * (float)Math.PI), 0.025f);

	public static Vector3 Offset;

	private void Start()
	{
		Offset = GridOffset;
	}
}

using System;
using UnityEngine;

[Obsolete]
public class BusGeneratorBehaviour : MonoBehaviour
{
	public PhysicalBehaviour PhysicalBehaviour;

	public Vector2 StartPosition;

	public int Amount;

	public Vector2 Offset;

	[Space]
	public Vector2 AnchorOffset;

	public float BreakingThreshold;
}

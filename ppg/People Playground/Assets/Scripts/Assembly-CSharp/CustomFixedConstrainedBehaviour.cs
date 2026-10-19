using System;
using UnityEngine;

[Obsolete]
public class CustomFixedConstrainedBehaviour : MonoBehaviour
{
	public Rigidbody2D Other;

	[Header("Anchor")]
	public Vector2 LocalPosition;

	public float RelativeAngle;

	public bool AutoAnchor = true;

	private Rigidbody2D rb;

	private Vector2 lastPosition;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		if (AutoAnchor)
		{
			LocalPosition = base.transform.InverseTransformPoint(Other.position);
			RelativeAngle = Mathf.DeltaAngle(Other.rotation, rb.rotation);
		}
	}

	private void FixedUpdate()
	{
		Vector2 relativePoint = rb.GetRelativePoint(LocalPosition);
		float angle = Utils.Mod(rb.rotation + RelativeAngle, 360f);
		Other.MovePosition(relativePoint);
		Other.MoveRotation(angle);
	}
}

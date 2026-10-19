using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VibrationBehaviour : MonoBehaviour
{
	public float VibrateForce;

	public float VibrateRotationSpeed;

	private Rigidbody2D rb;

	public Vector2 ForceVector
	{
		get
		{
			float f = Time.time * VibrateRotationSpeed;
			return new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * VibrateForce;
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (Mathf.Abs(VibrateForce) >= float.Epsilon)
		{
			rb.AddRelativeForce(ForceVector, ForceMode2D.Force);
		}
	}
}

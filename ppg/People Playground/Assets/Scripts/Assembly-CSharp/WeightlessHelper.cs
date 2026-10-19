using UnityEngine;

public class WeightlessHelper : MonoBehaviour
{
	[SkipSerialisation]
	public Rigidbody2D[] Bodies;

	private float[] originalMass;

	private void Awake()
	{
		originalMass = new float[Bodies.Length];
		for (int i = 0; i < Bodies.Length; i++)
		{
			Rigidbody2D rigidbody2D = Bodies[i];
			originalMass[i] = rigidbody2D.mass;
		}
	}

	private void OnMakeWeightless()
	{
		for (int i = 0; i < Bodies.Length; i++)
		{
			Rigidbody2D rigidbody2D = Bodies[i];
			rigidbody2D.gravityScale = 0f;
			rigidbody2D.mass = Mathf.Max(Mathf.Min(rigidbody2D.mass, 0.02f), rigidbody2D.mass * 0.02f);
		}
	}

	private void OnMakeWeightful()
	{
		for (int i = 0; i < Bodies.Length; i++)
		{
			Rigidbody2D obj = Bodies[i];
			obj.gravityScale = 1f;
			obj.mass = originalMass[i];
		}
	}
}

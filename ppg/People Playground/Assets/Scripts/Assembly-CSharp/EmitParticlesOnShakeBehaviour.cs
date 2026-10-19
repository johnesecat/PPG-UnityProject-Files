using UnityEngine;

public class EmitParticlesOnShakeBehaviour : MonoBehaviour
{
	public float ShakeThreshold = 3f;

	public float EmissionCount = 1f;

	private Rigidbody2D rb;

	private ParticleSystem ps;

	private Vector2 previousVelocity;

	private void Awake()
	{
		ps = GetComponent<ParticleSystem>();
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		Vector2 velocity = rb.velocity;
		float sqrMagnitude = (previousVelocity - velocity).sqrMagnitude;
		previousVelocity = velocity;
		float num = ShakeThreshold * ShakeThreshold;
		if (sqrMagnitude > num)
		{
			ps.Emit(Mathf.Min(Mathf.CeilToInt(EmissionCount * (sqrMagnitude / num)), 4));
		}
	}
}

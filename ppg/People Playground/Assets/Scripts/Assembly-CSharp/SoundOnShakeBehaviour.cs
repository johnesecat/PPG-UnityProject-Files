using UnityEngine;

public class SoundOnShakeBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public AudioClip[] Clips;

	[SkipSerialisation]
	public AudioSource AudioSource;

	public float ShakeThreshold;

	private Rigidbody2D rb;

	private Vector2 previousVelocity;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (AudioSource.isPlaying)
		{
			previousVelocity = rb.velocity;
			return;
		}
		Vector2 velocity = rb.velocity;
		float sqrMagnitude = (previousVelocity - velocity).sqrMagnitude;
		previousVelocity = velocity;
		float num = ShakeThreshold * ShakeThreshold;
		if (sqrMagnitude > num && Random.value > 0.5f)
		{
			AudioSource.PlayOneShot(Clips.PickRandom(), Mathf.Clamp01(sqrMagnitude / num - 0.5f));
		}
	}
}

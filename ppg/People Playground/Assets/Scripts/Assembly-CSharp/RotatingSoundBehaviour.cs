using UnityEngine;

public class RotatingSoundBehaviour : MonoBehaviour
{
	public AudioClip loop;

	public AudioSource loopingSource;

	public Rigidbody2D myRigidBody;

	public float foleyVolume = 1f;

	private void Awake()
	{
		myRigidBody = GetComponent<Rigidbody2D>();
		loopingSource = base.gameObject.AddComponent<AudioSource>();
		loopingSource.loop = true;
		loopingSource.minDistance = 1f;
		loopingSource.maxDistance = 5f;
		loopingSource.spatialBlend = 1f;
		loopingSource.dopplerLevel = 0f;
		loopingSource.clip = loop;
		loopingSource.Play();
	}

	private void FixedUpdate()
	{
		if (myRigidBody.bodyType == RigidbodyType2D.Dynamic)
		{
			loopingSource.volume = Mathf.Abs(foleyVolume * myRigidBody.angularVelocity);
		}
	}
}

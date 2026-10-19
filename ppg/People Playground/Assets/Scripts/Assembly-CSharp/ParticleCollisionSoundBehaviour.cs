using UnityEngine;

public class ParticleCollisionSoundBehaviour : MonoBehaviour
{
	public AudioClip[] Clips;

	public AudioSource AudioSource;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void OnParticleCollision(GameObject other)
	{
		if (!AudioSource)
		{
			phys.PlayClipOnce(Clips.PickRandom());
		}
		else
		{
			AudioSource.PlayOneShot(Clips.PickRandom());
		}
	}
}

using UnityEngine;

public class RadioBehaviour : MonoBehaviour
{
	private AudioSource audioSource;

	public AudioClip[] clips;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void FixedUpdate()
	{
		if (!audioSource.isPlaying)
		{
			audioSource.pitch = 1f;
			audioSource.clip = clips.PickRandom();
			audioSource.Play();
		}
	}

	private void Shocked(Zap zap)
	{
		if (Random.value > 0.99f)
		{
			audioSource.Stop();
		}
		if (Random.value > 0.99f)
		{
			audioSource.pitch = Random.Range(0.8f, 1.2f);
		}
	}
}

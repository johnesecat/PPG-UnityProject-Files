using UnityEngine;

public class LoopSoundBehaviour : MonoBehaviour
{
	public AudioSource AudioSource;

	public AudioClip StartClip;

	public AudioClip EndClip;

	public bool IsPlaying { get; private set; }

	public void StartLoop()
	{
		AudioSource.Play();
		AudioSource.PlayOneShot(StartClip);
		IsPlaying = true;
	}

	public void StopLoop()
	{
		AudioSource.Stop();
		AudioSource.PlayOneShot(EndClip);
		IsPlaying = false;
	}
}

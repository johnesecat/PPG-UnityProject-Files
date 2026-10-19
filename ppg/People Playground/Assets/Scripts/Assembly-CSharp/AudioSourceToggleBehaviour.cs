using UnityEngine;

public class AudioSourceToggleBehaviour : MonoBehaviour
{
	public AudioSource AudioSource;

	public void Toggle()
	{
		if (AudioSource.isPlaying)
		{
			AudioSource.Stop();
		}
		else
		{
			AudioSource.Play();
		}
	}
}

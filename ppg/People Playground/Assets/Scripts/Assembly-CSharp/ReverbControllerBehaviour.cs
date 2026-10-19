using System;
using UnityEngine;

[Obsolete]
public class ReverbControllerBehaviour : MonoBehaviour
{
	private static ReverbControllerBehaviour main;

	public AudioClip[] ReverbSounds;

	public AudioSource LeftAudioSource;

	public AudioSource RightAudioSource;

	private void Awake()
	{
		main = this;
	}

	public static void PlayReverb(float intensity = 0.2f)
	{
		main.PlayReverbAt(main.LeftAudioSource, intensity);
		main.PlayReverbAt(main.RightAudioSource, intensity);
	}

	private void PlayReverbAt(AudioSource source, float intensity)
	{
		source.volume = intensity;
		source.clip = main.ReverbSounds.PickRandom();
		source.Stop();
		source.Play();
	}
}

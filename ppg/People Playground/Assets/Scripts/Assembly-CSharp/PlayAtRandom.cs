using UnityEngine;

public class PlayAtRandom : MonoBehaviour
{
	public ParticleSystem[] ParticleSystems;

	public AudioSource[] AudioSources;

	[Range(0f, 1f)]
	public float Chance = 0.5f;

	private void Start()
	{
		if (!(Random.value > Chance))
		{
			ParticleSystem[] particleSystems = ParticleSystems;
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].Play();
			}
			AudioSource[] audioSources = AudioSources;
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].Play();
			}
		}
	}
}

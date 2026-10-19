using NaughtyAttributes;
using UnityEngine;

public class ParticleSystemSerialisationHelper : MonoBehaviour
{
	public ParticleSystem ParticleSystems;

	[ReadOnly]
	[SerializeField]
	public bool IsPlaying;

	private void Update()
	{
		IsPlaying = (bool)ParticleSystems && ParticleSystems.isEmitting;
	}

	private void Start()
	{
		if ((bool)ParticleSystems)
		{
			if (IsPlaying)
			{
				ParticleSystems.Play();
			}
			else
			{
				ParticleSystems.Stop();
			}
		}
	}
}

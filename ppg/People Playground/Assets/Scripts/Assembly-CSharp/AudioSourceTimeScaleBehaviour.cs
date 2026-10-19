using UnityEngine;

public class AudioSourceTimeScaleBehaviour : MonoBehaviour
{
	public bool IsAmbience;

	private void Start()
	{
		AudioSource[] componentsInChildren = GetComponentsInChildren<AudioSource>();
		foreach (AudioSource a in componentsInChildren)
		{
			Global.main.AddAudioSource(a, IsAmbience);
		}
	}

	private void OnDestroy()
	{
		AudioSource[] componentsInChildren = GetComponentsInChildren<AudioSource>();
		foreach (AudioSource audioSource in componentsInChildren)
		{
			Global.main.RemoveAudioSource(audioSource);
		}
	}
}

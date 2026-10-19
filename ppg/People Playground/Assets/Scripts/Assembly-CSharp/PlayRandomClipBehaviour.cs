using NaughtyAttributes;
using UnityEngine;

public class PlayRandomClipBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public AudioClip[] Clips;

	[SkipSerialisation]
	[HideIf("UsePhysicalBehaviourAudioSource")]
	public AudioSource Source;

	[SkipSerialisation]
	[ShowIf("UsePhysicalBehaviourAudioSource")]
	public PhysicalBehaviour PhysicalBehaviour;

	public bool PlayOnStart;

	public bool UsePhysicalBehaviourAudioSource;

	private void Start()
	{
		if (PlayOnStart)
		{
			Play();
		}
	}

	public void Play()
	{
		if (UsePhysicalBehaviourAudioSource)
		{
			PhysicalBehaviour.PlayClipOnce(Clips.PickRandom());
		}
		else
		{
			Source.PlayOneShot(Clips.PickRandom());
		}
	}
}

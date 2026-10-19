using UnityEngine;

public class RandomSoundBehaviour : MonoBehaviour
{
	public AudioClip[] Clips;

	public AudioSource AudioSource;

	public bool PlayOnStart;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		if (PlayOnStart)
		{
			PlayClip();
		}
	}

	public void PlayClip()
	{
		if (Clips != null && Clips.Length != 0 && base.enabled)
		{
			if ((bool)AudioSource)
			{
				AudioSource.PlayOneShot(Clips.PickRandom());
			}
			else if ((bool)phys)
			{
				phys.PlayClipOnce(Clips.PickRandom());
			}
		}
	}
}

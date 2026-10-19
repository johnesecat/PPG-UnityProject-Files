using NaughtyAttributes;
using UnityEngine;

public class SirenBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public AudioClip SirenSound;

	[SkipSerialisation]
	public AudioClip BrokenSirenSound;

	[SkipSerialisation]
	public DamagableMachineryBehaviour DamagableMachineryBehaviour;

	[ReadOnly]
	public bool Activated;

	private float time;

	[SkipSerialisation]
	public float RestartDuration = 1f;

	public void Use(ActivationPropagation activation)
	{
		Activated = !Activated;
	}

	private void Update()
	{
		if (Activated)
		{
			time += Time.deltaTime;
			if (time > RestartDuration)
			{
				time = 0f;
				AudioSource.PlayOneShot(DamagableMachineryBehaviour.Destroyed ? BrokenSirenSound : SirenSound);
			}
		}
	}
}

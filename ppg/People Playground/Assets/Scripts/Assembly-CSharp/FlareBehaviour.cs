using UnityEngine;

public class FlareBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public AudioSource FlareAudioSource;

	[SkipSerialisation]
	public ParticleSystem FlareSystem;

	[SkipSerialisation]
	public SpriteRenderer FlareTopSprite;

	[SkipSerialisation]
	public ParticleSystem FlareTopSystem;

	[SkipSerialisation]
	public TorchBehaviour TorchBehaviour;

	public bool Activated;

	private void Start()
	{
		if (Activated)
		{
			TorchBehaviour.enabled = true;
			FlareTopSprite.enabled = false;
			FlareAudioSource.Play();
			FlareSystem.Play();
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (!Activated)
		{
			TorchBehaviour.enabled = true;
			FlareTopSprite.enabled = false;
			FlareSystem.Play(withChildren: true);
			FlareTopSystem.Play(withChildren: false);
			FlareAudioSource.Play();
			Activated = true;
		}
	}
}

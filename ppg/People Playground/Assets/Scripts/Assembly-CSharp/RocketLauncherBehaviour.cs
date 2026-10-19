using NaughtyAttributes;
using UnityEngine;

public class RocketLauncherBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public GameObject Projectile;

	[SkipSerialisation]
	public AudioSource AudioSource;

	public float ScreenShakeIntensity = 0.5f;

	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	[SkipSerialisation]
	[InfoBox("This can be empty or null because it will fall back to the clip that's already in the audio source.", EInfoBoxType.Normal)]
	public AudioClip[] LaunchingSounds;

	public Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			if (LaunchingSounds != null && LaunchingSounds.Length != 0)
			{
				AudioSource.PlayOneShot(LaunchingSounds.PickRandom());
			}
			else
			{
				AudioSource.PlayOneShot(AudioSource.clip);
			}
			CameraShakeBehaviour.main.Shake(ScreenShakeIntensity, BarrelPosition);
			Object.Instantiate(Projectile, BarrelPosition, Quaternion.identity).transform.right = BarrelDirection;
		}
	}
}

using UnityEngine;

public class GenericScifiWeapon40Behaviour : MonoBehaviour
{
	[SkipSerialisation]
	public AudioClip ShotSound;

	[SkipSerialisation]
	public float ShotVolume = 1f;

	[SkipSerialisation]
	public ParticleSystem ParticleSystem;

	[SkipSerialisation]
	public Rigidbody2D Rigidbody;

	[SkipSerialisation]
	public float RecoilForce;

	[SkipSerialisation]
	public float ImpactForce;

	[SkipSerialisation]
	public LayerMask LayersToHit;

	[SkipSerialisation]
	public GameObject Impact;

	[SkipSerialisation]
	public Vector3 barrelPosition;

	[SkipSerialisation]
	public Vector3 barrelDirection;

	private AudioSource audioSource;

	public Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	private void Awake()
	{
		audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.spread = 35f;
		audioSource.volume = ShotVolume;
		audioSource.minDistance = 15f;
		audioSource.spatialBlend = 1f;
		audioSource.dopplerLevel = 0f;
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			ParticleSystem.Play();
			audioSource.PlayOneShot(ShotSound);
			Fire();
		}
	}

	public void Fire()
	{
		if (!base.enabled)
		{
			return;
		}
		Rigidbody.AddForceAtPosition((0f - RecoilForce) * base.transform.lossyScale.x * base.transform.right, BarrelPosition, ForceMode2D.Impulse);
		RaycastHit2D raycastHit2D = Physics2D.Raycast(BarrelPosition, BarrelDirection, 2500f, LayersToHit);
		if ((bool)raycastHit2D.transform && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
		{
			if (Random.value * 0.1f <= value.Properties.Softness || (Random.value > 0.9f && !value.isDisintegrated))
			{
				value.SendMessage("Slice", SendMessageOptions.DontRequireReceiver);
				value.SendMessage("Damage", float.MaxValue, SendMessageOptions.DontRequireReceiver);
				Object.Instantiate(Impact, raycastHit2D.point, Quaternion.identity);
				value.Disintegrate();
				ExplosionCreator.CreatePulseExplosion(raycastHit2D.point, ImpactForce / 2f, 1f, soundAndEffects: false);
			}
			else
			{
				value.SendMessage("Shot", new Shot(raycastHit2D.normal, raycastHit2D.point, 15f), SendMessageOptions.DontRequireReceiver);
				Vector2 force = raycastHit2D.normal * (0f - ImpactForce);
				value.rigidbody.AddForceAtPosition(force, raycastHit2D.point, ForceMode2D.Impulse);
			}
		}
	}
}

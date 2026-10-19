using UnityEngine;

public abstract class EffectGunBehaviour : CanShoot, Messages.IUse
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public AudioClip[] ShotSounds;

	[SkipSerialisation]
	public GameObject Muzzleflash;

	public Vector3 barrelPosition;

	public Vector3 barrelDirection;

	public float range = 1f;

	public float offset = 1f;

	private Collider2D[] buffer = new Collider2D[16];

	private AudioSource audioSource;

	public override Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	private void Awake()
	{
		audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.spread = 35f;
		audioSource.volume = 1f;
		audioSource.minDistance = 15f;
		audioSource.spatialBlend = 1f;
		audioSource.dopplerLevel = 0f;
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Object.Instantiate(Muzzleflash, BarrelPosition, base.transform.rotation).transform.right = BarrelDirection;
			Shoot();
		}
	}

	protected bool TryGetLimb(Collider2D coll, out LimbBehaviour limb)
	{
		limb = null;
		if ((bool)coll.transform)
		{
			return coll.transform.gameObject.TryGetComponent<LimbBehaviour>(out limb);
		}
		return false;
	}

	public override void Shoot()
	{
		audioSource.PlayOneShot(ShotSounds.PickRandom());
		int num = Physics2D.OverlapCircleNonAlloc(BarrelPosition + BarrelDirection * offset, range, buffer);
		for (int i = 0; i < num; i++)
		{
			Affect(buffer[i]);
		}
	}

	protected abstract void Affect(Collider2D coll);
}

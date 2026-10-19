using NaughtyAttributes;
using UnityEngine;

public class BlasterBehaviour : CanShoot, Messages.IUse, Messages.IUseContinuous
{
	[SkipSerialisation]
	public GameObject Bolt;

	public float Recoil = 1f;

	[SkipSerialisation]
	public ParticleSystem Muzzleflash;

	[SkipSerialisation]
	public AudioSource blasterSoundSource;

	public float InaccuracyMultiplier = 0.05f;

	public bool Automatic;

	[ShowIf("Automatic")]
	public float Interval = 0.05f;

	public AudioClip[] Clips;

	[SkipSerialisation]
	public float ScreenShakeMultiplier = 1f;

	private PhysicalBehaviour physicalBehaviour;

	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	private float t;

	[SkipSerialisation]
	public bool FlipMuzzleFlash = true;

	public override Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		if (base.transform.localScale.x < 0f && FlipMuzzleFlash)
		{
			Muzzleflash.transform.eulerAngles = new Vector3(0f, 0f, 180f);
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Shoot();
		}
	}

	public void UseContinuous(ActivationPropagation activation)
	{
		ContinuousActivationBehaviour.AssertState();
		if (Automatic && !(t < Interval) && base.enabled)
		{
			Shoot();
		}
	}

	private void Update()
	{
		t += Time.deltaTime;
	}

	public override void Shoot()
	{
		Vector2 vector = BarrelPosition;
		Vector2 vector2 = BarrelDirection;
		t = 0f;
		physicalBehaviour.rigidbody.AddForceAtPosition(vector2 * (0f - Recoil), vector, ForceMode2D.Impulse);
		Object.Instantiate(Bolt, vector, Quaternion.identity).transform.right = vector2 + InaccuracyMultiplier * Random.insideUnitCircle;
		blasterSoundSource.PlayOneShot((Clips != null && Clips.Length != 0) ? Clips.PickRandom() : blasterSoundSource.clip);
		CameraShakeBehaviour.main.Shake(0.5f * ScreenShakeMultiplier, vector);
		Muzzleflash.Play();
	}
}

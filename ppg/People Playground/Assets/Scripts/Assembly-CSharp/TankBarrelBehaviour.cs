using UnityEngine;

public class TankBarrelBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public GameObject Projectile;

	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	[SkipSerialisation]
	public GameObject muzzleFlash;

	[SkipSerialisation]
	public Rigidbody2D myRigidBody;

	[SkipSerialisation]
	public AudioSource foleyAudioSource;

	public float foleyVolume = 1f;

	[SkipSerialisation]
	public float RecoilIntensity = 15f;

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			GameObject obj = Object.Instantiate(Projectile, GetBarrelPosition(), Quaternion.identity);
			ExplosionCreator.CreatePulseExplosion(GetBarrelPosition(), 3f, 10f, soundAndEffects: false);
			obj.transform.right = GetBarrelDirection();
			obj.GetComponent<PhysicalBehaviour>().Temperature = 300f;
			myRigidBody.AddRelativeForce(RecoilIntensity * base.transform.lossyScale.x * Vector2.left, ForceMode2D.Impulse);
			Object.Instantiate(muzzleFlash, GetBarrelPosition(), Quaternion.identity).transform.right = GetBarrelDirection();
		}
	}

	public Vector2 GetBarrelPosition()
	{
		return base.transform.TransformPoint(barrelPosition);
	}

	public Vector2 GetBarrelDirection()
	{
		return base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;
	}

	private void FixedUpdate()
	{
		if (myRigidBody.bodyType == RigidbodyType2D.Dynamic)
		{
			foleyAudioSource.volume = Mathf.Abs(foleyVolume * myRigidBody.angularVelocity);
		}
	}
}

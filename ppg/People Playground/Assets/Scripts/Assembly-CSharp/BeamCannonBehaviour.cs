using System.Collections;
using UnityEngine;

public class BeamCannonBehaviour : CanShoot, Messages.IUse
{
	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public LineRenderer LineRenderer;

	[SkipSerialisation]
	public SpriteRenderer GlowSprite;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public float OverchargeThreshold = 50f;

	[SkipSerialisation]
	public float ScreenShake = 5f;

	[SkipSerialisation]
	public float Recoil = 25f;

	[SkipSerialisation]
	public LayerMask LayerMask;

	private readonly RaycastHit2D[] buffer = new RaycastHit2D[32];

	private int bufferLength;

	public override Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	private void Awake()
	{
		LineRenderer.useWorldSpace = false;
		LineRenderer.enabled = false;
	}

	public void Use(ActivationPropagation activation)
	{
		Shoot();
	}

	public override void Shoot()
	{
		PhysicalBehaviour.rigidbody.AddForceAtPosition(BarrelDirection * (0f - Recoil), BarrelPosition, ForceMode2D.Impulse);
		StartCoroutine((PhysicalBehaviour.Charge > OverchargeThreshold) ? OverchargedFireRoutine() : NormalFireRoutine());
	}

	private IEnumerator NormalFireRoutine()
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(BarrelPosition, BarrelDirection, 100000f, LayerMask);
		if ((bool)raycastHit2D.transform)
		{
			LineRenderer.SetPosition(1, base.transform.InverseTransformPoint(raycastHit2D.point));
			SetupGlowSprite(raycastHit2D.distance);
			OnHit(raycastHit2D.transform, raycastHit2D.point);
		}
		else
		{
			LineRenderer.SetPosition(1, new Vector3(100000f, 0f));
			SetupGlowSprite(100000f);
		}
		AudioSource.PlayOneShot(AudioSource.clip);
		CameraShakeBehaviour.main.Shake(ScreenShake, base.transform.position);
		if (PhysicalBehaviour.SimulateTemperature)
		{
			PhysicalBehaviour.Temperature += 30f;
		}
		yield return BlinkBeam();
	}

	private IEnumerator OverchargedFireRoutine()
	{
		bufferLength = Physics2D.LinecastNonAlloc(BarrelPosition, BarrelPosition + BarrelDirection * 100000f, buffer);
		LineRenderer.SetPosition(1, new Vector3(100000f, 0f));
		SetupGlowSprite(100000f);
		for (int i = 0; i < bufferLength; i++)
		{
			RaycastHit2D raycastHit2D = buffer[i];
			if ((bool)raycastHit2D.transform)
			{
				OnHit(raycastHit2D.transform, raycastHit2D.point);
			}
		}
		AudioSource.PlayOneShot(AudioSource.clip);
		CameraShakeBehaviour.main.Shake(ScreenShake * 2f, base.transform.position);
		yield return BlinkBeam();
	}

	private IEnumerator BlinkBeam()
	{
		LineRenderer.enabled = true;
		GlowSprite.enabled = true;
		yield return new WaitForSeconds(0.05f);
		LineRenderer.enabled = false;
		GlowSprite.enabled = false;
	}

	private void OnHit(Transform other, Vector3 pos)
	{
		ExplosionCreator.ExplosionParameters ex = default(ExplosionCreator.ExplosionParameters);
		ex.FragmentationRayCount = 32u;
		ex.Position = pos;
		ex.Range = 2f;
		ex.FragmentForce = 2f;
		ex.CreateParticlesAndSound = true;
		ex.LargeExplosionParticles = true;
		ex.DismemberChance = 1f;
		ExplosionCreator.Explode(ex);
		if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(other, out var value) && value.gameObject.layer != LayerMask.NameToLayer("Bounds"))
		{
			if (value.SimulateTemperature)
			{
				value.Temperature += 300f;
			}
			value.Ignite(ignoreFlammability: true, pos);
			value.rigidbody.AddForceAtPosition(BarrelDirection * 5f, pos, ForceMode2D.Impulse);
			value.burnIntensity = 1f;
			value.SendMessage("Break", BarrelDirection * 25f, SendMessageOptions.DontRequireReceiver);
			value.SendMessage("Slice", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void SetupGlowSprite(float length)
	{
		GlowSprite.transform.localPosition = new Vector3(length / 2f, 0f);
		GlowSprite.transform.localScale = 2f * new Vector3(length, length);
	}
}

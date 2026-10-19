using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCannonBehaviour : CanShoot, Messages.IUse
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public LineRenderer RayRenderer;

	[SkipSerialisation]
	public ParticleSystem MuzzleParticles;

	[SkipSerialisation]
	public ParticleSystem ImpactParticles;

	[SkipSerialisation]
	public ParticleSystem ChargeUpParticles;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public AudioClip CannonClip;

	[SkipSerialisation]
	public SpriteRenderer GlowSprite;

	[SkipSerialisation]
	public LayerMask LayerMask;

	[SkipSerialisation]
	public float RayCastWidth;

	[SkipSerialisation]
	public float RayDelay = 0.296f;

	[SkipSerialisation]
	public float RayDuration = 1.058f;

	[SkipSerialisation]
	public float ShakeIntensity = 2f;

	[SkipSerialisation]
	public float AmbientTemperatureIncrease = 30f;

	[SkipSerialisation]
	public float Recoil = 12f;

	[SkipSerialisation]
	public ExplosionCreator.ExplosionParameters ExplosionParams;

	[SkipSerialisation]
	public GameObject ExplosionPrefab;

	public Vector2 localBarrelPosition;

	public Vector2 localBarrelDirection;

	private bool isBusy;

	public override Vector2 BarrelPosition => PhysicalBehaviour.rigidbody.GetRelativePoint(localBarrelPosition * base.transform.localScale);

	public Vector2 BarrelDirection => PhysicalBehaviour.rigidbody.GetRelativeVector(localBarrelDirection) * base.transform.localScale.x;

	private void Start()
	{
		RayRenderer.enabled = false;
		GlowSprite.enabled = false;
		MuzzleParticles.Stop();
		ImpactParticles.Stop();
	}

	public override void Shoot()
	{
		if (base.enabled)
		{
			StartCoroutine(ShootRoutine());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		AudioSource.Stop();
		RayRenderer.enabled = false;
		GlowSprite.enabled = false;
		ImpactParticles.Stop();
		MuzzleParticles.Stop();
		isBusy = false;
	}

	private IEnumerator ShootRoutine()
	{
		if (isBusy)
		{
			yield break;
		}
		isBusy = true;
		AudioSource.PlayOneShot(CannonClip);
		ChargeUpParticles.Play();
		yield return new WaitForSeconds(RayDelay);
		Vector2 dir = BarrelDirection;
		Vector2 pos = BarrelPosition;
		RayRenderer.enabled = true;
		GlowSprite.enabled = true;
		ImpactParticles.Play();
		MuzzleParticles.Play();
		bool ambientTemp = UserPreferenceManager.Current.AmbientTemperatureTransfer;
		RaycastHit2D raycastHit2D;
		Vector2 vector;
		for (float t = 0f; t < RayDuration; t += Time.fixedDeltaTime)
		{
			PhysicalBehaviour.rigidbody.AddForce(BarrelDirection * (0f - Recoil), ForceMode2D.Force);
			dir = BarrelDirection;
			pos = BarrelPosition;
			raycastHit2D = Physics2D.CircleCast(pos, RayCastWidth, dir, 10000f, LayerMask);
			vector = (raycastHit2D.transform ? raycastHit2D.point : (pos + dir * 10000f));
			ImpactParticles.transform.position = vector;
			RayRenderer.SetPosition(1, RayRenderer.transform.InverseTransformPoint(vector));
			CameraShakeBehaviour.main.Shake(ShakeIntensity, base.transform.position);
			GlowSprite.transform.position = (vector + pos) * 0.5f;
			GlowSprite.transform.localScale = Vector3.one * raycastHit2D.distance;
			if ((bool)raycastHit2D.transform && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
			{
				AffectObject(value, raycastHit2D.point, raycastHit2D.normal);
			}
			if (ambientTemp)
			{
				HeatUpRayArea(pos, vector);
			}
			yield return new WaitForFixedUpdate();
		}
		raycastHit2D = Physics2D.CircleCast(pos, RayCastWidth, dir, 10000f, LayerMask);
		vector = (raycastHit2D.transform ? raycastHit2D.point : (pos + dir * 10000f));
		ExplosionParams.Position = vector;
		ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(vector), ExplosionParams);
		ExplosionCreator.CreatePulseExplosion(vector, ExplosionParams.FragmentForce, ExplosionParams.Range, soundAndEffects: false);
		UnityEngine.Object.Instantiate(ExplosionPrefab, vector, Quaternion.identity);
		MuzzleParticles.Stop();
		ImpactParticles.Stop();
		GlowSprite.enabled = false;
		RayRenderer.enabled = false;
		isBusy = false;
		yield return new WaitForSeconds(0.5f);
	}

	private void HeatUpRayArea(Vector2 pos, Vector2 endPoint)
	{
		AmbientTemperatureGridBehaviour instance = AmbientTemperatureGridBehaviour.Instance;
		Vector2Int vector2Int = instance.WorldToGridPoint(pos.x, pos.y);
		Vector2Int vector2Int2 = instance.WorldToGridPoint(endPoint.x, endPoint.y);
		foreach (Vector2Int item in line(vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y))
		{
			instance.EnsureExistence(item.x, item.y);
			instance.ChangeTemperatureAt(item.x, item.y, AmbientTemperatureIncrease);
		}
		IEnumerable<Vector2Int> line(int x, int y, int x2, int y2)
		{
			int num = x2 - x;
			int num2 = y2 - y;
			int dx1 = 0;
			int dy1 = 0;
			int dx2 = 0;
			int dy2 = 0;
			if (num < 0)
			{
				dx1 = -1;
			}
			else if (num > 0)
			{
				dx1 = 1;
			}
			if (num2 < 0)
			{
				dy1 = -1;
			}
			else if (num2 > 0)
			{
				dy1 = 1;
			}
			if (num < 0)
			{
				dx2 = -1;
			}
			else if (num > 0)
			{
				dx2 = 1;
			}
			int longest = Math.Abs(num);
			int shortest = Math.Abs(num2);
			if (longest <= shortest)
			{
				longest = Math.Abs(num2);
				shortest = Math.Abs(num);
				if (num2 < 0)
				{
					dy2 = -1;
				}
				else if (num2 > 0)
				{
					dy2 = 1;
				}
				dx2 = 0;
			}
			int numerator = longest >> 1;
			for (int i = 0; i <= longest; i++)
			{
				yield return new Vector2Int(x, y);
				numerator += shortest;
				if (numerator >= longest)
				{
					numerator -= longest;
					x += dx1;
					y += dy1;
				}
				else
				{
					x += dx2;
					y += dy2;
				}
			}
		}
	}

	private void AffectObject(PhysicalBehaviour phys, Vector2 pos, Vector2 normal)
	{
		if (phys.isDisintegrated)
		{
			return;
		}
		if (phys.SimulateTemperature)
		{
			phys.Temperature += UnityEngine.Random.Range(40, 60);
		}
		if (phys.rigidbody.bodyType == RigidbodyType2D.Dynamic)
		{
			phys.rigidbody.AddForceAtPosition(normal * -5f, pos);
		}
		if (UnityEngine.Random.value > 0.95f)
		{
			phys.SendMessage("Slice", SendMessageOptions.DontRequireReceiver);
		}
		if (phys.TryGetComponent<LimbBehaviour>(out var component))
		{
			component.InternalTemperature += 5f;
			component.SkinMaterialHandler.AddDamagePoint(DamageType.Burn, pos, 40f);
			if (UnityEngine.Random.value > 0.95f)
			{
				component.Crush();
			}
		}
	}

	public void Use(ActivationPropagation activation)
	{
		Shoot();
	}
}

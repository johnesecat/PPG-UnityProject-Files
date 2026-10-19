using System;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerBehaviour : MonoBehaviour
{
	public enum SprayEffect
	{
		Ignite,
		Extinguish
	}

	[SkipSerialisation]
	public GameObject particlePrefab;

	[SkipSerialisation]
	public Transform Muzzle;

	[SkipSerialisation]
	public AudioClip IgnitionSound;

	[SkipSerialisation]
	public AudioSource IgnitionSource;

	[SkipSerialisation]
	public SprayEffect Effect;

	[SkipSerialisation]
	public LayerMask LayerMask;

	[Space]
	[SkipSerialisation]
	public Vector2 Point;

	[SkipSerialisation]
	public int RayCount = 16;

	[SkipSerialisation]
	public float Range = 5f;

	[SkipSerialisation]
	public float Angle = 30f;

	private static readonly Collider2D[] buffer = new Collider2D[32];

	private float time;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Update()
	{
		time += Time.deltaTime;
		if (phys.StartedBeingUsedContinuously())
		{
			IgnitionSource.PlayOneShot(IgnitionSound);
			IgnitionSource.Play();
			IgnitionSource.time = UnityEngine.Random.value * IgnitionSource.clip.length;
			time = 0f;
		}
		if (phys.IsBeingUsedContinuously() && phys.Wetness <= 0f)
		{
			time += Time.deltaTime;
			if (time > 0.02f)
			{
				time = 0f;
				GameObject gameObject = PoolGenerator.Instance.RequestPrefab(particlePrefab, Muzzle.position);
				if ((bool)gameObject)
				{
					gameObject.transform.right = Muzzle.right * base.transform.localScale.x;
					gameObject.transform.localScale = new Vector3(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y, 1f);
				}
				phys.rigidbody.AddForceAtPosition(base.transform.right * (-0.75f * base.transform.localScale.x), Muzzle.position, ForceMode2D.Force);
				AffectPoint(base.transform.TransformPoint(Point), Muzzle.right * base.transform.localScale.x);
				foreach (Ray2D allRay in GetAllRays())
				{
					RaycastHit2D raycastHit2D = Physics2D.CircleCast(allRay.origin, 0.05f, allRay.direction, Range, LayerMask);
					if ((bool)raycastHit2D.transform)
					{
						AffectPoint(raycastHit2D.point, allRay.direction);
					}
				}
			}
		}
		if (phys.StoppedBeingUsedContinuously() && IgnitionSource.isPlaying)
		{
			IgnitionSource.Stop();
		}
	}

	private void AffectPoint(Vector2 worldPoint, Vector2 dir)
	{
		int num = Physics2D.OverlapCircleNonAlloc(worldPoint, 0.1f, buffer);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider = buffer[i];
			AffectCollider(collider, dir, worldPoint);
		}
	}

	private void AffectCollider(Collider2D collider, Vector2 dir, Vector2 point)
	{
		if (collider.transform.root == base.transform.root || !Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider.transform, out var value))
		{
			return;
		}
		switch (Effect)
		{
		case SprayEffect.Ignite:
			value.Temperature += Time.deltaTime / value.GetHeatCapacity() * 2f;
			value.Ignite(point);
			if (value.OnFire)
			{
				value.burnIntensity = 1f;
			}
			break;
		case SprayEffect.Extinguish:
			value.Extinguish();
			value.burnIntensity = 0f;
			break;
		}
		value.rigidbody.AddForce(dir * UnityEngine.Random.Range(0f, 0.2f), ForceMode2D.Force);
	}

	private IEnumerable<Ray2D> GetAllRays()
	{
		Vector3 center = base.transform.TransformPoint(Point);
		float baseAngle = base.transform.eulerAngles.z + (float)((!(base.transform.localScale.x > 0f)) ? 180 : 0);
		for (int i = 0; i < RayCount; i++)
		{
			float f = (float)Math.PI / 180f * (baseAngle + Angle * ((float)i / (float)RayCount) - Angle / 2f);
			yield return new Ray2D(center, new Vector2(Mathf.Cos(f), Mathf.Sin(f)));
		}
	}
}

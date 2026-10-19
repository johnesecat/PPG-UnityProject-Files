using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallisticsEmitter : IDisposable
{
	public struct CallbackParams
	{
		public Collider2D HitObject;

		public Vector2 SurfaceNormal;

		public Vector2 Direction;

		public Vector2 Position;
	}

	public ExplosionCreator.ExplosionParameters ExplosiveRoundParams;

	public readonly UnityEvent<LineRenderer> OnTracerCreation = new UnityEvent<LineRenderer>();

	private HashSet<LineRenderer> tracers = new HashSet<LineRenderer>();

	private float lastCrackPlayTime = float.MinValue;

	public uint MaxBallisticsIterations { get; set; } = 128u;


	public float MaxTotalDistance { get; set; } = 1000f;


	public float ThicknessStepSize { get; set; } = 0.08f;


	public float RicochetChanceMultiplier { get; set; } = 1f;


	public float MaxRange { get; set; } = 1000f;


	public float MaterialHardnessMultiplier { get; set; } = 0.35f;


	public float ImpactForceMultiplier { get; set; } = 18f;


	public LayerMask LayersToHit { get; set; }

	public LayerMask WaterLayer { get; set; }

	public bool DoTraversal { get; set; }

	public float TracerWidth { get; set; } = 0.02f;


	public bool RenderTracersIfTraversal { get; set; } = true;


	public float BulletDropMultiplier { get; set; } = 0.25f;


	public float Charge { get; set; }

	public float BulletCrackSpeedThreshold { get; set; } = 5f;


	public float BulletCrackChance { get; set; } = 0.8f;


	public float BulletCrackCooldownTime { get; set; } = 0.25f;


	public bool ExplosiveRound { get; set; }

	public bool ExplodeAtHitPoint { get; set; }

	public MonoBehaviour ConnectedBehaviour { get; set; }

	public Cartridge Cartridge { get; set; }

	public Action<CallbackParams> BulletEntryCallback { get; set; }

	public Action<CallbackParams> BulletExitCallback { get; set; }

	public Action<CallbackParams> BulletRicochetCallback { get; set; }

	public UnityEvent<CallbackParams> BulletEntryEvent { get; set; } = new UnityEvent<CallbackParams>();


	private Vector2 RandomisedAngle => UnityEngine.Random.insideUnitCircle * Cartridge.PenetrationRandomAngleMultiplier;

	public BallisticsEmitter(MonoBehaviour connectedBehaviour, Cartridge cartridge)
	{
		ConnectedBehaviour = connectedBehaviour;
		Cartridge = cartridge;
		LayersToHit = LayerMask.GetMask("Bounds", "Objects", "CollidingDebris");
		WaterLayer = LayerMask.GetMask("Water");
	}

	public void Emit(Vector2 origin, Vector2 direction)
	{
		LineRenderer tracer = null;
		if (DoTraversal && RenderTracersIfTraversal)
		{
			tracer = CreateTracer(origin, direction);
		}
		ConnectedBehaviour.StartCoroutine(BallisticIteration(origin, direction.normalized, Cartridge.StartSpeed, 0, 0f, null, tracer, shouldDoSplash: true, canExplode: true, Charge));
	}

	private LineRenderer CreateTracer(Vector2 origin, Vector2 direction)
	{
		GameObject gameObject = BulletTracerPool.Instance.Request(origin);
		if (!gameObject)
		{
			return null;
		}
		LineRenderer component = gameObject.GetComponent<LineRenderer>();
		component.widthMultiplier = TracerWidth;
		component.SetPosition(1, Vector3.right * MaxRange);
		tracers.Add(component);
		OnTracerCreation?.Invoke(component);
		return component;
	}

	private bool CanPlayCracks(Vector2 origin, float speed, float distance)
	{
		if (Global.main.Paused)
		{
			return false;
		}
		if (Global.main.camera.orthographicSize > 8f)
		{
			return false;
		}
		if (distance < 5f)
		{
			return false;
		}
		if (Time.time - lastCrackPlayTime < BulletCrackCooldownTime)
		{
			return false;
		}
		if (speed < BulletCrackSpeedThreshold)
		{
			return false;
		}
		if (Vector2.SqrMagnitude(origin - (Vector2)Global.main.camera.transform.position) > 400f)
		{
			return false;
		}
		if (BulletCrackChance < UnityEngine.Random.value)
		{
			lastCrackPlayTime = Time.time;
			return false;
		}
		return true;
	}

	private float GetDamageForDistance(float totalDistance)
	{
		return Cartridge.Damage * DamageMultiplierByDistance(totalDistance);
	}

	private IEnumerator BallisticIteration(Vector2 origin, Vector2 direction, float speed, int iteration = 0, float totalDistance = 0f, Collider2D previousCollider = null, LineRenderer tracer = null, bool shouldDoSplash = true, bool canExplode = false, float charge = 0f)
	{
		if (iteration >= MaxBallisticsIterations || totalDistance >= MaxTotalDistance)
		{
			removeTracer();
			yield break;
		}
		if (LavaBehaviour.IsPointInLava(origin))
		{
			RandomiseCourse(1f, 20f, ref direction, ref speed);
		}
		else if (WaterBehaviour.IsPointUnderWater(origin))
		{
			RandomiseCourse(0.99999f, 10f, ref direction, ref speed);
		}
		float num = Mathf.Clamp(speed / Cartridge.StartSpeed, 0.05f, 1f) * SpeedMultiplierByDistance(totalDistance);
		if (num <= 0.1f)
		{
			removeTracer();
			yield break;
		}
		PhysicalBehaviour physicalBehaviour = null;
		Collider2D collider2D = Physics2D.OverlapPoint(origin, LayersToHit, -0.1f, 0.1f);
		CallbackParams callbackParams;
		if ((bool)collider2D && collider2D.transform.root != ConnectedBehaviour.transform.root && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out physicalBehaviour))
		{
			float num2 = CalculateHardness(physicalBehaviour);
			if (collider2D != previousCollider)
			{
				float damage2 = GetDamageForDistance(ThicknessStepSize + totalDistance) * Mathf.Clamp01(num);
				if ((bool)tracer)
				{
					tracer.enabled = false;
				}
				if ((bool)previousCollider)
				{
					Vector2 vector = origin - direction * ThicknessStepSize;
					previousCollider.SendMessage("ExitShot", new Shot(direction, vector, damage2, triggerExplosiveOverride: true, Cartridge), SendMessageOptions.DontRequireReceiver);
					Action<CallbackParams> bulletExitCallback = BulletExitCallback;
					if (bulletExitCallback != null)
					{
						callbackParams = new CallbackParams
						{
							HitObject = previousCollider,
							Position = vector,
							Direction = direction,
							SurfaceNormal = direction
						};
						bulletExitCallback(callbackParams);
					}
				}
				collider2D.SendMessage("Shot", new Shot(direction * -1f, origin, damage2, triggerExplosiveOverride: true, Cartridge), SendMessageOptions.DontRequireReceiver);
				Action<CallbackParams> bulletEntryCallback = BulletEntryCallback;
				if (bulletEntryCallback != null)
				{
					callbackParams = new CallbackParams
					{
						HitObject = collider2D,
						Position = origin,
						Direction = direction,
						SurfaceNormal = direction
					};
					bulletEntryCallback(callbackParams);
				}
				UnityEvent<CallbackParams> bulletEntryEvent = BulletEntryEvent;
				callbackParams = new CallbackParams
				{
					HitObject = collider2D,
					Position = origin,
					Direction = direction,
					SurfaceNormal = direction
				};
				bulletEntryEvent.Invoke(callbackParams);
				if (canExplode && ExplosiveRound)
				{
					explode(origin);
				}
			}
			physicalBehaviour.rigidbody.AddForceAtPosition(direction * Cartridge.ImpactForce * num2 * ThicknessStepSize * ImpactForceMultiplier * 4f, origin, ForceMode2D.Force);
			ConnectedBehaviour.StartCoroutine(BallisticIteration(origin + direction * ThicknessStepSize, direction + RandomisedAngle, speed - num2, iteration + 1, totalDistance + ThicknessStepSize, collider2D, tracer, shouldDoSplash: true, canExplode, charge));
			yield break;
		}
		if (UserPreferenceManager.Current.BulletCrackWhiz && (bool)Global.main && Global.main.BulletCrackClips != null && Global.main.BulletCrackClips.Length != 0 && CanPlayCracks(origin, speed, totalDistance))
		{
			Global.main.CameraAudioSource.PlayOneShot(Global.main.BulletCrackClips.PickRandom(), UnityEngine.Random.Range(0.05f, 0.3f));
		}
		if (previousCollider != null)
		{
			Vector2 vector2 = origin - direction * ThicknessStepSize;
			previousCollider.SendMessage("ExitShot", new Shot(direction, vector2, GetDamageForDistance(ThicknessStepSize + totalDistance) * Mathf.Clamp01(num), triggerExplosiveOverride: true, Cartridge), SendMessageOptions.DontRequireReceiver);
			Action<CallbackParams> bulletExitCallback2 = BulletExitCallback;
			if (bulletExitCallback2 != null)
			{
				callbackParams = new CallbackParams
				{
					HitObject = previousCollider,
					Position = vector2,
					Direction = direction,
					SurfaceNormal = direction
				};
				bulletExitCallback2(callbackParams);
			}
			if ((bool)tracer)
			{
				tracer.enabled = true;
			}
		}
		RaycastHit2D hit = Physics2D.Raycast(origin, direction, MaxRange, LayersToHit, -0.1f, 0.1f);
		if (!(hit.transform != null) || !(hit.transform.root != ConnectedBehaviour.transform.root) || !Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(hit.transform, out physicalBehaviour))
		{
			if (DoTraversal)
			{
				float traversalDistance = Mathf.Min(MaxRange, Time.deltaTime * 60f * MaxRange * Mathf.Sqrt(num));
				if ((bool)tracer)
				{
					tracer.SetPosition(1, Mathf.Clamp01(num) * MaxRange * Vector3.right);
					tracer.transform.position = origin;
					tracer.transform.right = direction;
				}
				shouldDoSplash = !TestForWaterHit(origin, direction, GetDamageForDistance(totalDistance + traversalDistance * 0.1f) * num, shouldDoSplash);
				direction = Vector2.Lerp(direction, Vector2.down, (1f - Mathf.Pow(0.999f, Time.deltaTime * 60f)) * BulletDropMultiplier * 0.7f).normalized;
				yield return new WaitForEndOfFrame();
				ConnectedBehaviour.StartCoroutine(BallisticIteration(origin + traversalDistance * direction, direction, speed, iteration, totalDistance + traversalDistance * 0.1f, null, tracer, shouldDoSplash, canExplode, charge));
			}
			yield break;
		}
		float hardness = CalculateHardness(physicalBehaviour);
		float value = Mathf.Max(Cartridge.ImpactForce * hardness, Cartridge.ImpactForce);
		float damage = GetDamageForDistance(totalDistance + hit.distance) * Mathf.Clamp01(num);
		value = ImpactForceMultiplier * Mathf.Clamp(value, 0f, 1.5f) * 1.5f;
		physicalBehaviour.rigidbody.AddForceAtPosition(value * direction, origin, ForceMode2D.Force);
		physicalBehaviour.SendMessage("Shot", new Shot(hit.normal, hit.point, damage, triggerExplosiveOverride: true, Cartridge), SendMessageOptions.DontRequireReceiver);
		physicalBehaviour.Charge = Mathf.Max(Charge * physicalBehaviour.EnergyWireResistance, physicalBehaviour.Charge);
		Action<CallbackParams> bulletEntryCallback2 = BulletEntryCallback;
		if (bulletEntryCallback2 != null)
		{
			callbackParams = new CallbackParams
			{
				HitObject = hit.collider,
				Position = hit.point,
				Direction = direction,
				SurfaceNormal = hit.normal
			};
			bulletEntryCallback2(callbackParams);
		}
		UnityEvent<CallbackParams> bulletEntryEvent2 = BulletEntryEvent;
		callbackParams = new CallbackParams
		{
			HitObject = hit.collider,
			Position = hit.point,
			Direction = direction,
			SurfaceNormal = hit.normal
		};
		bulletEntryEvent2.Invoke(callbackParams);
		if (canExplode && ExplosiveRound)
		{
			explode(hit.point);
		}
		if ((bool)tracer)
		{
			tracer.transform.position = origin;
			tracer.transform.right = direction;
			tracer.SetPosition(1, Vector3.right * hit.distance);
		}
		yield return new WaitForSeconds(0.01f);
		shouldDoSplash = !TestForWaterHit(origin, direction, damage, shouldDoSplash);
		if (ShouldRicochet(direction, hit, physicalBehaviour))
		{
			RicochetBullet(direction, speed, iteration, totalDistance, charge, tracer, hit);
		}
		else if (!physicalBehaviour.BulletPenetration)
		{
			removeTracer();
		}
		else
		{
			ConnectedBehaviour.StartCoroutine(BallisticIteration(hit.point + direction * ThicknessStepSize, direction, speed - hardness, iteration + 1, totalDistance + hit.distance, hit.collider, tracer, shouldDoSplash, canExplode, charge));
		}
		void explode(Vector2 point)
		{
			ExplosiveRoundParams.Position = (ExplodeAtHitPoint ? point : (point + direction * 0.5f));
			ExplosionCreator.Explode(ExplosiveRoundParams);
			canExplode = false;
		}
		void removeTracer()
		{
			if ((bool)tracer)
			{
				tracers.Remove(tracer);
				BulletTracerPool.Instance.Return(tracer.gameObject);
			}
		}
	}

	private static void RandomiseCourse(float dampening, float diversion, ref Vector2 direction, ref float speed)
	{
		float deltaTime = Time.deltaTime;
		direction = (direction + diversion * deltaTime * UnityEngine.Random.insideUnitCircle).normalized;
		speed = Mathf.Lerp(speed, 0f, Utils.GetLerpFactorDeltaTime(dampening, deltaTime));
	}

	private void RicochetBullet(Vector2 direction, float speed, int iteration, float totalDistance, float charge, LineRenderer tracer, RaycastHit2D hit)
	{
		BulletRicochetCallback?.Invoke(new CallbackParams
		{
			HitObject = hit.collider,
			Position = hit.point,
			Direction = direction,
			SurfaceNormal = hit.normal
		});
		Vector2 direction2 = Vector2.Reflect(direction, hit.normal) + RandomisedAngle * 0.1f;
		ConnectedBehaviour.StartCoroutine(BallisticIteration(hit.point - hit.normal * 0.02f, direction2, speed, iteration + 1, totalDistance + hit.distance, hit.collider, tracer, shouldDoSplash: true, canExplode: false, charge));
	}

	private bool TestForWaterHit(Vector2 origin, Vector2 direction, float damage, bool sendMessage)
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, direction, MaxRange, WaterLayer);
		if ((bool)raycastHit2D && raycastHit2D.distance > 0.5f && raycastHit2D.transform.gameObject.layer == 4)
		{
			if (sendMessage)
			{
				raycastHit2D.transform.SendMessage("Shot", new Shot(raycastHit2D.normal, raycastHit2D.point, damage, triggerExplosiveOverride: true, Cartridge), SendMessageOptions.DontRequireReceiver);
			}
			return true;
		}
		return false;
	}

	private float CalculateHardness(PhysicalBehaviour phys)
	{
		float num = 1f - Mathf.Clamp01(phys.Properties.Softness + phys.Properties.Brittleness);
		return 2f * MaterialHardnessMultiplier * (4.5f * num) + phys.Properties.BulletSpeedAbsorptionPower;
	}

	private float SpeedMultiplierByDistance(float d)
	{
		return 1f / Mathf.Max(0.1f, Mathf.Sqrt(d * 0.2f));
	}

	private float DamageMultiplierByDistance(float d)
	{
		return Mathf.Clamp(1.5f / d + 0.7f / Mathf.Lerp(d, 0.8f, 0.992f), 0.5f, 10f);
	}

	private bool ShouldRicochet(Vector2 incoming, RaycastHit2D hit, PhysicalBehaviour phys)
	{
		if (!hit.transform)
		{
			return false;
		}
		float num = Mathf.Abs(Vector2.Dot(hit.normal, incoming));
		float num2 = Mathf.Clamp01(phys.Properties.Softness + phys.Properties.Brittleness);
		float num3 = 1f - (num2 * 150f + num);
		if (num3 > 0.05f)
		{
			return num3 * RicochetChanceMultiplier > UnityEngine.Random.value;
		}
		return false;
	}

	public void Dispose()
	{
		BulletEntryCallback = null;
		BulletExitCallback = null;
		BulletRicochetCallback = null;
		ConnectedBehaviour = null;
		Cartridge = null;
		foreach (LineRenderer tracer in tracers)
		{
			if ((bool)tracer && tracer.gameObject.activeSelf)
			{
				BulletTracerPool.Instance.Return(tracer.gameObject);
			}
		}
		tracers = null;
	}
}

using System.Collections;
using UnityEngine;

public class ArchelixCasterBoltBehaviour : BaseBoltBehaviour, Messages.IOnPoolableInitialised
{
	public float Phase;

	public float Frequency = 5f;

	public float Amplitude = 1f;

	[Space]
	public float AoeRadius = 10f;

	public float HomingForce = 1f;

	[Space]
	public float MaxTimeUntilDeath;

	public ParticleSystem ParticleSystem;

	public TrailRenderer TrailRendererToDetach;

	public float HomingTemperatureThreshold = 30f;

	[Space]
	[Space]
	public GameObject ImpactPoolable;

	private static readonly Collider2D[] buffer = new Collider2D[32];

	public Vector2 Direction;

	private int frame;

	private const int surroundingCheckInterval = 4;

	private float timeAlive;

	private ObjectPoolBehaviour pool;

	private bool isDespawning;

	private void Start()
	{
		frame = Random.Range(0, 10000);
		Direction = base.transform.right;
	}

	protected override void Update()
	{
		if (!isDespawning)
		{
			timeAlive += Time.deltaTime;
			float num = Mathf.Sin(Time.time * Frequency + Phase) * Amplitude;
			base.transform.right = Direction + Vector2.Perpendicular(Direction) * num;
			float num2 = Speed * Time.deltaTime;
			if (DoHitCheck(num2) || timeAlive > MaxTimeUntilDeath)
			{
				StartCoroutine(DespawnRoutine());
			}
			else
			{
				base.transform.position += base.transform.right * num2;
			}
		}
	}

	private IEnumerator DespawnRoutine()
	{
		isDespawning = true;
		ParticleSystem.Stop();
		yield return new WaitUntil(() => TrailRendererToDetach.positionCount == 0);
		pool.Return(base.gameObject);
	}

	private void FixedUpdate()
	{
		if (isDespawning)
		{
			return;
		}
		frame++;
		if (frame % 4 != 0)
		{
			return;
		}
		Vector3 position = base.transform.position;
		if (WaterBehaviour.IsPointUnderWater(position))
		{
			Direction += 6f * Time.deltaTime * (Vector2)Utils.GetPerlin2Mapped(position.x * 23.324f, position.y * 23.324f);
		}
		Vector2 direction = Direction;
		foreach (PhysicalBehaviour item in Global.main.GetPhysicsObjectsNearPositionAccurate(position, AoeRadius))
		{
			if ((bool)item && !(item.Temperature < PhysicalBehaviour.AmbientTemperature + HomingTemperatureThreshold))
			{
				Vector2 vector = item.GetClosestPointTo(position) - (Vector2)position;
				float sqrMagnitude = vector.sqrMagnitude;
				float num = Mathf.Sqrt(sqrMagnitude);
				Vector2 vector2 = vector / num;
				if (!(num < 1f) && !(Vector2.Dot(vector2, direction) < -0.7f))
				{
					vector2 /= sqrMagnitude;
					Direction += HomingForce * Mathf.Clamp01(item.Temperature) * Mathf.Clamp01(timeAlive * 1.5f) * sqrMagnitude * vector2;
				}
			}
		}
	}

	public override bool ShouldReflect(RaycastHit2D hit)
	{
		return false;
	}

	protected override void OnHit(RaycastHit2D hit)
	{
		if (!hit.collider)
		{
			return;
		}
		ExplosionCreator.CreatePulseExplosion(hit.point, 1f, 1f, soundAndEffects: false);
		PoolGenerator.Instance.RequestPrefab(ImpactPoolable, hit.point);
		hit.collider.SendMessage("Slice", SendMessageOptions.DontRequireReceiver);
		if (hit.collider.gameObject.TryGetComponent<LimbBehaviour>(out var component))
		{
			if (Random.value > 0.5f)
			{
				component.PhysicalBehaviour.Temperature = -273f;
				component.InternalTemperature = -273f;
			}
			component.CirculationBehaviour.Cut((Vector2)component.transform.position + component.PhysicalBehaviour.ObjectArea * Random.insideUnitCircle, Random.insideUnitCircle);
		}
	}

	public void OnPoolableInitialised(ObjectPoolBehaviour pool)
	{
		timeAlive = 0f;
		TrailRendererToDetach.Clear();
		this.pool = pool;
		Direction = base.transform.right;
		frame = Random.Range(0, 10000);
		isDespawning = false;
		ParticleSystem.Play();
	}
}

using UnityEngine;

public class LaunchedRocketBehaviour : MonoBehaviour
{
	public float UnitsPerSecond = 1f;

	public float AccelerationPerSecond = 0.5f;

	public float MaxSpeed = 80f;

	private float Seed;

	public ParticleSystem relevantSystem;

	public float WaterSlowdown = 0.8f;

	public float ImmobilityFieldSlowdown = 0.8f;

	private LayerMask mask;

	private float f;

	public LayerMask ImmobiltyFieldLayer;

	private float speedMultiplier = 1f;

	public float WobbleIntensity = 1f;

	public ExplosionCreator.ExplosionParameters Explosion = new ExplosionCreator.ExplosionParameters(24u, Vector2.zero, 24f, 4f, createFx: true, big: true, 0.2f);

	private void Awake()
	{
		Seed = Random.value * 1000f;
		mask = LayerMask.GetMask("Objects", "Bounds");
	}

	private void FixedUpdate()
	{
		if ((bool)Physics2D.OverlapPoint(base.transform.position, ImmobiltyFieldLayer))
		{
			speedMultiplier *= ImmobilityFieldSlowdown;
		}
		else
		{
			speedMultiplier = 1f;
		}
	}

	private void Update()
	{
		Vector3 right = base.transform.right;
		if (WobbleIntensity > float.Epsilon)
		{
			right += 0.25f * WobbleIntensity * new Vector3(Mathf.PerlinNoise(0f, 2f * Time.time + Seed) * 2f - 1f, Mathf.PerlinNoise(2f * Time.time - Seed, 5f) * 2f - 1f, 0f);
			right.Normalize();
		}
		float num = (WaterBehaviour.IsPointUnderWater(base.transform.position) ? WaterSlowdown : 1f) * speedMultiplier;
		float num2 = Mathf.Min(MaxSpeed, (UnitsPerSecond + AccelerationPerSecond * f) * num) * Time.deltaTime;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, right, num2, mask);
		if ((bool)raycastHit2D)
		{
			Vector2 vector = raycastHit2D.point + raycastHit2D.normal * 0.05f;
			relevantSystem.transform.SetParent(null);
			relevantSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			relevantSystem.gameObject.AddComponent<DeleteAfterTime>().Life = 5f;
			Object.Destroy(base.gameObject);
			Explosion.Position = vector;
			ExplosionCreator.Explode(Explosion);
			raycastHit2D.transform.SendMessage("Shot", new Shot(raycastHit2D.normal, raycastHit2D.point, 35f), SendMessageOptions.DontRequireReceiver);
			raycastHit2D.transform.SendMessage("ExitShot", new Shot(raycastHit2D.normal, raycastHit2D.point, 35f), SendMessageOptions.DontRequireReceiver);
			raycastHit2D.transform.SendMessage("Break", num2 * (Vector2)right, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			base.transform.position += right * num2;
		}
		f += Time.deltaTime;
	}
}

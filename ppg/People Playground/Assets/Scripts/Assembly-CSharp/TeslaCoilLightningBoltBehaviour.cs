using UnityEngine;

public class TeslaCoilLightningBoltBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public GameObject Spark;

	[SkipSerialisation]
	public Material LightningMaterial;

	[SkipSerialisation]
	public LineRenderer lineRenderer;

	public float seed;

	public float life;

	public LayerMask layer;

	[SkipSerialisation]
	public AnimationCurve curve;

	public float Radius = 6f;

	private PhysicalBehaviour target;

	private float t;

	private Vector2 randomOffset;

	[SkipSerialisation]
	public TeslaCoilBehaviour parent;

	private static readonly Collider2D[] overlapBuffer = new Collider2D[32];

	private readonly Vector3[] positions = new Vector3[64];

	private Collider2D[] colliderArray;

	private void Awake()
	{
		lineRenderer = base.gameObject.GetComponent<LineRenderer>();
		lineRenderer.useWorldSpace = true;
		lineRenderer.sharedMaterial = LightningMaterial;
		lineRenderer.positionCount = positions.Length;
	}

	private void Init()
	{
		FindNewTarget();
		seed = Random.Range(-9000f, 9000f);
		life = (((double)Random.value > 0.95) ? Random.Range(0.1f, 0.3f) : Random.Range(0.02f, 0.08f));
		lineRenderer.widthMultiplier = Random.Range(1f, 2f);
		randomOffset = Random.insideUnitCircle * 4f;
	}

	private void FindNewTarget()
	{
		target = null;
		int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, Radius, overlapBuffer, layer);
		if (num == 0)
		{
			return;
		}
		float num2 = 10000000f;
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = overlapBuffer[i];
			if ((bool)collider2D && !(collider2D.transform.root == base.transform.root) && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value) && value.Properties.Conducting)
			{
				float sqrMagnitude = (collider2D.transform.position - (base.transform.position + Random.insideUnitSphere)).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					target = value;
					num2 = sqrMagnitude;
				}
			}
		}
		if ((bool)target)
		{
			colliderArray = target.GetComponents<Collider2D>();
		}
	}

	private void Update()
	{
		t += Time.smoothDeltaTime;
		if (t > life)
		{
			t = 0f;
			Init();
		}
		float num = t;
		Vector3 position = base.transform.position;
		Vector3 vector;
		if ((bool)target)
		{
			vector = target.transform.position;
			float num2 = float.MaxValue;
			Vector3 vector2 = base.transform.position + (Vector3)randomOffset;
			Collider2D[] array = colliderArray;
			foreach (Collider2D collider2D in array)
			{
				if ((bool)collider2D)
				{
					Vector3 vector3 = collider2D.ClosestPoint(base.transform.position);
					float sqrMagnitude = (vector3 - vector2).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
						vector = vector3;
					}
				}
			}
			target.Charge += Time.deltaTime * 4f;
			target.rigidbody.AddForce(Random.insideUnitCircle * 0.01f, ForceMode2D.Impulse);
			if (SmoothRandom(10f).x > 0.99f)
			{
				if ((bool)parent)
				{
					parent.PlaySparkSound();
				}
				Object.Instantiate(Spark, vector, Quaternion.identity);
			}
		}
		else
		{
			vector = Random.insideUnitCircle * Radius + (Vector2)position;
		}
		for (int j = 0; j < lineRenderer.positionCount; j++)
		{
			float time = (float)j / (float)(lineRenderer.positionCount - 1);
			Vector3 vector4 = Vector3.Lerp(position, vector, time);
			Vector2 vector5 = Utils.GetPerlin2Mapped(vector4.x * 3.354f - seed, vector4.y * 3.354f + seed) * 0.5f;
			Vector2 vector6 = curve.Evaluate(time) * (Vector2)Utils.GetPerlin2Mapped(vector4.x * 0.3f - seed, vector4.y * 0.3f + seed) * 1.4f + Physics2D.gravity * num * -0.1f;
			Vector3 vector7 = 5.25f * num * (Vector3)vector5 + (Vector3)vector6 * 1.3f;
			positions[j] = vector4 + vector7 + (Vector3)SmoothRandom(5f) * 0.02f;
		}
		lineRenderer.SetPositions(positions);
	}

	private Vector2 SmoothRandom(float frq = 1f)
	{
		float num = frq * Time.time;
		return new Vector2(Mathf.PerlinNoise(num, seed) * 2f - 1f, Mathf.PerlinNoise(seed, num) * 2f - 1f);
	}
}

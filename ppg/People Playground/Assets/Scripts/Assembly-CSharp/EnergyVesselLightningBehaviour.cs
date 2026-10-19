using System.Collections;
using UnityEngine;

public class EnergyVesselLightningBehaviour : MonoBehaviour
{
	public GameObject Spark;

	public Material LightningMaterial;

	public LineRenderer lineRenderer;

	public float seed;

	public float life;

	public LayerMask layer;

	public AnimationCurve curve;

	public float Radius = 190f;

	private float t;

	private Vector2 targetDirection = Vector2.zero;

	private bool hasExploded;

	private void Awake()
	{
		lineRenderer = base.gameObject.GetComponent<LineRenderer>();
		lineRenderer.useWorldSpace = true;
		lineRenderer.sharedMaterial = LightningMaterial;
	}

	private IEnumerator Init()
	{
		lineRenderer.enabled = false;
		yield return new WaitForSeconds(Random.Range(0.1f, 1f));
		lineRenderer.enabled = true;
		seed = Random.Range(-9000f, 9000f);
		targetDirection = Random.onUnitSphere;
		life = (((double)Random.value > 0.95) ? Random.Range(0.1f, 0.3f) : Random.Range(0.02f, 0.08f));
		lineRenderer.widthMultiplier = Random.Range(1f, 2f);
	}

	private void Update()
	{
		if (!lineRenderer.enabled)
		{
			return;
		}
		t += Time.smoothDeltaTime;
		if (t > life)
		{
			t = 0f;
			hasExploded = false;
			StartCoroutine(Init());
		}
		float num = t;
		Vector3 position = base.transform.position;
		Vector3 vector = Vector3.zero;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, targetDirection);
		if ((bool)raycastHit2D)
		{
			vector = raycastHit2D.point;
			if (!hasExploded)
			{
				ExplosionCreator.Explode(new ExplosionCreator.ExplosionParameters(16u, vector, 15f, 5f, createFx: false, big: true));
				Object.Instantiate(Spark, vector, Quaternion.identity);
				hasExploded = true;
			}
		}
		else
		{
			t = life;
		}
		lineRenderer.positionCount = 3 * Mathf.Clamp((int)Vector3.Distance(position, vector) * 3, 5, 64);
		Vector3[] array = new Vector3[lineRenderer.positionCount];
		for (int i = 0; i < lineRenderer.positionCount; i++)
		{
			float time = (float)i / (float)(lineRenderer.positionCount - 1);
			Vector3 vector2 = Vector3.Lerp(position, vector, time);
			Vector2 vector3 = new Vector2(Mathf.PerlinNoise(vector2.x * 0.2f - seed, vector2.y * 0.2f + seed) * 2f - 1f, Mathf.PerlinNoise(vector2.x * 0.2f + seed, vector2.y * 0.2f - seed));
			Vector2 vector4 = curve.Evaluate(time) * new Vector2(Mathf.PerlinNoise(vector2.x * 0.03f - seed, vector2.y * 0.03f + seed) * 2f - 1f, Mathf.PerlinNoise(vector2.x * 0.03f + seed, vector2.y * 0.03f - seed) * 2f - 1f);
			Vector3 vector5 = 36.25f * num * (Vector3)vector3 + (Vector3)vector4 * 21.3f;
			array[i] = vector2 + vector5 + (Vector3)SmoothRandom(12f, vector2) * 2f;
		}
		lineRenderer.SetPositions(array);
	}

	private Vector2 SmoothRandom(float frq = 1f, Vector2 pos = default(Vector2))
	{
		float num = frq * Time.time;
		return new Vector2(Mathf.PerlinNoise(num, seed + pos.x) * 2f - 1f, Mathf.PerlinNoise(seed, num + pos.y) * 2f - 1f);
	}
}

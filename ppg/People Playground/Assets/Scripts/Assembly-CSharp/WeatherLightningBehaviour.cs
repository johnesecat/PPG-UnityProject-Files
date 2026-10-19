using System.Collections;
using UnityEngine;

public class WeatherLightningBehaviour : MonoBehaviour
{
	public float Width;

	public float Interval = 0.1f;

	public float Chance;

	public AudioClip[] Thunder;

	public AudioSource AudioSource;

	public SpriteRenderer LightSprite;

	private LayerMask mask;

	private Vector3[] vertices;

	public LineRenderer LineRenderer;

	private float t;

	private void Awake()
	{
		mask = LayerMask.GetMask("Objects", "Bounds");
		vertices = new Vector3[LineRenderer.positionCount];
		LightSprite.enabled = false;
	}

	private void Update()
	{
		if (Mathf.Approximately(Chance, 0f))
		{
			return;
		}
		t += Time.deltaTime;
		if (t > Interval)
		{
			t = 0f;
			if (Random.value < Chance / 100f)
			{
				StartCoroutine(FireBolt());
			}
		}
	}

	private IEnumerator FireBolt()
	{
		Vector3 vector = base.transform.position + Random.value * Width * Vector3.right;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector3.down, 10000f, mask);
		if ((bool)raycastHit2D)
		{
			float x = Random.value * 10000f;
			float num = raycastHit2D.distance * 0.2f;
			LightSprite.transform.position = vector;
			AudioSource.PlayOneShot(Thunder.PickRandom());
			int num2 = vertices.Length;
			for (int j = 0; j < num2; j++)
			{
				float num3 = (float)j / (float)num2;
				float num4 = 1f - Mathf.Abs(2f * num3 - 1f);
				Vector3 vector2 = (Utils.GetPerlin2Mapped(x, num3 / num * 19f) + (Vector3)Random.insideUnitCircle * 0.2f) * num4;
				vertices[j] = Vector3.Lerp(vector, raycastHit2D.point, num3) + num * vector2;
			}
			LineRenderer.SetPositions(vertices);
			ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(raycastHit2D.point), new ExplosionCreator.ExplosionParameters(16u, raycastHit2D.point, 35f, 12f, createFx: true, big: true));
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
			{
				value.Charge += 1500f;
			}
			CameraShakeBehaviour.main.Shake(150f, raycastHit2D.point, 0.1f);
			for (int i = 0; i < Random.Range(2, 5); i++)
			{
				LineRenderer.enabled = true;
				LightSprite.enabled = true;
				yield return new WaitForSeconds(0.032f);
				LineRenderer.enabled = false;
				LightSprite.enabled = false;
				yield return new WaitForSeconds(Random.Range(0.032f, 0.3f));
			}
		}
	}
}

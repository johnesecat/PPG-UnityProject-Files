using System.Collections;
using UnityEngine;

public class LightningToolEntityBehaviour : MonoBehaviour
{
	public ParticleSystem ParticleSystem;

	public AudioSource AudioSource;

	public AudioSource LoopingAudio;

	public LineRenderer SingleBolt;

	public SpriteRenderer LargeFlashLight;

	public float BoltThreshold = 2f;

	public float CooldownTime = 0.5f;

	public float Radius = 2f;

	[Space]
	public AudioClip Ignite;

	public AudioClip Release;

	public AudioClip[] Thunder;

	private Vector3 delta;

	private Vector3 oldPos;

	private float cooldown;

	private Vector3[] vertices;

	private LayerMask mask;

	private bool isHeld;

	private Collider2D[] buffer;

	private void Awake()
	{
		mask = LayerMask.GetMask("Objects", "Bounds");
		vertices = new Vector3[SingleBolt.positionCount];
		buffer = new Collider2D[64];
		LargeFlashLight.enabled = false;
	}

	public void StartHold()
	{
		if (base.enabled)
		{
			SingleBolt.enabled = false;
			LargeFlashLight.enabled = false;
			AudioSource.PlayOneShot(Ignite);
			LoopingAudio.Play();
			ParticleSystem.Play();
			isHeld = true;
			oldPos = Input.mousePosition;
		}
	}

	private void Update()
	{
		if (!isHeld)
		{
			return;
		}
		delta = (Input.mousePosition - oldPos) * (Time.unscaledDeltaTime * 19f);
		oldPos = Input.mousePosition;
		for (int i = 0; i < Physics2D.OverlapCircleNonAlloc(Global.main.MousePosition, Radius, buffer); i++)
		{
			Collider2D collider2D = buffer[i];
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				value.Charge += 2f * Time.deltaTime;
			}
		}
		if (cooldown > 0f)
		{
			cooldown -= Time.unscaledDeltaTime;
		}
		else if (delta.sqrMagnitude > BoltThreshold)
		{
			Debug.DrawLine(base.transform.position, base.transform.position + delta, Color.red, 2f);
			StartCoroutine(FireBolt());
		}
	}

	private IEnumerator FireBolt()
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, delta, 5000f, mask);
		if ((bool)raycastHit2D)
		{
			float x = Random.value * 10000f;
			float num = raycastHit2D.distance * 0.2f;
			cooldown = CooldownTime;
			AudioSource.PlayOneShot(Thunder.PickRandom());
			int num2 = vertices.Length;
			for (int j = 0; j < num2; j++)
			{
				float num3 = (float)j / (float)num2;
				float num4 = 1f - Mathf.Abs(2f * num3 - 1f);
				Vector3 vector = (Utils.GetPerlin2Mapped(x, num3 / num * 19f) + (Vector3)Random.insideUnitCircle * 0.2f) * num4;
				vertices[j] = Vector3.Lerp(base.transform.position, raycastHit2D.point, num3) + num * vector;
			}
			SingleBolt.SetPositions(vertices);
			ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(raycastHit2D.point), new ExplosionCreator.ExplosionParameters(8u, raycastHit2D.point, 15f, 8f, createFx: true, big: true));
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
			{
				value.Charge += 150f;
			}
			for (int i = 0; i < Random.Range(2, 5); i++)
			{
				SingleBolt.enabled = true;
				LargeFlashLight.enabled = true;
				yield return new WaitForSeconds(0.032f);
				SingleBolt.enabled = false;
				LargeFlashLight.enabled = false;
				yield return new WaitForSeconds(Random.Range(0.032f, 0.3f));
			}
		}
	}

	public void StopHold()
	{
		if (base.enabled)
		{
			SingleBolt.enabled = false;
			LargeFlashLight.enabled = false;
			LoopingAudio.Stop();
			if (isHeld)
			{
				AudioSource.PlayOneShot(Release);
			}
			ParticleSystem.Stop();
			isHeld = false;
		}
	}
}

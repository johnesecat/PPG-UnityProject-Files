using System.Collections;
using UnityEngine;

public class BeamformerBehaviour : MonoBehaviour, Messages.IUse
{
	private float temperature;

	private SpriteRenderer renderer;

	private MaterialPropertyBlock propertyBlock;

	[SkipSerialisation]
	public AudioSource AudioSource;

	public Vector3 barrelPosition;

	public Vector3 barrelDirection;

	public float TemperatureCooldown = 1f;

	[SkipSerialisation]
	public ParticleSystem MuzzleFlash;

	[SkipSerialisation]
	public LineRenderer AimLineRenderer;

	[SkipSerialisation]
	public LineRenderer BeamLineRenderer;

	public LayerMask LayersToHit;

	[SkipSerialisation]
	public Rigidbody2D rigidBody;

	public float BeamRange = 1100f;

	public float ExplosionForce = 1f;

	public float DirectionalForce = 1f;

	public float RecoilForce;

	public float BeamWidth = 0.05f;

	private Vector3[] positions = new Vector3[3];

	private static readonly RaycastHit2D[] buffer = new RaycastHit2D[1024];

	private PhysicalBehaviour phys;

	public Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		rigidBody = GetComponent<Rigidbody2D>();
		renderer = GetComponent<SpriteRenderer>();
		propertyBlock = new MaterialPropertyBlock();
		renderer.GetPropertyBlock(propertyBlock);
	}

	private void Start()
	{
		BeamLineRenderer.positionCount = positions.Length;
		BeamLineRenderer.useWorldSpace = true;
		BeamLineRenderer.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (temperature > 0f)
		{
			temperature -= Time.deltaTime;
		}
	}

	private void OnWillRenderObject()
	{
		propertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), 1f - temperature / TemperatureCooldown);
		renderer.SetPropertyBlock(propertyBlock);
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Shoot();
		}
	}

	private void OnEnable()
	{
		AimLineRenderer.enabled = true;
	}

	private void OnDisable()
	{
		temperature = 0f;
		AimLineRenderer.enabled = false;
		OnWillRenderObject();
	}

	public void Shoot()
	{
		if (!(temperature > 0.05f))
		{
			temperature += TemperatureCooldown;
			AudioSource.PlayOneShot(AudioSource.clip);
			MuzzleFlash.Play();
			CameraShakeBehaviour.main.Shake(5f, BarrelPosition);
			rigidBody.AddForce(-1f * RecoilForce * BarrelDirection, ForceMode2D.Impulse);
			StartCoroutine(BeamRenderRoutine());
			phys.Temperature += 10f;
		}
	}

	private IEnumerator BeamRenderRoutine()
	{
		Vector2 vector = BarrelPosition;
		Vector2 vector2 = BarrelDirection;
		positions[0] = vector;
		positions[1] = vector + vector2 * BeamRange / 2f;
		positions[2] = vector + vector2 * BeamRange;
		BeamLineRenderer.SetPositions(positions);
		BeamLineRenderer.gameObject.SetActive(value: true);
		DoLaserDamage(vector, vector2);
		yield return new WaitForSeconds(0.02f);
		BeamLineRenderer.gameObject.SetActive(value: false);
	}

	private void DoLaserDamage(Vector2 position, Vector2 forward)
	{
		for (int i = 0; i < Physics2D.CircleCastNonAlloc(position + forward * (0.02f + BeamWidth), BeamWidth, forward, buffer, (int)LayersToHit); i++)
		{
			RaycastHit2D raycastHit2D = buffer[i];
			if (raycastHit2D.transform.root == base.transform.root || !Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
			{
				continue;
			}
			value.Ignite(Random.value > 0.25f, raycastHit2D.point);
			value.Temperature += 100f;
			value.SendMessage("Break", forward * DirectionalForce, SendMessageOptions.DontRequireReceiver);
			if (raycastHit2D.transform.TryGetComponent<LimbBehaviour>(out var component))
			{
				if (component.HasJoint)
				{
					component.Slice();
				}
				else
				{
					component.Crush();
				}
			}
			else
			{
				value.SendMessage("Slice", SendMessageOptions.DontRequireReceiver);
			}
			ExplosionCreator.Explode(new ExplosionCreator.ExplosionParameters(5u, raycastHit2D.point, 2f, ExplosionForce, createFx: true));
			value.rigidbody.AddForceAtPosition(DirectionalForce * value.rigidbody.mass * forward, raycastHit2D.point, ForceMode2D.Impulse);
		}
	}
}

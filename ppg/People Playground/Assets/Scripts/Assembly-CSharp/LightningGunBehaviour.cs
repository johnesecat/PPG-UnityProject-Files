using UnityEngine;

public class LightningGunBehaviour : MonoBehaviour
{
	public float Range;

	public float Radius;

	public float ChargeOnHit = 5f;

	public float PushForce = 1f;

	public float VibrateIntensity = 1f;

	public float CameraShakeIntensity = 0.01f;

	public float IgnitionChance = 0.01f;

	public int PriorityLayer;

	public LayerMask Layers;

	[SkipSerialisation]
	public ParticleSystem ParticleSystem;

	[SkipSerialisation]
	public Transform TargetPosition;

	[SkipSerialisation]
	public AudioClip Start;

	[SkipSerialisation]
	public AudioClip End;

	[SkipSerialisation]
	public PhysicalBehaviour Phys;

	[SkipSerialisation]
	public AudioSource LoopSource;

	private float heldTime;

	private static readonly Collider2D[] buffer = new Collider2D[64];

	private Vector3 RayOrigin => ParticleSystem.transform.position;

	private Vector3 Direction => base.transform.localToWorldMatrix * new Vector3(1f, 0f, 0f);

	private void Update()
	{
		if (Phys.StartedBeingUsedContinuously())
		{
			heldTime = 0f;
			ParticleSystem.Play();
			LoopSource.Play();
			LoopSource.PlayOneShot(Start);
		}
		if (Phys.IsBeingUsedContinuously())
		{
			heldTime += Time.deltaTime;
		}
		if (Phys.StoppedBeingUsedContinuously())
		{
			ParticleSystem.Stop();
			LoopSource.Stop();
			LoopSource.PlayOneShot(End);
		}
	}

	private void FixedUpdate()
	{
		if (Phys.IsBeingUsedContinuously())
		{
			CameraShakeBehaviour.main.Shake(CameraShakeIntensity + Mathf.Min(heldTime * 0.1f, 0.4f), base.transform.position);
			Phys.rigidbody.AddForce(Random.insideUnitCircle * VibrateIntensity);
			PerformLightning();
		}
	}

	private void OnDisable()
	{
		heldTime = 0f;
		ParticleSystem.Stop();
		if (LoopSource.isPlaying)
		{
			LoopSource.Stop();
			LoopSource.PlayOneShot(End);
		}
	}

	private void PerformLightning()
	{
		Vector3 direction = Direction;
		Vector3 rayOrigin = RayOrigin;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(RayOrigin, Direction, Range, Layers);
		Vector3 vector = ((!raycastHit2D.transform) ? (rayOrigin + direction * Range) : ((Vector3)raycastHit2D.point));
		Vector3 position = vector;
		int num = Physics2D.OverlapCircleNonAlloc(vector, Radius, buffer, Layers);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if (collider2D.transform.root == base.transform.root || PriorityLayer != collider2D.transform.root.gameObject.layer)
			{
				continue;
			}
			Vector3 vector2 = collider2D.transform.position - rayOrigin;
			RaycastHit2D raycastHit2D2 = Physics2D.Raycast(rayOrigin, vector2);
			Debug.DrawRay(raycastHit2D2.point, raycastHit2D2.normal);
			if (raycastHit2D2.transform.root == collider2D.transform.root && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				if ((value.Properties.Conducting && Random.value > 0.5f) || Random.value < value.Properties.MagneticAttractionIntensity * 0.05f)
				{
					position = raycastHit2D2.point;
					DoDamageTo(value, raycastHit2D2.point, vector2);
				}
				else if (Random.value < 0.05f)
				{
					DoDamageTo(value, raycastHit2D2.point, vector2);
				}
			}
		}
		TargetPosition.position = position;
	}

	private void DoDamageTo(PhysicalBehaviour phys, Vector2 point, Vector2 dir)
	{
		phys.Charge += ChargeOnHit * Mathf.Min(heldTime * 2f, 200f);
		phys.rigidbody.AddForce(dir.normalized * PushForce);
		if (Random.value < IgnitionChance + heldTime / 10f)
		{
			phys.Ignite(point);
		}
	}
}

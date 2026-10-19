using System.Collections;
using UnityEngine;

public class JavelinMissileBehaviour : MonoBehaviour, Messages.IOnEMPHit, Messages.IShot
{
	private enum MissileStage
	{
		VerticalTakeOff,
		TargetedStrike
	}

	public Rigidbody2D Rigidbody;

	public Vector2 LocalDirection = Vector2.right;

	public AudioSource AudioSource;

	public AudioClip TakeOffSound;

	public Transform Fins;

	public AnimationCurve FinExtendCurve;

	public Vector3 StaticTargetPoint;

	public PhysicalBehaviour Target;

	public ParticleSystem TrailParticleEffect;

	public float TakeOffDelay = 1f;

	public float ForceIntensity = 1f;

	public float ArcHeight = 20f;

	public float VerticalLiftoffRotationStrength = 0.5f;

	public float AimingStrength = 1f;

	private MissileStage Stage;

	private bool takenOff;

	private bool exploded;

	private float creationTime;

	public GameObject Glow;

	public GameObject ExplosionEffect;

	private bool broken;

	public GameObject SparkPrefab;

	private Vector2 TargetPosition => Target ? Target.transform.position : StaticTargetPoint;

	private void Start()
	{
		Stage = MissileStage.VerticalTakeOff;
		creationTime = Time.time;
		TrailParticleEffect.gameObject.SetActive(value: false);
		StartCoroutine(TakeOff());
	}

	private IEnumerator TakeOff()
	{
		yield return new WaitForSeconds(TakeOffDelay);
		float startY = base.transform.position.y;
		TrailParticleEffect.gameObject.SetActive(value: true);
		takenOff = true;
		AudioSource.PlayOneShot(TakeOffSound);
		AudioSource.Play();
		CameraShakeBehaviour.main.Shake(6f, base.transform.position);
		Stage = MissileStage.VerticalTakeOff;
		Rigidbody.gravityScale = 0f;
		yield return new WaitUntil(() => base.transform.position.y > startY + ArcHeight);
		Stage = MissileStage.TargetedStrike;
	}

	private void Update()
	{
		Fins.localScale = new Vector3(1f, FinExtendCurve.Evaluate(Time.time - creationTime), 1f);
	}

	private void FixedUpdate()
	{
		if (!broken && takenOff)
		{
			switch (Stage)
			{
			case MissileStage.VerticalTakeOff:
				Rigidbody.AddTorque(VerticalLiftoffRotationStrength * Mathf.DeltaAngle(base.transform.eulerAngles.z, 90f));
				break;
			case MissileStage.TargetedStrike:
			{
				float magnitude = (TargetPosition - (Vector2)base.transform.position).magnitude;
				Vector2 vector = (Target ? (Target.rigidbody.velocity * magnitude / Rigidbody.velocity.magnitude) : Vector2.zero);
				Vector2 normalized = (TargetPosition + vector - (Vector2)base.transform.position).normalized;
				Debug.DrawLine(base.transform.position, TargetPosition + vector);
				Rigidbody.AddTorque(AimingStrength * Mathf.DeltaAngle(base.transform.eulerAngles.z, 57.29578f * Mathf.Atan2(normalized.y, normalized.x)));
				Debug.DrawRay(base.transform.position, normalized);
				break;
			}
			}
			Rigidbody.angularVelocity *= 0.92f;
			Rigidbody.AddRelativeForce(LocalDirection * ForceIntensity);
			HandleAerodynamics();
		}
	}

	private void HandleAerodynamics()
	{
		Vector2 vector = base.transform.up;
		float x = base.transform.localScale.x;
		float magnitude = Rigidbody.velocity.magnitude;
		Vector2 rhs = ((magnitude == 0f) ? Vector2.right : (Rigidbody.velocity / magnitude));
		float num = Vector2.Dot(vector, rhs) * -3f;
		Rigidbody.AddForce(num * Mathf.Abs(x) * magnitude * vector);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.relativeVelocity.sqrMagnitude >= 600f)
		{
			Explode();
		}
	}

	public void Explode()
	{
		if (!exploded && takenOff)
		{
			exploded = true;
			Glow.SetActive(value: false);
			ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(base.transform.position), new ExplosionCreator.ExplosionParameters(64u, base.transform.position, 40f, 90f, createFx: false));
			ExplosionCreator.CreatePulseExplosion(base.transform.position, 2f, 40f, soundAndEffects: false);
			if (!WaterBehaviour.IsPointUnderWater(base.transform.position))
			{
				Object.Instantiate(ExplosionEffect, base.transform.position, Quaternion.identity);
			}
			TrailParticleEffect.transform.SetParent(null);
			TrailParticleEffect.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			TrailParticleEffect.gameObject.AddComponent<DeleteAfterTime>().Life = 5f;
			Object.Destroy(base.gameObject);
		}
	}

	public void OnEMPHit()
	{
		if (!broken)
		{
			Object.Instantiate(SparkPrefab, base.transform.position, Quaternion.identity);
		}
		broken = true;
		StopAllCoroutines();
		AudioSource.Stop();
		TrailParticleEffect.gameObject.SetActive(value: false);
		Rigidbody.gravityScale = 1f;
	}

	public void Shot(Shot shot)
	{
		if (shot.damage > 40f)
		{
			Explode();
		}
	}
}

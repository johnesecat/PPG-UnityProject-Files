using UnityEngine;

public class PowerHammerBehaviour : MonoBehaviour
{
	private static readonly ContactPoint2D[] buffer = new ContactPoint2D[4];

	private static readonly Collider2D[] collbuffer = new Collider2D[64];

	private float lastImpactTime = float.MinValue;

	[SerializeField]
	private Collider2D powerImpactCollider;

	[SerializeField]
	private AudioSource mainAudioSource;

	[SerializeField]
	private PhysicalBehaviour physicalBehaviour;

	[SkipSerialisation]
	public AudioClip[] ImpactClips;

	[SkipSerialisation]
	public float ForceMultiplier = 1f;

	[SkipSerialisation]
	public float Range = 1f;

	[SkipSerialisation]
	public float CameraShakeIntensity = 3f;

	[SkipSerialisation]
	public float RequiredImpactForce = 15f;

	[SkipSerialisation]
	public LayerMask LayersToConsider;

	[SkipSerialisation]
	public float ImpactCharge;

	[SkipSerialisation]
	public GameObject HammerEffectPrefab;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (Time.time - lastImpactTime < 1f || collision.otherCollider != powerImpactCollider)
		{
			return;
		}
		float num = Utils.GetMinImpulse(buffer, collision.GetContacts(buffer));
		if (collision.gameObject.TryGetComponent<LimbBehaviour>(out var _))
		{
			num *= 12f;
		}
		if (num <= RequiredImpactForce)
		{
			return;
		}
		ContactPoint2D contact = collision.GetContact(0);
		Vector2 normal = contact.normal;
		Vector2 point = contact.point;
		float z = Mathf.Atan2(normal.y, normal.x) * 57.29578f - 90f;
		GameObject gameObject = Object.Instantiate(HammerEffectPrefab, point, Quaternion.Euler(0f, 0f, z));
		Vector3 up = base.transform.up;
		if (Vector2.SignedAngle(normal, up) > 0f)
		{
			gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		float num2 = Mathf.Max(base.transform.localScale.x, base.transform.localScale.y);
		Vector2 dir = (Vector2)gameObject.transform.right.normalized * (float)((gameObject.transform.localScale.x > 0f) ? 1 : (-1));
		gameObject.transform.localScale *= num2;
		mainAudioSource.PlayOneShot(ImpactClips.PickRandom());
		CameraShakeBehaviour.main.Shake(CameraShakeIntensity * num2, base.transform.position);
		Vector2 affectingForce = dir * ForceMultiplier * num2;
		affectingForce *= Mathf.Max(1f, physicalBehaviour.Charge);
		applyForce(collision.collider);
		Vector2 pointA = point - normal;
		Vector2 pointB = point + dir * Range * num2 + normal * 3f;
		int num3 = Physics2D.OverlapAreaNonAlloc(pointA, pointB, collbuffer, LayersToConsider);
		for (int i = 0; i < num3; i++)
		{
			if (collbuffer[i].gameObject != base.gameObject)
			{
				applyForce(collbuffer[i]);
			}
		}
		ExplosionCreator.CreatePulseExplosion(point, 2f * (physicalBehaviour.Charge + 1f), 2f, soundAndEffects: false);
		lastImpactTime = Time.time;
		void applyForce(Collider2D e)
		{
			e.SendMessage("Break", dir, SendMessageOptions.DontRequireReceiver);
			if ((bool)e.attachedRigidbody)
			{
				e.attachedRigidbody.AddForce(affectingForce * e.attachedRigidbody.mass, ForceMode2D.Impulse);
			}
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(e.transform, out var value))
			{
				value.Charge += ImpactCharge;
			}
		}
	}
}

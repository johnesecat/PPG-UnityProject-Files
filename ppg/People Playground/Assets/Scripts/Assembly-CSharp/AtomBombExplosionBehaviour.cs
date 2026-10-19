using System;
using UnityEngine;

[Obsolete]
public class AtomBombExplosionBehaviour : MonoBehaviour
{
	public CircleCollider2D ShockwaveCollider;

	public DeleteAfterTime DeleteAfterTime;

	public SpriteRenderer ShockwaveRenderer;

	public float ShockwaveStrength;

	public float InverseSquareLawMultiplier = 0.5f;

	private float lastRadius;

	private void Start()
	{
		foreach (PhysicalBehaviour item in Global.main.PhysicalObjectsInWorld)
		{
			float num = Mathf.Max(float.Epsilon, (item.transform.position - base.transform.position).sqrMagnitude);
			item.Ignite(ignoreFlammability: true);
			item.Temperature += 10000f / num;
			item.BurnProgress = Mathf.Lerp(item.BurnProgress, 1f, 0.2f / (num / 281250f));
		}
	}

	private void LateUpdate()
	{
		if ((bool)ShockwaveRenderer && (bool)ShockwaveCollider)
		{
			ShockwaveCollider.radius = ShockwaveRenderer.bounds.extents.x;
			lastRadius = ShockwaveCollider.radius * ShockwaveCollider.radius;
			CameraShakeBehaviour.main.Shake(Mathf.Max(0f, 600f - ShockwaveCollider.radius), base.transform.position, 0.1f);
		}
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if ((bool)ShockwaveRenderer && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider.transform, out var value))
		{
			Vector2 vector = value.transform.position - base.transform.position;
			float sqrMagnitude = vector.sqrMagnitude;
			if (!(Mathf.Abs(lastRadius - sqrMagnitude) > 10000f))
			{
				float shockwaveStrength = ShockwaveStrength;
				Vector2 vector2 = vector.normalized * shockwaveStrength / Mathf.Lerp(1f, Mathf.Max(sqrMagnitude, 1f), InverseSquareLawMultiplier);
				value.rigidbody.AddForce(vector2 * value.rigidbody.mass, ForceMode2D.Impulse);
				value.gameObject.SendMessage("Break", vector2, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}

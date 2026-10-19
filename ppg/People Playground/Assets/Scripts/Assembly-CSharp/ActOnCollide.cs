using UnityEngine;
using UnityEngine.Events;

public class ActOnCollide : MonoBehaviour
{
	[SkipSerialisation]
	public float ImpactForceThreshold;

	[SkipSerialisation]
	public float DispatchChance = 1f;

	[SkipSerialisation]
	[Tooltip("Leave null to allow any collider to trigger the event")]
	public Collider2D Collider;

	[SkipSerialisation]
	public UnityEvent Actions;

	private static ContactPoint2D[] buffer = new ContactPoint2D[4];

	private void OnCollisionEnter2D(Collision2D coll)
	{
		if (!(Random.value > DispatchChance))
		{
			int contacts = coll.GetContacts(buffer);
			if ((!Collider || coll.otherCollider == Collider) && Utils.GetAverageImpulseRemoveOutliers(buffer, contacts) > ImpactForceThreshold)
			{
				Actions.Invoke();
			}
		}
	}
}

using System.Collections;
using UnityEngine;

public class BalloonBehaviour : MonoBehaviour, Messages.IExitShot, Messages.IShot, Messages.IStabbed, Messages.IBreak, Messages.IOnFragmentHit, Messages.IOnInsideVacuum
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public GameObject PoppedEffect;

	[SkipSerialisation]
	public float CollisionPressureThreshold = 5f;

	private bool popped;

	private static ContactPoint2D[] buffer = new ContactPoint2D[4];

	public void ExitShot(Shot shot)
	{
		StopAllCoroutines();
		Pop();
	}

	public void Shot(Shot shot)
	{
		StartCoroutine(WaitAFrame());
		IEnumerator WaitAFrame()
		{
			yield return new WaitForSeconds(0.05f);
			Pop();
		}
	}

	public void OnFragmentHit(float force)
	{
		Pop();
	}

	public void Break(Vector2 velocity = default(Vector2))
	{
		Pop();
	}

	public void Stabbed(Stabbing stab)
	{
		Pop();
	}

	private void Update()
	{
		if (PhysicalBehaviour.BurnProgress > 0.02f || PhysicalBehaviour.Charge > 0.01f)
		{
			Pop();
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (Utils.GetAverageImpulseRemoveOutliers(buffer, collision.GetContacts(buffer)) > CollisionPressureThreshold)
		{
			Pop();
		}
	}

	public void Pop()
	{
		if (!popped)
		{
			popped = true;
			PoppedEffect.SetActive(value: true);
			PoppedEffect.transform.SetParent(null);
			ParticleSystem.ShapeModule shape = PoppedEffect.GetComponent<ParticleSystem>().shape;
			shape.texture = GetComponent<SpriteRenderer>().sprite.texture;
			Object.Destroy(base.gameObject);
		}
	}

	public void OnInsideVacuum()
	{
		Pop();
	}
}

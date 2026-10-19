using UnityEngine;

public class StunnerBehaviour : MonoBehaviour
{
	public float Speed = 1f;

	public float Gravity = 1f;

	public GameObject ImpactEffect;

	public LayerMask mask;

	private float t;

	private void Update()
	{
		Vector3 right = base.transform.right;
		right.y -= t;
		right.Normalize();
		float num = Speed * Time.deltaTime;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, right, num, mask);
		if ((bool)raycastHit2D)
		{
			Object.Destroy(base.gameObject);
			Object.Instantiate(ImpactEffect, base.transform.position, Quaternion.identity);
			raycastHit2D.transform.SendMessage("StunImpact", new Shot(raycastHit2D.normal, raycastHit2D.point, 0f), SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			base.transform.position += right * num;
		}
		t += Time.deltaTime * 0.1f * Gravity;
	}
}

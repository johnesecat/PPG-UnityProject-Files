using System;
using UnityEngine;

[Obsolete]
public class MachineGunProjectileBehaviour : MonoBehaviour
{
	public float UnitsPerSecond = 1f;

	private LayerMask mask;

	private float f;

	private void Awake()
	{
		mask = LayerMask.GetMask("Objects", "Bounds");
	}

	private void Update()
	{
		Vector3 right = base.transform.right;
		right.y -= f;
		right.Normalize();
		float num = UnitsPerSecond * Time.deltaTime;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, right, num, mask);
		if ((bool)raycastHit2D)
		{
			Vector2 vector = raycastHit2D.point + raycastHit2D.normal * 0.05f;
			UnityEngine.Object.Destroy(base.gameObject);
			ExplosionCreator.CreateFragmentationExplosion(8u, vector, 2f, 4f, particleAndSound: true);
			raycastHit2D.transform.SendMessage("Shot", new Shot(raycastHit2D.normal, raycastHit2D.point, 35f), SendMessageOptions.DontRequireReceiver);
			raycastHit2D.transform.SendMessage("ExitShot", new Shot(raycastHit2D.normal, raycastHit2D.point, 35f), SendMessageOptions.DontRequireReceiver);
			raycastHit2D.transform.SendMessage("Break", num * (Vector2)right, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			base.transform.position += right * num;
		}
		f += Time.deltaTime * 0.1f;
	}
}

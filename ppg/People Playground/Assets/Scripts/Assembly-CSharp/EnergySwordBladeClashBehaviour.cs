using UnityEngine;

public class EnergySwordBladeClashBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public GameObject CollideWithOtherSwordVFX;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Object.Instantiate(CollideWithOtherSwordVFX, collision.GetContact(0).point, Quaternion.identity);
	}
}

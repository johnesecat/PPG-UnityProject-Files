using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
	public Vector2 LocalDirection = Vector2.right;

	public float Force = 5f;

	[SkipSerialisation]
	public Rigidbody2D Rigidbody;

	private void Start()
	{
		Rigidbody.AddForce(base.transform.TransformDirection(LocalDirection) * Force, ForceMode2D.Impulse);
	}
}

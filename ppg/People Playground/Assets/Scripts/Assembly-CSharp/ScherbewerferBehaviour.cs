using UnityEngine;

public class ScherbewerferBehaviour : MonoBehaviour, Messages.IUse
{
	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	[Space]
	public PhysicalBehaviour Projectile;

	public PhysicalBehaviour PhysicalBehaviour;

	public LineRenderer Rope;

	public float BaseLaunchForce = 1f;

	public Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	public void Use(ActivationPropagation activation)
	{
		Launch();
	}

	private void Launch()
	{
		Vector3 vector = base.transform.right * (BaseLaunchForce + PhysicalBehaviour.charge);
		Projectile.rigidbody.AddForce(vector, ForceMode2D.Impulse);
		PhysicalBehaviour.rigidbody.AddForce(-vector, ForceMode2D.Impulse);
	}
}

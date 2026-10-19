using UnityEngine;

public struct Stabbing
{
	public PhysicalBehaviour stabber;

	public PhysicalBehaviour victim;

	public Vector2 normal;

	public Vector2 point;

	public Stabbing(PhysicalBehaviour stabber, PhysicalBehaviour victim, Vector2 normal, Vector2 point)
	{
		this.stabber = stabber;
		this.victim = victim;
		this.normal = normal;
		this.point = point;
	}
}

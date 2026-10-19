using UnityEngine;

[RequireComponent(typeof(BloodContainer))]
public class PointLiquidTransferBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public Vector2 Point;

	[SkipSerialisation]
	public Space Space;

	[SkipSerialisation]
	public float UnitsPerSecond = 0.56f;

	[SkipSerialisation]
	public LayerMask Layers;

	[SkipSerialisation]
	public bool OnlyWhileUse = true;

	private BloodContainer bloodContainer;

	private PhysicalBehaviour physicalBehaviour;

	private void Awake()
	{
		bloodContainer = GetComponent<BloodContainer>();
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	private void FixedUpdate()
	{
		if (physicalBehaviour.IsBeingUsedContinuously())
		{
			Transfer(Time.fixedDeltaTime);
		}
	}

	private void Transfer(float dt)
	{
		float amount = UnitsPerSecond * dt;
		Collider2D collider2D = Physics2D.OverlapPoint(GetPoint(), Layers);
		if ((bool)collider2D && collider2D.transform != base.transform && collider2D.TryGetComponent<BloodContainer>(out var component) && (!component.IsFull() || component.AllowsOverflow))
		{
			bloodContainer.TransferTo(amount, component);
		}
	}

	public Vector2 GetPoint()
	{
		switch (Space)
		{
		case Space.World:
			return Point;
		default:
			return base.transform.TransformPoint(Point);
		}
	}
}

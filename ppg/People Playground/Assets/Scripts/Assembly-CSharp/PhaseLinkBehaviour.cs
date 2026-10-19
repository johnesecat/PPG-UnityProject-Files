using UnityEngine;

public class PhaseLinkBehaviour : LinkDeviceBehaviour
{
	protected override void AfterInitialise()
	{
		Collider2D[] componentsInChildren = PhysicalBehaviour.transform.root.GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider2D in componentsInChildren)
		{
			Collider2D[] componentsInChildren2 = Other.transform.root.GetComponentsInChildren<Collider2D>();
			foreach (Collider2D collider2D2 in componentsInChildren2)
			{
				if (!(collider2D == collider2D2))
				{
					IgnoreCollisionStackController.RequestIgnoreCollision(collider2D, collider2D2);
				}
			}
		}
	}

	protected override void OnDestroy()
	{
		if ((bool)PhysicalBehaviour && (bool)Other)
		{
			Collider2D[] componentsInChildren = PhysicalBehaviour.transform.root.GetComponentsInChildren<Collider2D>();
			foreach (Collider2D collider2D in componentsInChildren)
			{
				Collider2D[] componentsInChildren2 = Other.transform.root.GetComponentsInChildren<Collider2D>();
				foreach (Collider2D collider2D2 in componentsInChildren2)
				{
					if (!(collider2D == collider2D2))
					{
						IgnoreCollisionStackController.RequestDontIgnoreCollision(collider2D, collider2D2);
					}
				}
			}
		}
		base.OnDestroy();
	}

	protected override Sprite GetDeviceSprite()
	{
		return Resources.Load<Sprite>("Sprites/PhaseLink");
	}

	protected override Color GetWireColor()
	{
		return Color.white;
	}

	protected override Material GetWireMaterial()
	{
		return Resources.Load<Material>("Materials/PhaseLink");
	}

	protected override float GetWireWidth()
	{
		return 0.08f;
	}
}

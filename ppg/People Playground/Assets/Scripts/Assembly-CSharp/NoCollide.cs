using UnityEngine;

public class NoCollide : MonoBehaviour
{
	public Collider2D[] NoCollideSetA = new Collider2D[0];

	public Collider2D[] NoCollideSetB = new Collider2D[0];

	public bool BisA;

	private void Start()
	{
		Collider2D[] noCollideSetA = NoCollideSetA;
		foreach (Collider2D collider2D in noCollideSetA)
		{
			Collider2D[] array = (BisA ? NoCollideSetA : NoCollideSetB);
			foreach (Collider2D collider2D2 in array)
			{
				if ((bool)collider2D && (bool)collider2D2 && collider2D != collider2D2)
				{
					IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(collider2D, collider2D2);
				}
			}
		}
	}

	private void OnDestroy()
	{
		Collider2D[] noCollideSetA = NoCollideSetA;
		foreach (Collider2D collider2D in noCollideSetA)
		{
			Collider2D[] noCollideSetB = NoCollideSetB;
			foreach (Collider2D collider2D2 in noCollideSetB)
			{
				if ((bool)collider2D2 && (bool)collider2D)
				{
					IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(collider2D, collider2D2, ignore: false);
				}
			}
		}
	}
}

using UnityEngine;

public class NoCollideGm : MonoBehaviour
{
	public GameObject A;

	public GameObject B;

	private void Awake()
	{
		if (!A || !B)
		{
			return;
		}
		Collider2D[] componentsInChildren = A.GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider2D in componentsInChildren)
		{
			Collider2D[] componentsInChildren2 = B.GetComponentsInChildren<Collider2D>();
			foreach (Collider2D collider2D2 in componentsInChildren2)
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
		if (!A || !B)
		{
			return;
		}
		Collider2D[] componentsInChildren = A.GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider2D in componentsInChildren)
		{
			Collider2D[] componentsInChildren2 = B.GetComponentsInChildren<Collider2D>();
			foreach (Collider2D collider2D2 in componentsInChildren2)
			{
				if ((bool)collider2D2 && (bool)collider2D)
				{
					IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(collider2D, collider2D2, ignore: false);
				}
			}
		}
	}
}

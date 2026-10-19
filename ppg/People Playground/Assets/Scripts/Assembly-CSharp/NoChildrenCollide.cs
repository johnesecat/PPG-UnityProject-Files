using UnityEngine;

public class NoChildrenCollide : MonoBehaviour
{
	private void Start()
	{
		Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
		Collider2D[] array = componentsInChildren;
		foreach (Collider2D collider2D in array)
		{
			Collider2D[] array2 = componentsInChildren;
			foreach (Collider2D collider2D2 in array2)
			{
				if ((bool)collider2D && (bool)collider2D2 && collider2D != collider2D2 && collider2D.transform != collider2D2.transform)
				{
					IgnoreCollisionStackController.RequestIgnoreCollision(collider2D, collider2D2);
				}
			}
		}
	}
}

using UnityEngine;

public class ArrayCloner : MonoBehaviour
{
	public int Amount;

	public Vector2 Delta;

	public void Start()
	{
		Vector2 vector = base.transform.position;
		for (int i = 1; i < Amount; i++)
		{
			Vector2 vector2 = vector + Delta * i;
			GameObject obj = Object.Instantiate(base.gameObject, vector2, base.transform.rotation);
			obj.transform.localScale = base.transform.lossyScale;
			Object.Destroy(obj.GetComponent<ArrayCloner>());
			obj.transform.SetParent(base.transform.parent, worldPositionStays: true);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Bounds bounds = GetComponent<SpriteRenderer>().bounds;
		Vector2 vector = base.transform.position;
		for (int i = 1; i < Amount; i++)
		{
			Gizmos.DrawWireCube(vector + Delta * i, bounds.size);
		}
	}
}

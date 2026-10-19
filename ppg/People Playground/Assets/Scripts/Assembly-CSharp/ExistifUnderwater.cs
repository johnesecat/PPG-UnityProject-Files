using UnityEngine;

public class ExistifUnderwater : MonoBehaviour
{
	public Vector2 Offset;

	public Space Space;

	public bool Invert;

	private Vector2 GetPoint()
	{
		Space space = Space;
		if (space != 0 && space == Space.Self)
		{
			return (Vector2)base.transform.position + (Vector2)base.transform.TransformVector(Offset);
		}
		return (Vector2)base.transform.position + Offset;
	}

	private void Start()
	{
		base.gameObject.SetActive(Invert ^ WaterBehaviour.IsPointUnderWater(GetPoint()));
	}
}

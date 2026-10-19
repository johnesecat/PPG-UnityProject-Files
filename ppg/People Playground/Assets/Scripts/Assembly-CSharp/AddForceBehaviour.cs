using UnityEngine;

[SkipSerialisation]
public class AddForceBehaviour : MonoBehaviour
{
	public ForceMode2D ForceMode;

	public Vector2 LocalAxis;

	public bool ScaleWithSize;

	public void AddRelativeForce(float intensity)
	{
		if (ScaleWithSize)
		{
			float num = Mathf.Abs(base.transform.lossyScale.x * base.transform.lossyScale.y);
			intensity *= num;
		}
		Debug.Log(base.transform.lossyScale.x * base.transform.lossyScale.y);
		GetComponent<Rigidbody2D>().AddForce(base.transform.TransformDirection(LocalAxis) * intensity, ForceMode);
	}
}

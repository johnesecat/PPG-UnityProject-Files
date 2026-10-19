using UnityEngine;

[ExecuteInEditMode]
public class RotateUI : MonoBehaviour
{
	public float Speed;

	private void Update()
	{
		base.transform.Rotate(0f, 0f, Time.unscaledDeltaTime * Speed);
	}
}

using UnityEngine;

public class SlightMovement : MonoBehaviour
{
	public float Intensity = 1f;

	public float Speed = 1f;

	private void Update()
	{
		float num = Speed * Time.time;
		base.transform.position = Intensity * Utils.GetPerlin2Mapped(num, num + 7892.387f);
	}
}

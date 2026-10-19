using UnityEngine;

public class WindControllerBehaviour : MonoBehaviour
{
	private WindZone windZone;

	public float BaseStrength = 3f;

	private void Awake()
	{
		windZone = GetComponent<WindZone>();
	}

	private void Update()
	{
		float num = Time.time * 0.05f;
		float num2 = Mathf.PerlinNoise(-57.1532f, 0f - num) * 2f - 1f;
		windZone.windMain = BaseStrength * Mathf.PerlinNoise(num, 5253.294f);
		base.transform.Rotate(0f, 0.5f * num2, 0f);
	}
}

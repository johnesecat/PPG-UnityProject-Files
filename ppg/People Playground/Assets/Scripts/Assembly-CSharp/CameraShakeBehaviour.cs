using UnityEngine;

public class CameraShakeBehaviour : MonoBehaviour
{
	public static CameraShakeBehaviour main;

	private float shakeIntensity;

	private void Awake()
	{
		main = this;
	}

	private void FixedUpdate()
	{
		shakeIntensity *= 0.85f;
	}

	private void Update()
	{
		shakeIntensity = Mathf.Clamp(shakeIntensity.NaNFallback(), 0f, 5f);
		float num = Noise(1000f) * shakeIntensity * 0.1f;
		float y = Mathf.Sin(Time.time * 80f) * shakeIntensity * 0.04f;
		float num2 = Noise(0f) * shakeIntensity * 0.25f;
		num *= num;
		num2 *= num2;
		float num3 = UserPreferenceManager.Current.ShakeIntensity;
		base.transform.root.position = 0.5f * num3 * new Vector3(num, y, 0f);
		base.transform.eulerAngles = 0.5f * num3 * new Vector3(0f, 0f, num2);
	}

	private float Noise(float s)
	{
		return Mathf.PerlinNoise(Time.time * 24f, s) * 2f - 1f;
	}

	public void Shake(float intensity, Vector2 position, float distanceInfluence = 1f)
	{
		float num = 1f - CameraListenerBehaviour.NormalisedDistance * 0.5f;
		float num2 = Mathf.Max(1f, Vector2.Distance(position, base.transform.position) * distanceInfluence);
		shakeIntensity += intensity * num / num2;
	}
}

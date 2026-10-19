using UnityEngine;

public class DecalDripStreakBehaviour : MonoBehaviour
{
	public static float MinLocalStreakLength = 1f;

	public static float MaxLocalStreakLength = 3f;

	public static float StreakDripSpeed = 0.2f;

	private float radius;

	private float t;

	private float speed;

	private float length;

	public const float ReferenceGravity = 9.81f;

	private void Start()
	{
		speed = Random.Range(0.5f, 1f) * StreakDripSpeed;
		length = Random.Range(MinLocalStreakLength, MaxLocalStreakLength);
		radius = base.transform.localScale.x;
	}

	private void Update()
	{
		Vector3 vector = base.transform.parent.worldToLocalMatrix.MultiplyVector(Physics2D.gravity);
		float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		float num = vector.sqrMagnitude / 96.23611f;
		t += Time.deltaTime * speed * num;
		float num2 = EaseOut(Mathf.Clamp01(t));
		base.transform.localScale = new Vector3(radius + num2 * length, radius, radius);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, z);
		if (t > 1f || num <= float.Epsilon)
		{
			base.gameObject.isStatic = true;
			Object.Destroy(this);
		}
	}

	private float EaseOut(float x)
	{
		float num = 1f - x;
		return 1f - num * num * num;
	}
}

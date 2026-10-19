using System;
using UnityEngine;

[Obsolete]
public class ShockwaveBehaviour : MonoBehaviour
{
	public float Duration = 0.02f;

	public float SizeMultiplier = 5f;

	public AnimationCurve GrowingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private float startSize;

	public MeshRenderer renderer;

	private float t;

	private void Start()
	{
		startSize = base.transform.localScale.x;
		renderer.enabled = false;
	}

	private void Update()
	{
		t += Time.deltaTime;
		float num = t / Duration;
		float num2 = GrowingCurve.Evaluate(num) * SizeMultiplier + startSize;
		base.transform.localScale = new Vector3(num2, num2, 0f);
		renderer.material.SetFloat("_Intensity", (1f - num) * 0.1f);
		if (t > Duration)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}

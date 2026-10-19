using UnityEngine;

[ExecuteAlways]
public class ColorWave : MonoBehaviour
{
	[SkipSerialisation]
	public SpriteRenderer Renderer;

	[SkipSerialisation]
	public Gradient Gradient;

	public float Duration;

	public float Offset;

	public bool UseSinWave = true;

	public float OffsetVaration;

	public float SpeedVaration;

	public float Randomisation;

	private float oV;

	private float sV;

	private void OnEnable()
	{
		oV = Random.value;
		sV = Random.value;
	}

	private void OnWillRenderObject()
	{
		if ((bool)Renderer && Gradient != null && Duration > 0f)
		{
			float num = Time.time * (1f + sV * SpeedVaration) / Duration + Offset + oV * OffsetVaration;
			float num2 = (UseSinWave ? (Mathf.Sin(num) * 0.5f + 0.5f) : Utils.Mod(num, 1f));
			if (Randomisation != 0f)
			{
				num2 += Utils.GetPerlin2Mapped(Time.time / Duration, oV * 423f).x;
			}
			Renderer.color = Gradient.Evaluate(num2);
		}
	}
}

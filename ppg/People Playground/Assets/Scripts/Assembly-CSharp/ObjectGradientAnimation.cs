using UnityEngine;

[ExecuteInEditMode]
public abstract class ObjectGradientAnimation : MonoBehaviour
{
	public Gradient Gradient;

	public Color Tint = Color.white;

	public float Speed = 0.1f;

	public float Offset;

	public CycleType CycleType;

	public bool ReplayOnEnable;

	public float PositionOffsetInfluence;

	private float t;

	private void Start()
	{
		ApplyColour();
	}

	private void Update()
	{
		t += Time.deltaTime * Speed;
		ApplyColour();
	}

	private void OnEnable()
	{
		if (ReplayOnEnable)
		{
			t = 0f;
			ApplyColour();
		}
	}

	private void ApplyColour()
	{
		float num = t + Offset + Mathf.Lerp(0f, base.transform.position.x, PositionOffsetInfluence) + Mathf.Lerp(0f, base.transform.position.y, PositionOffsetInfluence);
		switch (CycleType)
		{
		case CycleType.Once:
			num = Mathf.Clamp01(num);
			break;
		case CycleType.SineWave:
			num = Mathf.Sin(num) * 0.5f + 0.5f;
			break;
		case CycleType.SawTooth:
			num %= 1f;
			break;
		}
		SetColor(Gradient.Evaluate(num) * Tint);
	}

	protected abstract void SetColor(Color color);
}

using UnityEngine;

public class ColorAnimationBehaviour : TrivialAnimationBehaviour
{
	public Gradient ColourOverTime;

	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected override void AnimationTick(float progress)
	{
		Color color = ColourOverTime.Evaluate(progress);
		spriteRenderer.color = color;
	}
}

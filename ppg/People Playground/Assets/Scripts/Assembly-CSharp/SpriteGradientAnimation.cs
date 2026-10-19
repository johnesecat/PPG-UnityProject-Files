using UnityEngine;

public class SpriteGradientAnimation : ObjectGradientAnimation
{
	[Space]
	public SpriteRenderer SpriteRenderer;

	protected override void SetColor(Color color)
	{
		if ((bool)SpriteRenderer)
		{
			SpriteRenderer.color = color;
		}
	}
}

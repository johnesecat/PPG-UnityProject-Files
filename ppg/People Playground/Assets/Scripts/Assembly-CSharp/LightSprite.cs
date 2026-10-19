using UnityEngine;

public class LightSprite : MonoBehaviour
{
	public SpriteRenderer SpriteRenderer;

	private Color color;

	private float brightness;

	private float radius;

	private MaterialPropertyBlock propertyBlock;

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
			SpriteRenderer.color = color;
		}
	}

	public float Brightness
	{
		get
		{
			return brightness;
		}
		set
		{
			brightness = value;
			propertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), value);
			SpriteRenderer.SetPropertyBlock(propertyBlock);
		}
	}

	public float Radius
	{
		get
		{
			return radius;
		}
		set
		{
			radius = value;
			base.transform.localScale = Vector3.one * radius;
		}
	}

	private void Awake()
	{
		if (!SpriteRenderer)
		{
			SpriteRenderer = GetComponent<SpriteRenderer>();
		}
		propertyBlock = new MaterialPropertyBlock();
		SpriteRenderer.GetPropertyBlock(propertyBlock);
	}
}

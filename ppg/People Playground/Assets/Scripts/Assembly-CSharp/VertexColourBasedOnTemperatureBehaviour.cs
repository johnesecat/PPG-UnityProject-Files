using UnityEngine;

public class VertexColourBasedOnTemperatureBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public Gradient Gradient;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public SpriteRenderer SpriteRenderer;

	[SkipSerialisation]
	public Vector2 TemperatureRange = new Vector2(0f, 7000f);

	private void Update()
	{
		if (SpriteRenderer.isVisible)
		{
			float time = Utils.MapRange(TemperatureRange.x, TemperatureRange.y, 0f, 1f, PhysicalBehaviour.Temperature);
			SpriteRenderer.color = Gradient.Evaluate(time);
		}
	}
}

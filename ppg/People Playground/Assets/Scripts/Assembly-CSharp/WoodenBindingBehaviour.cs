using UnityEngine;

public class WoodenBindingBehaviour : SteelBindingBehaviour
{
	protected override void Created()
	{
		SteelBindingBehaviour.stress = Resources.LoadAll<AudioClip>("Audio/wooden binding stress/");
		WireColor = Color.white;
		lineRenderer.startColor = Color.white;
		lineRenderer.endColor = Color.white;
		lineRenderer.numCapVertices = 0;
		lineRenderer.sortingLayerName = "Background";
		lineRenderer.textureMode = LineTextureMode.Tile;
	}

	protected override void JointBroken()
	{
		if ((bool)physicalBehaviour)
		{
			physicalBehaviour.PlayClipOnce(Resources.LoadAll<AudioClip>("Audio/wooden binding break").PickRandom(), Random.value);
		}
	}
}

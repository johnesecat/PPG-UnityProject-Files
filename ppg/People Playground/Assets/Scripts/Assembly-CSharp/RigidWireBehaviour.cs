using UnityEngine;

public class RigidWireBehaviour : DistanceJointWireBehaviour
{
	protected override void Start()
	{
		base.Start();
		if (lineRenderer.startColor == LegacyColour)
		{
			SetColor(Color.white);
		}
	}

	public override int GetVertexCount()
	{
		return 1;
	}
}

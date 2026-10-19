using UnityEngine;

public class FixedCableBehaviour : FixedJointWireBehaviour
{
	protected override void Start()
	{
		base.Start();
		if (lineRenderer.startColor == LegacyColour)
		{
			SetColor(Color.white);
		}
	}
}

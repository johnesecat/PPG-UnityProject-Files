using System.Collections.Generic;
using UnityEngine;

public class CenterOfBounds : CenterOfGravity
{
	protected override Vector3 GetPosition(IEnumerable<PhysicalBehaviour> phys)
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		foreach (PhysicalBehaviour phy in phys)
		{
			if ((bool)phy && (bool)phy.spriteRenderer)
			{
				Bounds bounds = phy.spriteRenderer.bounds;
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (min.x < num)
				{
					num = min.x;
				}
				if (min.y < num3)
				{
					num3 = min.y;
				}
				if (max.x > num2)
				{
					num2 = max.x;
				}
				if (max.y > num4)
				{
					num4 = max.y;
				}
			}
		}
		return new Vector3((num + num2) / 2f, (num3 + num4) / 2f, 0f);
	}
}

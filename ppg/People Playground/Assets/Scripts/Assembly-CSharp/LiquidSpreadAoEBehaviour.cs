using System.Collections.Generic;
using UnityEngine;

public class LiquidSpreadAoEBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public BloodContainer LiquidContainer;

	[SkipSerialisation]
	public float AmountMultiplier = 1f;

	[SkipSerialisation]
	public bool LimbsOnly;

	[SkipSerialisation]
	public bool SpreadOnStart;

	[SkipSerialisation]
	public float Range = 1f;

	[SkipSerialisation]
	public LayerMask Mask;

	private static readonly Collider2D[] results = new Collider2D[64];

	private void Start()
	{
		if (SpreadOnStart)
		{
			Spread();
		}
	}

	public void Spread()
	{
		int num = Mathf.Min(results.Length, Physics2D.OverlapCircleNonAlloc(base.transform.position, Range, results, Mask));
		float amount = LiquidContainer.TotalLiquidAmount * AmountMultiplier / (float)num * 3f;
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = results[i];
			if (!collider2D.TryGetComponent<BloodContainer>(out var component) || (LimbsOnly && !collider2D.TryGetComponent<LimbBehaviour>(out var _)))
			{
				continue;
			}
			foreach (KeyValuePair<Liquid, BloodContainer.RefFloat> item in LiquidContainer.LiquidDistribution)
			{
				component.AddLiquid(item.Key, amount);
			}
		}
	}
}

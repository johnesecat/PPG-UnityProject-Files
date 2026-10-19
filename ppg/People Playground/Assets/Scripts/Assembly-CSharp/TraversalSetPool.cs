using System.Collections.Generic;
using UnityEngine;

public class TraversalSetPool : Pool<HashSet<Object>>
{
	public TraversalSetPool(int maxCapacity = 10000)
		: base(maxCapacity)
	{
	}

	protected override HashSet<Object> CreateFresh()
	{
		return new HashSet<Object>();
	}

	protected override HashSet<Object> GetOverCapacityFallback()
	{
		return new HashSet<Object>();
	}

	protected override void ResetObjectForNextUse(HashSet<Object> obj)
	{
		obj.Clear();
	}
}

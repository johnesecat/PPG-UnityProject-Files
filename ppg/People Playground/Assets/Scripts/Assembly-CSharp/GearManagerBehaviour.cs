using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class GearManagerBehaviour : MonoBehaviour
{
	private class RandomComparer<T> : IComparer<T>
	{
		public int Compare(T x, T y)
		{
			return UnityEngine.Random.Range(-100, 100);
		}
	}

	public static readonly List<GearBehaviour> Gears = new List<GearBehaviour>();

	private const int iterationsPerSecond = 256;

	private static readonly FixedIntervalDistributor distributor = new FixedIntervalDistributor();

	private static readonly RandomComparer<GearBehaviour> comparer = new RandomComparer<GearBehaviour>();

	private void Awake()
	{
		Gears.Clear();
		distributor.RateHz = 256f;
		distributor.MaxCycles = 320;
	}

	protected virtual void Update()
	{
		int num = distributor.CalculateCycleCount(Time.deltaTime);
		Gears.Sort(comparer);
		float intervalSeconds = distributor.IntervalSeconds;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < Gears.Count; j++)
			{
				GearBehaviour gearBehaviour = Gears[j];
				if (gearBehaviour != null && gearBehaviour.enabled)
				{
					gearBehaviour.ManagedUpdate(intervalSeconds);
				}
			}
		}
		for (int k = 0; k < Gears.Count; k++)
		{
			GearBehaviour gearBehaviour2 = Gears[k];
			if (gearBehaviour2 != null && gearBehaviour2.enabled)
			{
				gearBehaviour2.ManagedLateUpdate();
			}
		}
	}
}

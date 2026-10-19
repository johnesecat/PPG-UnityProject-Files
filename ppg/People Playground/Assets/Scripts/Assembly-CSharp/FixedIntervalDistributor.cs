using System;
using UnityEngine;

public class FixedIntervalDistributor
{
	private float timeStepAccumulator;

	private float updatesPerSecond = 60f;

	public float RateHz
	{
		get
		{
			return updatesPerSecond;
		}
		set
		{
			updatesPerSecond = value;
			IntervalSeconds = 1f / value;
		}
	}

	[Obsolete]
	public float Rate
	{
		get
		{
			return RateHz;
		}
		set
		{
			RateHz = value;
		}
	}

	public int MaxCycles { get; set; } = 256;


	[Obsolete]
	public int MaxRate
	{
		get
		{
			return MaxCycles;
		}
		set
		{
			MaxCycles = value;
		}
	}

	public float IntervalSeconds { get; private set; } = 0.016666668f;


	public void ResetAccumulator(float t = 0f)
	{
		timeStepAccumulator = t;
	}

	public int CalculateCycleCount(float realDeltaTimeInSeconds)
	{
		timeStepAccumulator += realDeltaTimeInSeconds;
		int num = Math.Min(MaxCycles, Mathf.FloorToInt(timeStepAccumulator / IntervalSeconds));
		if (num > 0)
		{
			timeStepAccumulator -= (float)num * IntervalSeconds;
		}
		return num;
	}
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ContinuousActivationBehaviour : MonoBehaviour
{
	private readonly FixedIntervalDistributor activationSignalProcessor = new FixedIntervalDistributor
	{
		RateHz = 40f,
		MaxCycles = 4
	};

	public UnityEvent<float> OnContinuousUpdate = new UnityEvent<float>();

	private float cleanupTimer;

	private const float cleanupIntervalSeconds = 1f;

	private uint oscillation;

	private List<ActivationPropagation> toDelete = new List<ActivationPropagation>();

	public static ContinuousActivationBehaviour Instance { get; private set; }

	public bool CanProcessContinuousSignals { get; private set; }

	public static float DeltaTime => Instance.activationSignalProcessor.IntervalSeconds;

	public static bool AssertState()
	{
		_ = Instance.CanProcessContinuousSignals;
		return Instance.CanProcessContinuousSignals;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		activationSignalProcessor.ResetAccumulator();
	}

	private void LateUpdate()
	{
		for (int i = 0; i < Global.main.PhysicalObjectsInWorld.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = Global.main.PhysicalObjectsInWorld[i];
			for (int j = 0; j < ActivationPropagation.AllChannels.Length; j++)
			{
				ushort key = ActivationPropagation.AllChannels[j];
				physicalBehaviour.ContinuousActivationTracker[key].UpdatePreviousValue();
			}
		}
		CanProcessContinuousSignals = true;
		for (int k = 0; k < activationSignalProcessor.CalculateCycleCount(Time.deltaTime); k++)
		{
			OnContinuousUpdate?.Invoke(activationSignalProcessor.IntervalSeconds);
			if (oscillation % 2u == 0)
			{
				for (int l = 0; l < Global.main.PhysicalObjectsInWorld.Count; l++)
				{
					PhysicalBehaviour physicalBehaviour2 = Global.main.PhysicalObjectsInWorld[l];
					for (int m = 0; m < ActivationPropagation.AllChannels.Length; m++)
					{
						ushort key2 = ActivationPropagation.AllChannels[m];
						DeltaInt deltaInt = physicalBehaviour2.ContinuousActivationTracker[key2];
						if (deltaInt.PreviousValueFixed == deltaInt.Value)
						{
							deltaInt.Value = 0;
						}
						deltaInt.PreviousValueFixed = deltaInt.Value;
					}
				}
			}
			oscillation++;
		}
		CanProcessContinuousSignals = false;
		cleanupTimer += Time.deltaTime;
		if (!(cleanupTimer > 1f))
		{
			return;
		}
		cleanupTimer = 0f;
		toDelete.Clear();
		foreach (ActivationPropagation item in ActivationPropagation.ActiveSignalSet)
		{
			int previousActivityIntensity = item.PreviousActivityIntensity;
			item.PreviousActivityIntensity = item.ActivityIntensity;
			if (item.ActivityIntensity == previousActivityIntensity)
			{
				toDelete.Add(item);
			}
		}
		foreach (ActivationPropagation item2 in toDelete)
		{
			if (item2 != null)
			{
				item2.Dispose();
				continue;
			}
			Debug.LogWarningFormat("Null signal found and deleted. This is not normal.");
			ActivationPropagation.ActiveSignalSet.Remove(item2);
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BloodContainer : MonoBehaviour, Messages.IOnAfterDeserialise, Messages.IOnBeforeSerialise
{
	public enum PressureDirection
	{
		Push,
		Pull,
		None
	}

	[Serializable]
	public struct SerialisableDistribution
	{
		public string LiquidID;

		public float Amount;
	}

	public class RefFloat
	{
		public float Previous;

		private float raw;

		public float Raw
		{
			get
			{
				return raw;
			}
			set
			{
				raw = Mathf.Clamp(value, 0f, float.MaxValue);
			}
		}

		public bool HasIncreased(float threshold = 0.0001f)
		{
			return Raw - Previous > threshold;
		}

		public bool HasDecreased(float threshold = 0.0001f)
		{
			return Previous - Raw > threshold;
		}

		public bool HasChanged(float threshold = 0.0001f)
		{
			return Mathf.Abs(Previous - Raw) > threshold;
		}

		public RefFloat(float v)
		{
			Raw = v;
		}
	}

	public const float PushPressureValue = 4096f;

	public const float PullPressureValue = -4096f;

	private bool isComputedColourCached;

	private Color cachedComputedColour;

	public const float RemovalThreshold = 0.02f;

	public UnityEvent OnDistributionChange;

	[HideInInspector]
	public float MeasuredPressure;

	[SkipSerialisation]
	public Dictionary<Liquid, RefFloat> LiquidDistribution = new Dictionary<Liquid, RefFloat>();

	private static readonly Liquid[] liquidBuffer = new Liquid[256];

	private bool shouldDeleteEmptyEntries;

	public SerialisableDistribution[] SerialisableDistributions;

	private float timer;

	[SkipSerialisation]
	public float TotalLiquidAmount { get; private set; }

	[SkipSerialisation]
	public virtual float ScaledLiquidAmount => Utils.MapRange(Limits.x, Limits.y, 0f, 1f, TotalLiquidAmount);

	[SkipSerialisation]
	public virtual PressureDirection Pressure => PressureDirection.None;

	[SkipSerialisation]
	public virtual bool AllowsOverflow => true;

	[SkipSerialisation]
	public virtual bool AllowsTransfer => true;

	[SkipSerialisation]
	public virtual Vector2 Limits => new Vector2(0f, 1f);

	[SkipSerialisation]
	public float UpperLimit
	{
		get
		{
			if (!AllowsOverflow)
			{
				return Limits.y;
			}
			return float.MaxValue;
		}
	}

	[SkipSerialisation]
	public float LowerLimit => Limits.x;

	[SkipSerialisation]
	public bool IsComputerColourCached => isComputedColourCached;

	[Obsolete]
	public float BloodAmount
	{
		get
		{
			return TotalLiquidAmount;
		}
		set
		{
			ForceSetAllLiquid(value);
		}
	}

	[SkipSerialisation]
	public virtual bool AllowPressureTransfer => true;

	public float AddLiquid(Liquid type, float amount)
	{
		if (amount <= float.Epsilon)
		{
			return 0f;
		}
		float num = Mathf.Clamp(TotalLiquidAmount + amount, LowerLimit, UpperLimit) - TotalLiquidAmount;
		if (float.IsNaN(num) || num <= float.Epsilon)
		{
			return 0f;
		}
		isComputedColourCached = false;
		TotalLiquidAmount += num;
		if (LiquidDistribution.TryGetValue(type, out var value))
		{
			value.Raw += num;
		}
		else
		{
			LiquidDistribution.Add(type, new RefFloat(num));
			OnLiquidEnter(type);
		}
		return num;
	}

	public float RemoveLiquid(Liquid type, float amount)
	{
		if (amount <= float.Epsilon)
		{
			return 0f;
		}
		float num = Mathf.Clamp(TotalLiquidAmount - amount, LowerLimit, UpperLimit) - TotalLiquidAmount;
		if (float.IsNaN(num) || num >= -1E-45f)
		{
			return 0f;
		}
		if (LiquidDistribution.TryGetValue(type, out var value))
		{
			if (value.Raw + num < 0f)
			{
				num = 0f - value.Raw;
			}
			TotalLiquidAmount += num;
			value.Raw += num;
		}
		isComputedColourCached = false;
		shouldDeleteEmptyEntries = true;
		return num;
	}

	public void Drain(float amount)
	{
		if (amount <= float.Epsilon || LiquidDistribution.Count == 0)
		{
			return;
		}
		float amount2 = amount / (float)LiquidDistribution.Count;
		foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
		{
			RemoveLiquid(item.Key, amount2);
		}
		shouldDeleteEmptyEntries = true;
	}

	public void DeleteEmptyLiquidEntries()
	{
		shouldDeleteEmptyEntries = false;
		foreach (Liquid item in Liquid.LiquidSet)
		{
			if (LiquidDistribution.TryGetValue(item, out var value))
			{
				if (value.Raw <= 0.02f && (value.Raw <= float.Epsilon || value.HasDecreased() || !value.HasChanged()))
				{
					TotalLiquidAmount -= value.Raw;
					OnLiquidExit(item);
					LiquidDistribution.Remove(item);
				}
				value.Previous = value.Raw;
			}
		}
	}

	public void ForceSetAllLiquid(float amount)
	{
		if (Mathf.Approximately(TotalLiquidAmount, 0f))
		{
			return;
		}
		amount = Mathf.Clamp(amount, LowerLimit, UpperLimit);
		float num = amount / TotalLiquidAmount;
		TotalLiquidAmount *= num;
		foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
		{
			item.Value.Raw *= num;
		}
		isComputedColourCached = false;
		shouldDeleteEmptyEntries = true;
	}

	public void ClearLiquid()
	{
		foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
		{
			OnLiquidExit(item.Key);
		}
		LiquidDistribution.Clear();
		TotalLiquidAmount = 0f;
	}

	public void ForceRecalculateTotalLiquidAmount()
	{
		TotalLiquidAmount = 0f;
		foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
		{
			TotalLiquidAmount += item.Value.Raw;
		}
	}

	public float TransferTo(Liquid liquid, float amount, BloodContainer target)
	{
		if (!base.enabled || !target.enabled || amount <= float.Epsilon)
		{
			return 0f;
		}
		float num = Mathf.Abs(RemoveLiquid(liquid, amount));
		if (num > float.Epsilon)
		{
			target.AddLiquid(liquid, num);
		}
		return num;
	}

	public void CopyTo(Liquid liquid, float amount, BloodContainer target)
	{
		if (base.enabled && target.enabled && !(amount <= float.Epsilon))
		{
			target.AddLiquid(liquid, amount);
		}
	}

	public float TransferTo(float amount, BloodContainer target, bool proportional = true)
	{
		if (!base.enabled || !target.enabled || amount <= float.Epsilon)
		{
			return 0f;
		}
		float totalLiquidAmount = TotalLiquidAmount;
		float num = 0f;
		foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
		{
			Liquid key = item.Key;
			float amount2 = (proportional ? (amount * (item.Value.Raw / totalLiquidAmount)) : amount);
			num += TransferTo(key, amount2, target);
		}
		return num;
	}

	public void CopyTo(float amount, BloodContainer target, bool proportional = true)
	{
		if (!base.enabled || !target.enabled || amount <= float.Epsilon)
		{
			return;
		}
		float totalLiquidAmount = TotalLiquidAmount;
		foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
		{
			Liquid key = item.Key;
			float num = (proportional ? (item.Value.Raw / totalLiquidAmount) : 1f);
			CopyTo(key, amount * num, target);
		}
	}

	public float GetAmount(Liquid liquid)
	{
		if (LiquidDistribution.TryGetValue(liquid, out var value))
		{
			return value.Raw;
		}
		return 0f;
	}

	public float GetPercentageOf(Liquid liquid)
	{
		if (AllowsOverflow)
		{
			return 0f;
		}
		return GetAmount(liquid) / UpperLimit;
	}

	protected virtual void Update()
	{
		timer += Time.deltaTime;
		if (AllowPressureTransfer)
		{
			MeasuredPressure = Mathf.Lerp(MeasuredPressure, CalculatePressureFromContents(), Utils.GetLerpFactorDeltaTime(0.6f, Time.deltaTime));
			switch (Pressure)
			{
			case PressureDirection.Push:
				MeasuredPressure = Mathf.Lerp(MeasuredPressure, 4096f, Utils.GetLerpFactorDeltaTime(0.95f, Time.deltaTime));
				break;
			case PressureDirection.Pull:
				MeasuredPressure = Mathf.Lerp(MeasuredPressure, -4096f, Utils.GetLerpFactorDeltaTime(0.95f, Time.deltaTime));
				break;
			}
		}
		else
		{
			float measuredPressure;
			switch (Pressure)
			{
			case PressureDirection.Push:
				measuredPressure = 4096f;
				break;
			case PressureDirection.Pull:
				measuredPressure = -4096f;
				break;
			default:
				measuredPressure = CalculatePressureFromContents();
				break;
			}
			MeasuredPressure = measuredPressure;
		}
		if (timer > 1f)
		{
			timer = 0f;
			DeleteEmptyLiquidEntries();
			int count = LiquidDistribution.Count;
			int num = 0;
			foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
			{
				liquidBuffer[num] = item.Key;
				num++;
			}
			for (int i = 0; i < count; i++)
			{
				liquidBuffer[i].OnUpdate(this);
			}
			for (int j = 0; j < LiquidMixingController.MixInstructions.Count; j++)
			{
				LiquidMixInstructions liquidMixInstructions = LiquidMixingController.MixInstructions[j];
				if (liquidMixInstructions == null || liquidMixInstructions.SourceLiquids == null || liquidMixInstructions.SourceLiquids.Length == 0 || liquidMixInstructions.TargetLiquid == null || !Liquid.HasLiquid(liquidMixInstructions.TargetLiquid))
				{
					continue;
				}
				bool flag = true;
				for (int k = 0; k < liquidMixInstructions.SourceLiquids.Length; k++)
				{
					if (!Liquid.HasLiquid(liquidMixInstructions.SourceLiquids[k]))
					{
						flag = false;
						break;
					}
				}
				if (flag && (liquidMixInstructions.ContainerFilter?.Invoke(this) ?? true))
				{
					Utils.LiquidMixProcess(this, liquidMixInstructions.SourceLiquids, liquidMixInstructions.TargetLiquid, liquidMixInstructions.RatePerSecond);
				}
			}
		}
		if (shouldDeleteEmptyEntries)
		{
			DeleteEmptyLiquidEntries();
		}
	}

	private float CalculatePressureFromContents()
	{
		return ScaledLiquidAmount;
	}

	public Color ForceCalculateComputedColor(Color fallback)
	{
		isComputedColourCached = false;
		return GetComputedColor(fallback);
	}

	public Color GetComputedColor()
	{
		return GetComputedColor(cachedComputedColour);
	}

	public Color GetComputedColor(Color fallback)
	{
		if (isComputedColourCached)
		{
			return cachedComputedColour;
		}
		if (LiquidDistribution.Count == 0)
		{
			return fallback;
		}
		if (Mathf.Approximately(0f, TotalLiquidAmount))
		{
			return fallback;
		}
		Vector4 vector = Color.clear;
		foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
		{
			float num = item.Value.Raw / TotalLiquidAmount;
			vector += (Vector4)item.Key.Color * num;
		}
		vector.w = Mathf.Clamp(vector.w, 0.1f, 1f);
		vector.x = Mathf.Clamp(vector.x, 0f, 10f);
		vector.y = Mathf.Clamp(vector.y, 0f, 10f);
		vector.z = Mathf.Clamp(vector.z, 0f, 10f);
		if (UserPreferenceManager.Current.GorelessMode)
		{
			Color color = vector;
			vector = Utils.ChangeRedToOrange(in color);
		}
		cachedComputedColour = vector;
		isComputedColourCached = true;
		return cachedComputedColour;
	}

	protected virtual void OnLiquidEnter(Liquid type)
	{
		OnDistributionChange?.Invoke();
		type.OnEnterContainer(this);
	}

	protected virtual void OnLiquidExit(Liquid type)
	{
		OnDistributionChange?.Invoke();
		type.OnExitContainer(this);
	}

	public virtual void OnBeforeSerialise()
	{
		SerialisableDistributions = new SerialisableDistribution[LiquidDistribution.Count];
		int num = 0;
		foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
		{
			SerialisableDistributions[num] = new SerialisableDistribution
			{
				Amount = item.Value.Raw,
				LiquidID = Liquid.GetIdentity(item.Key)
			};
			num++;
		}
	}

	public virtual void OnAfterDeserialise(List<GameObject> gameObjects)
	{
		TotalLiquidAmount = 0f;
		LiquidDistribution.Clear();
		SerialisableDistribution[] serialisableDistributions = SerialisableDistributions;
		for (int i = 0; i < serialisableDistributions.Length; i++)
		{
			SerialisableDistribution serialisableDistribution = serialisableDistributions[i];
			LiquidDistribution.Add(Liquid.GetLiquid(serialisableDistribution.LiquidID), new RefFloat(serialisableDistribution.Amount));
			TotalLiquidAmount += serialisableDistribution.Amount;
		}
	}

	public bool IsFull(float percentage = 0.999f)
	{
		return ScaledLiquidAmount >= percentage;
	}
}

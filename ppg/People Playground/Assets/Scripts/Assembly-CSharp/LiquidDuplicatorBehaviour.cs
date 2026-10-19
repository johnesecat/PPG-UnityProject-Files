using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidDuplicatorBehaviour : BloodContainer, Messages.IUse
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public AudioClip PeriodicNoise;

	[SkipSerialisation]
	public AudioClip FlushSound;

	[SkipSerialisation]
	public float Capacity = 1.7857144f;

	[SkipSerialisation]
	public SpriteRenderer EnabledLight;

	[SkipSerialisation]
	public float RateMultiplier = 1f;

	[SkipSerialisation]
	public LiquidContainerController LiquidContainer;

	[SkipSerialisation]
	public DamagableMachineryBehaviour DamagableMachinery;

	public bool Activated;

	private bool isBusy;

	private readonly FixedIntervalDistributor fixedInterval = new FixedIntervalDistributor
	{
		RateHz = 2f / 9f
	};

	public override Vector2 Limits => new Vector2(0f, Capacity);

	public override PressureDirection Pressure
	{
		get
		{
			if (!Activated || DamagableMachinery.Destroyed)
			{
				return PressureDirection.None;
			}
			return PressureDirection.Push;
		}
	}

	private void Start()
	{
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton(() => !isBusy, "flushLiquidentifier", "Flush", "Drain the container of its contents", delegate
		{
			StartCoroutine(FlushRoutine());
		}));
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled && !DamagableMachinery.Destroyed)
		{
			if (activation.Channel == 1)
			{
				TryFlush();
			}
			else
			{
				Activated = !Activated;
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		float num = RateMultiplier * (1f + PhysicalBehaviour.Charge * 0.1f) * Time.deltaTime * 60f;
		if (Activated && !DamagableMachinery.Destroyed && base.TotalLiquidAmount < Capacity)
		{
			for (int i = 0; i < fixedInterval.CalculateCycleCount(Time.deltaTime); i++)
			{
				PhysicalBehaviour.PlayClipOnce(PeriodicNoise, 0.1f);
			}
			foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
			{
				AddLiquid(item.Key, item.Value.Raw / Capacity * num);
			}
		}
		if (EnabledLight.enabled != Activated)
		{
			EnabledLight.enabled = Activated;
		}
		if ((bool)LiquidContainer)
		{
			LiquidContainer.FillPercentage = Mathf.Clamp01(ScaledLiquidAmount);
			LiquidContainer.Color = GetComputedColor();
		}
	}

	public void TryFlush()
	{
		StartCoroutine(FlushRoutine());
	}

	private IEnumerator FlushRoutine()
	{
		if (!isBusy)
		{
			isBusy = true;
			PhysicalBehaviour.PlayClipOnce(FlushSound);
			float startAmount = base.TotalLiquidAmount;
			for (float timeSpent = 0f; (base.TotalLiquidAmount >= float.Epsilon || timeSpent <= FlushSound.length) && timeSpent < 5f; timeSpent += Time.deltaTime)
			{
				Drain(startAmount * Time.deltaTime / 1.521f);
				yield return new WaitForEndOfFrame();
			}
			DeleteEmptyLiquidEntries();
			isBusy = false;
		}
	}
}

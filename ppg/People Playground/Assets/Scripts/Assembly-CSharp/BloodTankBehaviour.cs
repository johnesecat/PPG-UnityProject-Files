using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BloodTankBehaviour : BloodContainer, Messages.IUse
{
	public enum TankMode
	{
		Push,
		Pull,
		Idle,
		Drain
	}

	[SkipSerialisation]
	public ParticleSystem BloodDrainParticles;

	[SkipSerialisation]
	public ParticleSystem WaterPuffParticles;

	[SkipSerialisation]
	public FancyBloodSplatController PhysicalParticles;

	[SkipSerialisation]
	public TextMeshPro ModeDisplay;

	[SkipSerialisation]
	public LiquidContainerController LiquidContainer;

	public float DrainRate = 1f;

	public TankMode Mode = TankMode.Idle;

	private PhysicalBehaviour phys;

	public override bool AllowsOverflow => false;

	public override PressureDirection Pressure
	{
		get
		{
			switch (Mode)
			{
			case TankMode.Push:
				return PressureDirection.Push;
			case TankMode.Pull:
			case TankMode.Drain:
				return PressureDirection.Pull;
			case TankMode.Idle:
				return PressureDirection.None;
			default:
				return PressureDirection.None;
			}
		}
	}

	public override Vector2 Limits => new Vector2(0f, 14f);

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		PhysicalParticles.OnCollide += OnParticleCollide;
	}

	private void OnParticleCollide(Collider2D hit)
	{
		if (hit.TryGetComponent<BloodContainer>(out var component))
		{
			CopyTo(DrainRate * Time.deltaTime, component);
		}
	}

	private void Start()
	{
		List<ContextMenuButton> buttons = phys.ContextMenuOptions.Buttons;
		buttons.Add(new ContextMenuButton(() => Mode != TankMode.Push, "setToPush", "Set to push", "Set tank to push mode", delegate
		{
			Mode = TankMode.Push;
			UpdateActivation();
		}));
		buttons.Add(new ContextMenuButton(() => Mode != TankMode.Pull, "setToPull", "Set to pull", "Set tank to pull mode", delegate
		{
			Mode = TankMode.Pull;
			UpdateActivation();
		}));
		buttons.Add(new ContextMenuButton(() => Mode != TankMode.Idle, "setToIdle", "Set to idle", "Set tank to idle mode", delegate
		{
			Mode = TankMode.Idle;
			UpdateActivation();
		}));
		buttons.Add(new ContextMenuButton(() => Mode != TankMode.Drain, "setToDrain", "Set to drain", "Set tank to drain mode", delegate
		{
			Mode = TankMode.Drain;
			UpdateActivation();
		}));
		UpdateActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (activation.Channel != 1)
		{
			NextMode();
		}
		else
		{
			PreviousMode();
		}
		UpdateActivation();
	}

	private void NextMode()
	{
		int mode = (int)Mode;
		mode++;
		if (mode > 3)
		{
			mode = 0;
		}
		Mode = (TankMode)mode;
		UpdateActivation();
	}

	private void PreviousMode()
	{
		int mode = (int)Mode;
		mode--;
		if (mode < 0)
		{
			mode = 3;
		}
		Mode = (TankMode)mode;
		UpdateActivation();
	}

	private void UpdateActivation()
	{
		if (Mode == TankMode.Drain)
		{
			if (IsParticleSubmerged())
			{
				WaterPuffParticles.Play();
			}
			else
			{
				BloodDrainParticles.Play();
			}
		}
		else
		{
			BloodDrainParticles.Stop();
			WaterPuffParticles.Stop();
			PhysicalParticles.SpawningChance = 0f;
		}
		ModeDisplay.text = Mode.ToString().ToUpper();
	}

	private bool IsParticleSubmerged()
	{
		return WaterBehaviour.IsPointUnderWater(WaterPuffParticles.transform.position);
	}

	protected override void Update()
	{
		base.Update();
		LiquidContainer.FillPercentage = Mathf.Clamp01(ScaledLiquidAmount);
		LiquidContainer.Color = GetComputedColor();
		switch (Mode)
		{
		case TankMode.Push:
		case TankMode.Pull:
			PhysicalParticles.SpawningChance = 0f;
			BloodDrainParticles.Stop();
			WaterPuffParticles.Stop();
			break;
		case TankMode.Drain:
		{
			float amount = Time.deltaTime * DrainRate;
			Drain(amount);
			bool num = IsParticleSubmerged();
			if (num)
			{
				ParticleSystem.EmissionModule emission = WaterPuffParticles.emission;
				ParticleSystem.MainModule main = WaterPuffParticles.main;
				main.startColor = GetComputedColor();
				emission.rateOverTimeMultiplier = ((base.TotalLiquidAmount > 0.001f) ? 1 : 0);
				emission.rateOverDistanceMultiplier = ((base.TotalLiquidAmount > 0.001f) ? 3 : 0);
			}
			else
			{
				ParticleSystem.EmissionModule emission2 = BloodDrainParticles.emission;
				ParticleSystem.MainModule main2 = BloodDrainParticles.main;
				main2.startColor = GetComputedColor();
				main2.startSpeedMultiplier = ScaledLiquidAmount * 5f;
				emission2.rateOverTimeMultiplier = ((base.TotalLiquidAmount > 0.001f) ? 120 : 0);
				PhysicalParticles.DecalColourMultiplier = GetComputedColor();
				PhysicalParticles.EjectionForce = new Vector2(0.25f, 0.35f) * BloodDrainParticles.main.startSpeedMultiplier;
			}
			if (num)
			{
				if (BloodDrainParticles.isPlaying)
				{
					BloodDrainParticles.Stop();
				}
				if (!WaterPuffParticles.isPlaying)
				{
					WaterPuffParticles.Play();
				}
				PhysicalParticles.SpawningChance = 0f;
			}
			else
			{
				if (!BloodDrainParticles.isPlaying)
				{
					BloodDrainParticles.Play();
				}
				if (WaterPuffParticles.isPlaying)
				{
					WaterPuffParticles.Stop();
				}
				PhysicalParticles.SpawningChance = base.TotalLiquidAmount;
			}
			break;
		}
		}
	}

	private void OnDestroy()
	{
		PhysicalParticles.OnCollide -= OnParticleCollide;
	}
}

using System.Collections.Generic;
using UnityEngine;

public class CentrifugeBehaviour : BloodContainer, Messages.IUse
{
	[SkipSerialisation]
	public LiquidContainerController LiquidContainer;

	[SkipSerialisation]
	public VibrationBehaviour VibrationBehaviour;

	public bool Activated;

	[SkipSerialisation]
	public float VibrationIntensity = 2f;

	[SkipSerialisation]
	public float ParticleForceIntensity = 1f;

	[SkipSerialisation]
	public CosmeticRotationBehaviour Roer;

	[SkipSerialisation]
	public DamagableMachineryBehaviour DamagableMachinery;

	private float currentIntensity;

	[SkipSerialisation]
	public SpriteRenderer[] LiquidCountLights;

	[SkipSerialisation]
	public AudioSource CentrifugeLoopAudioSource;

	[SkipSerialisation]
	public SpriteRenderer EnabledLight;

	public override Vector2 Limits => new Vector2(0f, 7f);

	public override bool AllowsOverflow => false;

	private void Start()
	{
		UpdateActivation();
	}

	protected override void Update()
	{
		base.Update();
		if ((bool)LiquidContainer)
		{
			LiquidContainer.FillPercentage = Mathf.Clamp01(ScaledLiquidAmount);
			LiquidContainer.Color = GetComputedColor();
			Roer.ShouldRotate = Activated && !DamagableMachinery.Destroyed;
		}
	}

	private void FixedUpdate()
	{
		VibrationBehaviour.VibrateForce = VibrationIntensity * currentIntensity;
		if (Activated && !DamagableMachinery.Destroyed)
		{
			currentIntensity += Time.fixedDeltaTime;
			if ((bool)LiquidContainer)
			{
				LiquidContainer.AddForce(VibrationBehaviour.ForceVector * ParticleForceIntensity, 1f);
			}
			if (LiquidDistribution.Count > 1)
			{
				Liquid liquid = null;
				float num = float.MaxValue;
				foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution)
				{
					if (item.Value.Raw < num && num > 0f)
					{
						num = item.Value.Raw;
						liquid = item.Key;
					}
				}
				if (liquid != null && num > 0f)
				{
					RemoveLiquid(liquid, Time.fixedDeltaTime);
				}
			}
		}
		else
		{
			currentIntensity -= Time.fixedDeltaTime;
		}
		currentIntensity = Mathf.Clamp01(currentIntensity);
	}

	private void OnWillRenderObject()
	{
		for (int i = 0; i < LiquidCountLights.Length; i++)
		{
			LiquidCountLights[i].enabled = i < LiquidDistribution.Count;
		}
	}

	private void UpdateActivation()
	{
		EnabledLight.enabled = Activated;
		if (Activated)
		{
			CentrifugeLoopAudioSource.Play();
		}
		else
		{
			CentrifugeLoopAudioSource.Stop();
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}
}

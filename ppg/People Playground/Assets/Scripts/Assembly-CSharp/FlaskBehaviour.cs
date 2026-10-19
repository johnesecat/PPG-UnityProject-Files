using UnityEngine;

public class FlaskBehaviour : BloodContainer
{
	public float Capacity = 2.8f;

	[SkipSerialisation]
	public SerialisableDistribution StartLiquid;

	[SkipSerialisation]
	public LiquidContainerController LiquidContainer;

	public bool NewlySpawned = true;

	public override Vector2 Limits => new Vector2(0f, Capacity);

	public override bool AllowsOverflow => false;

	private void Start()
	{
		if (NewlySpawned && StartLiquid.Amount > float.Epsilon)
		{
			AddLiquid(Liquid.GetLiquid(StartLiquid.LiquidID), StartLiquid.Amount);
		}
		NewlySpawned = false;
	}

	protected override void Update()
	{
		base.Update();
		if ((bool)LiquidContainer)
		{
			LiquidContainer.FillPercentage = Mathf.Clamp01(ScaledLiquidAmount);
			LiquidContainer.Color = GetComputedColor();
		}
	}
}

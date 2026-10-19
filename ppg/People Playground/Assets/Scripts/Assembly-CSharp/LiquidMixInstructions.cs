using System;

public class LiquidMixInstructions
{
	public Liquid[] SourceLiquids;

	public Liquid TargetLiquid;

	public float RatePerSecond = 0.05f;

	public Func<BloodContainer, bool> ContainerFilter;

	public LiquidMixInstructions(Liquid[] sourceLiquids, Liquid targetLiquid, float ratePerSecond = 0.05f)
	{
		SourceLiquids = sourceLiquids;
		TargetLiquid = targetLiquid;
		RatePerSecond = ratePerSecond;
	}

	public LiquidMixInstructions(Liquid sourceA, Liquid sourceB, Liquid targetLiquid, float ratePerSecond = 0.05f)
	{
		SourceLiquids = new Liquid[2] { sourceA, sourceB };
		TargetLiquid = targetLiquid;
		RatePerSecond = ratePerSecond;
	}

	public LiquidMixInstructions(Liquid sourceA, Liquid sourceB, Liquid sourceC, Liquid targetLiquid, float ratePerSecond = 0.05f)
	{
		SourceLiquids = new Liquid[3] { sourceA, sourceB, sourceC };
		TargetLiquid = targetLiquid;
		RatePerSecond = ratePerSecond;
	}
}

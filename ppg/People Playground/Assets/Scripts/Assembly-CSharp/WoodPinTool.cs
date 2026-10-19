public class WoodPinTool : PinTool
{
	public override void OnFixedHold()
	{
	}

	public override void OnHold()
	{
	}

	public override void OnDeselect()
	{
	}

	public override void OnToolChosen()
	{
	}

	public override void OnToolUnchosen()
	{
	}

	protected override float GetBreakingThreshold()
	{
		return 6000f;
	}

	protected override void OnPinCreation(PinBehaviour pin)
	{
		pin.PinSpritePath = "Sprites/WoodPin";
	}
}

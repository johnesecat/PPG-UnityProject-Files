using UnityEngine;

public class LiquidFilterBehaviour : BloodContainer
{
	public override bool AllowsOverflow => false;

	public override bool AllowsTransfer => true;

	public override Vector2 Limits => new Vector2(0f, 28f);
}

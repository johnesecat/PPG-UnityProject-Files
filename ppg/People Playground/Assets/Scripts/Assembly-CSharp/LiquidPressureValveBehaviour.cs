public class LiquidPressureValveBehaviour : ValveBehaviour
{
	public override bool AllowsTransfer => false;

	public override bool AllowPressureTransfer => ValvePosition > 0.5f;
}

public class TimeToggle : Toggle
{
	protected override bool Visible => Global.main.Paused;
}

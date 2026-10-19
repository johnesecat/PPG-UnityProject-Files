public class ActiveSettingIfNotVsync : ConditionalSettingBehaviour
{
	public override bool GetShouldBeActive()
	{
		return !UserPreferenceManager.Current.VSync;
	}
}

using System;

public class ApplicationConfigBaseNonGeneric
{
	public static bool IsInitialized { get; protected set; }

	public static event EventHandler<EventArgs> SettingsInitialized;

	protected static void InvokeSettingsInitializedEvent()
	{
		ApplicationConfigBaseNonGeneric.SettingsInitialized?.Invoke(null, EventArgs.Empty);
	}
}

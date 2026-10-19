using System;

public class TemperatureSettingAttribute : Attribute
{
	public float ToPreference(float realValue)
	{
		return (float)Math.Round(Utils.CelsiusToPreference(realValue), 2);
	}

	public float ToCelsius(float userInput)
	{
		return (float)Math.Round(Utils.PreferenceToCelsius(userInput), 2);
	}
}

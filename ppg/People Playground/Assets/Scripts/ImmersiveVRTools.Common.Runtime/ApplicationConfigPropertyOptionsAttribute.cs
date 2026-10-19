using System;

public class ApplicationConfigPropertyOptionsAttribute : Attribute
{
	public string OptionallyOverrideWithCommandLineArgumentValue { get; set; }
}

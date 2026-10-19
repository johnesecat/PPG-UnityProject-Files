using System;

public class DynamicConfigValueSetterCommandLineArgInfoAttribute : Attribute
{
	public string Name { get; }

	public DynamicConfigValueSetterCommandLineArgInfoAttribute(string name)
	{
		Name = name;
	}
}

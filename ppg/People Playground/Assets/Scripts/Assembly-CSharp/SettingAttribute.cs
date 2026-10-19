using System;

public class SettingAttribute : Attribute
{
	public readonly SettingCategory Category;

	public readonly string Title;

	public readonly string Description;

	public SettingAttribute(SettingCategory category, string title, string desc)
	{
		Category = category;
		Title = title;
		Description = desc;
	}

	public SettingAttribute(SettingCategory category, string title)
	{
		Category = category;
		Title = title;
		Description = string.Empty;
	}
}

using System;
using System.Reflection;

public static class PreferenceExtensions
{
	public static float GetFloat(this Preferences instance, string key)
	{
		return instance.GetByName<float>(key);
	}

	public static string GetString(this Preferences instance, string key)
	{
		return instance.GetByName<string>(key);
	}

	public static void SetByName<T>(this Preferences instance, string name, T value)
	{
		GetField(typeof(Preferences), typeof(T), name).SetValue(instance, value);
	}

	public static T GetByName<T>(this Preferences instance, string name)
	{
		return (T)GetField(typeof(Preferences), typeof(T), name).GetValue(instance);
	}

	public static void SetByName(this Preferences instance, string name, object value)
	{
		GetField(typeof(Preferences), null, name).SetValue(instance, value);
	}

	public static object GetByName(this Preferences instance, string name)
	{
		return GetField(typeof(Preferences), null, name).GetValue(instance);
	}

	private static FieldInfo GetField(Type instanceType, Type fieldType, string fieldName)
	{
		FieldInfo field = instanceType.GetField(fieldName);
		if (fieldType != null && field.FieldType != fieldType)
		{
			throw new Exception("Attempt to get preference field with a type mismatch. Got " + fieldType.Name + ", expected " + field.FieldType.Name);
		}
		return field;
	}
}

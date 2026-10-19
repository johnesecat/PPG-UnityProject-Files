using System;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class DynamicConfigValueSetterFromFile : DynamicConfigValueBase
{
	[SerializeField]
	[HideInInspector]
	private string _configTypeName;

	[SerializeField]
	private string _configKeyName;

	protected override void SetConfigValueOnObjectInternal(Action<object> setOnObject)
	{
		MethodInfo method = ReflectionHelper.GetType(_configTypeName).GetMethod("Get", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		Action<object> action = delegate(dynamic settings)
		{
			object configValue = GetConfigValue((object)settings);
			setOnObject(configValue);
		};
		method.Invoke(null, new object[2] { this, action });
	}

	private object GetConfigValue(object settingsObject)
	{
		FieldInfo configKeyFieldInfo = GetConfigKeyFieldInfo();
		if (configKeyFieldInfo != null)
		{
			return configKeyFieldInfo.GetValue(settingsObject);
		}
		PropertyInfo configKeyPropertyInfo = GetConfigKeyPropertyInfo();
		if (configKeyPropertyInfo != null)
		{
			return configKeyPropertyInfo.GetValue(settingsObject);
		}
		throw new Exception("Unable to find field or property on config object");
	}

	private new void OnValidate()
	{
		if (!string.IsNullOrEmpty(_configTypeName) && !string.IsNullOrEmpty(_configKeyName) && GetConfigKeyFieldInfo() == null && GetConfigKeyPropertyInfo() == null)
		{
			throw new Exception("Unable to find config key: " + _configKeyName + ", for config type '" + _configTypeName + "'. Make sure you're using public fields or public properties.");
		}
		base.OnValidate();
	}

	private FieldInfo GetConfigKeyFieldInfo()
	{
		return ReflectionHelper.GetType(_configTypeName).GetField(_configKeyName);
	}

	private PropertyInfo GetConfigKeyPropertyInfo()
	{
		return ReflectionHelper.GetType(_configTypeName).GetProperty(_configKeyName);
	}
}

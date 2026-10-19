using System;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class DynamicConfigValueSetterFromCommandLine : DynamicConfigValueBase
{
	[SerializeField]
	[HideInInspector]
	private string _commandLineOptionsTypeName;

	[SerializeField]
	private string _commandLineOptionsClassPropertyName;

	[SerializeField]
	[ReadOnly]
	private string _commandLineArgName;

	[ContextMenu("TriggerSetConfigValueOnObjectFresh")]
	private void TriggerSetConfigValueOnObjectFresh()
	{
		ReflectionHelper.GetType(_commandLineOptionsTypeName).BaseType.GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, null);
		SetConfigValueOnObject();
	}

	protected override void SetConfigValueOnObjectInternal(Action<object> setOnObject)
	{
		object value = ReflectionHelper.GetType(_commandLineOptionsTypeName).GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).GetValue(null);
		object value2 = value.GetType().GetProperty(_commandLineOptionsClassPropertyName).GetValue(value);
		setOnObject(value2);
	}

	private new void OnValidate()
	{
		if (!string.IsNullOrEmpty(_commandLineOptionsTypeName) && !string.IsNullOrEmpty(_commandLineOptionsClassPropertyName))
		{
			PropertyInfo property = ReflectionHelper.GetType(_commandLineOptionsTypeName).GetProperty(_commandLineOptionsClassPropertyName);
			if (property == null)
			{
				throw new Exception("Unable to find config prop: " + _commandLineOptionsClassPropertyName + ", for config type '" + _commandLineOptionsTypeName + "'. Make sure you're using public fields or public properties.");
			}
			DynamicConfigValueSetterCommandLineArgInfoAttribute customAttribute = property.GetCustomAttribute<DynamicConfigValueSetterCommandLineArgInfoAttribute>();
			if (customAttribute == null)
			{
				throw new Exception("There's none DynamicConfigValueSetterCommandLineArgInfoAttribute on: " + _commandLineOptionsClassPropertyName + ", for config type '" + _commandLineOptionsTypeName + "'. Make sure to add.");
			}
			_commandLineArgName = customAttribute.Name;
		}
		base.OnValidate();
	}
}

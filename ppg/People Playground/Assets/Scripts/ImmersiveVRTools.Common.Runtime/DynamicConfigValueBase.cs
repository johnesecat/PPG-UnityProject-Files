using System;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public abstract class DynamicConfigValueBase : MonoBehaviour
{
	[SerializeField]
	private MonoBehaviour _setOnObject;

	[SerializeField]
	private string _setOnObjectFieldName;

	[SerializeField]
	private bool _disableComponentTillConfigValueSet;

	private void Awake()
	{
		if (!_setOnObject)
		{
			Debug.LogWarning("No _setOnObject for DynamicConfigValueBase. (this may not be intentional when running on server)");
			return;
		}
		if (_disableComponentTillConfigValueSet)
		{
			_setOnObject.enabled = false;
		}
		SetConfigValueOnObject();
	}

	protected abstract void SetConfigValueOnObjectInternal(Action<object> setOnObject);

	[ContextMenu("SetConfigValueOnObject")]
	protected void SetConfigValueOnObject()
	{
		SetConfigValueOnObjectInternal(delegate(object configValue)
		{
			Type type = _setOnObject.GetType();
			GetSetOnFieldInfo(type).SetValue(_setOnObject, configValue);
			if (_disableComponentTillConfigValueSet)
			{
				_setOnObject.enabled = true;
			}
		});
	}

	protected virtual void OnValidate()
	{
		if ((bool)_setOnObject)
		{
			Type type = _setOnObject.GetType();
			if (!string.IsNullOrEmpty(_setOnObjectFieldName) && GetSetOnFieldInfo(type) == null)
			{
				throw new Exception("Unable to find: " + _setOnObjectFieldName + ", " + $"for type '{type}'. " + "Make sure you're using fields.");
			}
		}
	}

	private FieldInfo GetSetOnFieldInfo(Type setOnObjectType)
	{
		return ReflectionHelper.GetFieldInfoIncludingBaseClasses(setOnObjectType, _setOnObjectFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
	}
}

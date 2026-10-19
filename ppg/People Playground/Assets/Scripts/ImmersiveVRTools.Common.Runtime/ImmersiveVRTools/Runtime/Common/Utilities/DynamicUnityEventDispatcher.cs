using System;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class DynamicUnityEventDispatcher : MonoBehaviour, ITriggerable
	{
		[SerializeField]
		[ReadOnly]
		private string _findComponentTypeName;

		[SerializeField]
		private DynamicUnityEventDispatcherFindMode _dynamicUnityEventDispatcherFindMode;

		[SerializeField]
		private string _methodNameToTrigger;

		[SerializeField]
		private bool _includeDisabledGameObjects;

		public string FindComponentTypeName => _findComponentTypeName;

		public string MethodNameToTrigger => _methodNameToTrigger;

		public string TriggerName => FindComponentTypeName + ":" + MethodNameToTrigger;

		private void Awake()
		{
			FindMethodToTrigger(ResolveComponentType());
		}

		[ContextMenu("Trigger")]
		public void Trigger()
		{
			Type type = ResolveComponentType();
			if (_dynamicUnityEventDispatcherFindMode == DynamicUnityEventDispatcherFindMode.FirstOfType)
			{
				UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(type);
				if (@object == null)
				{
					throw new Exception($"Unable to find component for type '{FindComponentTypeName}' using mode: '{_dynamicUnityEventDispatcherFindMode}'");
				}
				FindMethodToTrigger(type).Invoke(@object, null);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}

		private MethodInfo FindMethodToTrigger(Type componentType)
		{
			MethodInfo method = componentType.GetMethod(MethodNameToTrigger);
			if (method == null)
			{
				throw new Exception($"Unable to find method: {MethodNameToTrigger}, component type '{FindComponentTypeName}' using mode: '{_dynamicUnityEventDispatcherFindMode}'. Make sure it's public parameterless method.");
			}
			return method;
		}

		private void OnValidate()
		{
			if (!string.IsNullOrEmpty(FindComponentTypeName) && !string.IsNullOrEmpty(MethodNameToTrigger))
			{
				FindMethodToTrigger(ResolveComponentType());
			}
		}

		private Type ResolveComponentType()
		{
			return ReflectionHelper.GetType(FindComponentTypeName);
		}
	}
}

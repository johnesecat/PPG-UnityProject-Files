using System;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class DynamicApplicationConfigInitializer : MonoBehaviour
{
	private void Awake()
	{
		foreach (Type item in ReflectionHelper.GetAllInstantiableTypesDerivedFrom(typeof(ApplicationConfigBase<>)))
		{
			MethodInfo method = item.GetMethod("Get", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			object[] parameters = new MonoBehaviour[2] { this, null };
			method.Invoke(null, parameters);
		}
	}
}

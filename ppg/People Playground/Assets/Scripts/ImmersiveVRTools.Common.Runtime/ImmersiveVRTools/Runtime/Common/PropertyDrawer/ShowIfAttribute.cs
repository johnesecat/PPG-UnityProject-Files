using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.PropertyDrawer
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public class ShowIfAttribute : PropertyAttribute
	{
		public string CheckFieldName { get; }

		public ShowIfAttribute(string checkFieldName)
		{
			CheckFieldName = checkFieldName;
		}
	}
}

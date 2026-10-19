using System;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.PropertyDrawer
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public class NarrowObjectPickerAttribute : PropertyAttribute
	{
		private Type _toType;

		private static readonly Type DefaultTypeIfUnresolvedFromString = typeof(UnityEngine.Object);

		public Type ToType
		{
			get
			{
				if (_toType == null)
				{
					try
					{
						Type type = (_toType = ReflectionHelper.GetType(ToTypeName) ?? DefaultTypeIfUnresolvedFromString);
					}
					catch (Exception)
					{
						_toType = DefaultTypeIfUnresolvedFromString;
					}
				}
				return _toType;
			}
		}

		private string ToTypeName { get; }

		public NarrowObjectPickerAttribute(Type toType)
		{
			_toType = toType;
		}

		public NarrowObjectPickerAttribute(string toTypeName)
		{
			ToTypeName = toTypeName;
		}
	}
}

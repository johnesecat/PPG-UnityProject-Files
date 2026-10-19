using System;
using ImmersiveVRTools.Runtime.Common.Variable;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class ScriptableObjectReferenceFieldInitializer
	{
		public static bool TrySetIfNotAssigned<T, VariableT>(Reference<T, VariableT> so, Action<VariableT> setSo, string soName) where VariableT : Variable<T>
		{
			return EditorFieldInitializerGeneric<VariableT>.TrySetIfNotAssigned(so.Variable, setSo, soName, typeof(VariableT).Name);
		}
	}
}

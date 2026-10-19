using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;

namespace MonoMod.Utils
{
	public sealed class DMDEmitDynamicMethodGenerator : DMDGenerator<DMDEmitDynamicMethodGenerator>
	{
		private static readonly FieldInfo _DynamicMethod_returnType = typeof(DynamicMethod).GetField("returnType", BindingFlags.Instance | BindingFlags.NonPublic) ?? typeof(DynamicMethod).GetField("m_returnType", BindingFlags.Instance | BindingFlags.NonPublic);

		protected override MethodInfo _Generate(DynamicMethodDefinition dmd, object context)
		{
			MethodBase originalMethod = dmd.OriginalMethod;
			MethodDefinition definition = dmd.Definition;
			Type[] array;
			if (originalMethod != null)
			{
				ParameterInfo[] parameters = originalMethod.GetParameters();
				int num = 0;
				if (!originalMethod.IsStatic)
				{
					num++;
					array = new Type[parameters.Length + 1];
					array[0] = originalMethod.GetThisParamType();
				}
				else
				{
					array = new Type[parameters.Length];
				}
				for (int i = 0; i < parameters.Length; i++)
				{
					array[i + num] = parameters[i].ParameterType;
				}
			}
			else
			{
				int num2 = 0;
				if (definition.HasThis)
				{
					num2++;
					array = new Type[definition.Parameters.Count + 1];
					Type type2 = definition.DeclaringType.ResolveReflection();
					if (type2.IsValueType)
					{
						type2 = type2.MakeByRefType();
					}
					array[0] = type2;
				}
				else
				{
					array = new Type[definition.Parameters.Count];
				}
				for (int j = 0; j < definition.Parameters.Count; j++)
				{
					array[j + num2] = definition.Parameters[j].ParameterType.ResolveReflection();
				}
			}
			string text = dmd.Name ?? ("DMD<" + (originalMethod?.GetID(null, null, withType: true, proxyMethod: false, simple: true) ?? definition.GetID(null, null, withType: true, simple: true)) + ">");
			Type type3 = (originalMethod as MethodInfo)?.ReturnType ?? definition.ReturnType?.ResolveReflection();
			MMDbgLog.Log(string.Format("new DynamicMethod: {0} {1}({2})", type3, text, string.Join(",", array.Select((Type type) => type?.ToString()).ToArray())));
			if (originalMethod != null)
			{
				MMDbgLog.Log("orig: " + ((originalMethod as MethodInfo)?.ReturnType?.ToString() ?? "NULL") + " " + originalMethod.Name + "(" + string.Join(",", (from arg in originalMethod.GetParameters()
					select arg?.ParameterType?.ToString() ?? "NULL").ToArray()) + ")");
			}
			MMDbgLog.Log("mdef: " + (definition.ReturnType?.ToString() ?? "NULL") + " " + text + "(" + string.Join(",", definition.Parameters.Select((ParameterDefinition arg) => arg?.ParameterType?.ToString() ?? "NULL").ToArray()) + ")");
			DynamicMethod dynamicMethod = new DynamicMethod(text, typeof(void), array, originalMethod?.DeclaringType ?? dmd.OwnerType ?? typeof(DynamicMethodDefinition), skipVisibility: true);
			_DynamicMethod_returnType.SetValue(dynamicMethod, type3);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			_DMDEmit.Generate(dmd, dynamicMethod, iLGenerator);
			return dynamicMethod;
		}
	}
}

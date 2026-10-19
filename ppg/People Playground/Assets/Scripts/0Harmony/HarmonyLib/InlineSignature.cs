using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mono.Cecil;
using MonoMod.Utils;

namespace HarmonyLib
{
	public class InlineSignature : ICallSiteGenerator
	{
		public class ModifierType
		{
			public bool IsOptional;

			public Type Modifier;

			public object Type;

			public override string ToString()
			{
				string[] array = new string[6];
				Type type = Type as Type;
				array[0] = (((object)type != null) ? type.FullDescription() : Type?.ToString());
				array[1] = " mod";
				array[2] = (IsOptional ? "opt" : "req");
				array[3] = "(";
				array[4] = Modifier?.FullDescription();
				array[5] = ")";
				return string.Concat(array);
			}

			internal TypeReference ToTypeReference(ModuleDefinition module)
			{
				if (IsOptional)
				{
					return new OptionalModifierType(module.ImportReference(Modifier), GetTypeReference(module, Type));
				}
				return new RequiredModifierType(module.ImportReference(Modifier), GetTypeReference(module, Type));
			}
		}

		public bool HasThis { get; set; }

		public bool ExplicitThis { get; set; }

		public CallingConvention CallingConvention { get; set; } = CallingConvention.Winapi;


		public List<object> Parameters { get; set; } = new List<object>();


		public object ReturnType { get; set; } = typeof(void);


		public override string ToString()
		{
			Type type = ReturnType as Type;
			return (((object)type != null) ? type.FullDescription() : ReturnType?.ToString()) + " (" + Parameters.Join(delegate(object p)
			{
				Type type2 = p as Type;
				return ((object)type2 == null) ? p?.ToString() : type2.FullDescription();
			}) + ")";
		}

		internal static TypeReference GetTypeReference(ModuleDefinition module, object param)
		{
			Type type = param as Type;
			if ((object)type == null)
			{
				InlineSignature inlineSignature = param as InlineSignature;
				if (inlineSignature == null)
				{
					ModifierType modifierType = param as ModifierType;
					if (modifierType != null)
					{
						return modifierType.ToTypeReference(module);
					}
					throw new NotSupportedException($"Unsupported inline signature parameter type: {param} ({param?.GetType().FullDescription()})");
				}
				return inlineSignature.ToFunctionPointer(module);
			}
			return module.ImportReference(type);
		}

		CallSite ICallSiteGenerator.ToCallSite(ModuleDefinition module)
		{
			CallSite callSite = new CallSite(GetTypeReference(module, ReturnType))
			{
				HasThis = HasThis,
				ExplicitThis = ExplicitThis,
				CallingConvention = (MethodCallingConvention)((byte)CallingConvention - 1)
			};
			foreach (object parameter in Parameters)
			{
				callSite.Parameters.Add(new ParameterDefinition(GetTypeReference(module, parameter)));
			}
			return callSite;
		}

		private FunctionPointerType ToFunctionPointer(ModuleDefinition module)
		{
			FunctionPointerType functionPointerType = new FunctionPointerType
			{
				ReturnType = GetTypeReference(module, ReturnType),
				HasThis = HasThis,
				ExplicitThis = ExplicitThis,
				CallingConvention = (MethodCallingConvention)((byte)CallingConvention - 1)
			};
			foreach (object parameter in Parameters)
			{
				functionPointerType.Parameters.Add(new ParameterDefinition(GetTypeReference(module, parameter)));
			}
			return functionPointerType;
		}
	}
}

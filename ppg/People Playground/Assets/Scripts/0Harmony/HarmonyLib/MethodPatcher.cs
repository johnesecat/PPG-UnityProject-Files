using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal class MethodPatcher
	{
		private const string INSTANCE_PARAM = "__instance";

		private const string ORIGINAL_METHOD_PARAM = "__originalMethod";

		private const string ARGS_ARRAY_VAR = "__args";

		private const string RESULT_VAR = "__result";

		private const string STATE_VAR = "__state";

		private const string EXCEPTION_VAR = "__exception";

		private const string RUN_ORIGINAL_VAR = "__runOriginal";

		private const string PARAM_INDEX_PREFIX = "__";

		private const string INSTANCE_FIELD_PREFIX = "___";

		private readonly bool debug;

		private readonly MethodBase original;

		private readonly MethodBase source;

		private readonly List<MethodInfo> prefixes;

		private readonly List<MethodInfo> postfixes;

		private readonly List<MethodInfo> transpilers;

		private readonly List<MethodInfo> finalizers;

		private readonly int idx;

		private readonly bool useStructReturnBuffer;

		private readonly Type returnType;

		private readonly DynamicMethodDefinition patch;

		private readonly ILGenerator il;

		private readonly Emitter emitter;

		private static readonly MethodInfo m_GetMethodFromHandle1 = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[1] { typeof(RuntimeMethodHandle) });

		private static readonly MethodInfo m_GetMethodFromHandle2 = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[2]
		{
			typeof(RuntimeMethodHandle),
			typeof(RuntimeTypeHandle)
		});

		internal MethodPatcher(MethodBase original, MethodBase source, List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers, List<MethodInfo> finalizers, bool debug)
		{
			if ((object)original == null)
			{
				throw new ArgumentNullException("original");
			}
			this.debug = debug;
			this.original = original;
			this.source = source;
			this.prefixes = prefixes;
			this.postfixes = postfixes;
			this.transpilers = transpilers;
			this.finalizers = finalizers;
			Memory.MarkForNoInlining(original);
			if (debug)
			{
				FileLog.LogBuffered("### Patch: " + original.FullDescription());
				FileLog.FlushBuffer();
			}
			idx = prefixes.Count + postfixes.Count + finalizers.Count;
			useStructReturnBuffer = StructReturnBuffer.NeedsFix(original);
			if (debug && useStructReturnBuffer)
			{
				FileLog.Log("### Note: A buffer for the returned struct is used. That requires an extra IntPtr argument before the first real argument");
			}
			returnType = AccessTools.GetReturnedType(original);
			patch = CreateDynamicMethod(original, $"_Patch{idx}", debug);
			if (patch == null)
			{
				throw new Exception("Could not create replacement method");
			}
			il = patch.GetILGenerator();
			emitter = new Emitter(il, debug);
		}

		internal MethodInfo CreateReplacement(out Dictionary<int, CodeInstruction> finalInstructions)
		{
			LocalBuilder[] existingVariables = DeclareLocalVariables(il, source ?? original);
			Dictionary<string, LocalBuilder> privateVars = new Dictionary<string, LocalBuilder>();
			List<MethodInfo> list = prefixes.Union(postfixes).Union(finalizers).ToList();
			LocalBuilder localBuilder = null;
			if (idx > 0)
			{
				localBuilder = DeclareLocalVariable(returnType, isReturnValue: true);
				privateVars["__result"] = localBuilder;
			}
			LocalBuilder localBuilder2 = null;
			if (list.Any((MethodInfo fix) => fix.GetParameters().Any((ParameterInfo p) => p.Name == "__args")))
			{
				PrepareArgumentArray();
				localBuilder2 = il.DeclareLocal(typeof(object[]));
				emitter.Emit(OpCodes.Stloc, localBuilder2);
				privateVars["__args"] = localBuilder2;
			}
			Label? label = null;
			LocalBuilder localBuilder3 = null;
			bool flag = prefixes.Any((MethodInfo fix) => PrefixAffectsOriginal(fix));
			bool flag2 = list.Any((MethodInfo fix) => fix.GetParameters().Any((ParameterInfo p) => p.Name == "__runOriginal"));
			if (flag || flag2)
			{
				localBuilder3 = DeclareLocalVariable(typeof(bool));
				emitter.Emit(OpCodes.Ldc_I4_1);
				emitter.Emit(OpCodes.Stloc, localBuilder3);
				if (flag)
				{
					label = il.DefineLabel();
				}
			}
			list.ForEach(delegate(MethodInfo fix)
			{
				if ((object)fix.DeclaringType != null && !privateVars.ContainsKey(fix.DeclaringType.AssemblyQualifiedName))
				{
					(from patchParam in fix.GetParameters()
						where patchParam.Name == "__state"
						select patchParam).Do(delegate(ParameterInfo patchParam)
					{
						LocalBuilder value = DeclareLocalVariable(patchParam.ParameterType);
						privateVars[fix.DeclaringType.AssemblyQualifiedName] = value;
					});
				}
			});
			LocalBuilder local = null;
			if (finalizers.Any())
			{
				local = DeclareLocalVariable(typeof(bool));
				privateVars["__exception"] = DeclareLocalVariable(typeof(Exception));
				emitter.MarkBlockBefore(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock), out var _);
			}
			AddPrefixes(privateVars, localBuilder3);
			if (label.HasValue)
			{
				emitter.Emit(OpCodes.Ldloc, localBuilder3);
				emitter.Emit(OpCodes.Brfalse, label.Value);
			}
			MethodCopier methodCopier = new MethodCopier(source ?? original, il, existingVariables);
			methodCopier.SetArgumentShift(useStructReturnBuffer);
			methodCopier.SetDebugging(debug);
			foreach (MethodInfo transpiler in transpilers)
			{
				methodCopier.AddTranspiler(transpiler);
			}
			List<Label> list2 = new List<Label>();
			methodCopier.Finalize(emitter, list2, out var hasReturnCode);
			foreach (Label item in list2)
			{
				emitter.MarkLabel(item);
			}
			if (localBuilder != null && hasReturnCode)
			{
				emitter.Emit(OpCodes.Stloc, localBuilder);
			}
			if (label.HasValue)
			{
				emitter.MarkLabel(label.Value);
			}
			AddPostfixes(privateVars, localBuilder3, passthroughPatches: false);
			if (localBuilder != null && hasReturnCode)
			{
				emitter.Emit(OpCodes.Ldloc, localBuilder);
			}
			bool flag3 = AddPostfixes(privateVars, localBuilder3, passthroughPatches: true);
			bool num = finalizers.Any();
			if (num)
			{
				if (flag3)
				{
					emitter.Emit(OpCodes.Stloc, localBuilder);
					emitter.Emit(OpCodes.Ldloc, localBuilder);
				}
				AddFinalizers(privateVars, localBuilder3, catchExceptions: false);
				emitter.Emit(OpCodes.Ldc_I4_1);
				emitter.Emit(OpCodes.Stloc, local);
				Label label3 = il.DefineLabel();
				emitter.Emit(OpCodes.Ldloc, privateVars["__exception"]);
				emitter.Emit(OpCodes.Brfalse, label3);
				emitter.Emit(OpCodes.Ldloc, privateVars["__exception"]);
				emitter.Emit(OpCodes.Throw);
				emitter.MarkLabel(label3);
				emitter.MarkBlockBefore(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock), out var _);
				emitter.Emit(OpCodes.Stloc, privateVars["__exception"]);
				emitter.Emit(OpCodes.Ldloc, local);
				Label label5 = il.DefineLabel();
				emitter.Emit(OpCodes.Brtrue, label5);
				bool num2 = AddFinalizers(privateVars, localBuilder3, catchExceptions: true);
				emitter.MarkLabel(label5);
				Label label6 = il.DefineLabel();
				emitter.Emit(OpCodes.Ldloc, privateVars["__exception"]);
				emitter.Emit(OpCodes.Brfalse, label6);
				if (num2)
				{
					emitter.Emit(OpCodes.Rethrow);
				}
				else
				{
					emitter.Emit(OpCodes.Ldloc, privateVars["__exception"]);
					emitter.Emit(OpCodes.Throw);
				}
				emitter.MarkLabel(label6);
				emitter.MarkBlockAfter(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock));
				if (localBuilder != null)
				{
					emitter.Emit(OpCodes.Ldloc, localBuilder);
				}
			}
			if (useStructReturnBuffer)
			{
				LocalBuilder local2 = DeclareLocalVariable(returnType);
				emitter.Emit(OpCodes.Stloc, local2);
				emitter.Emit(original.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);
				emitter.Emit(OpCodes.Ldloc, local2);
				emitter.Emit(OpCodes.Stobj, returnType);
			}
			if (num || hasReturnCode)
			{
				emitter.Emit(OpCodes.Ret);
			}
			finalInstructions = emitter.GetInstructions();
			if (debug)
			{
				FileLog.LogBuffered("DONE");
				FileLog.LogBuffered("");
				FileLog.FlushBuffer();
			}
			return patch.Generate().Pin();
		}

		internal static DynamicMethodDefinition CreateDynamicMethod(MethodBase original, string suffix, bool debug)
		{
			if ((object)original == null)
			{
				throw new ArgumentNullException("original");
			}
			bool flag = StructReturnBuffer.NeedsFix(original);
			string text = (original.DeclaringType?.FullName ?? "GLOBALTYPE") + "." + original.Name + suffix;
			text = text.Replace("<>", "");
			ParameterInfo[] parameters = original.GetParameters();
			List<Type> list = new List<Type>();
			list.AddRange(parameters.Types());
			if (flag)
			{
				list.Insert(0, typeof(IntPtr));
			}
			if (!original.IsStatic)
			{
				if (AccessTools.IsStruct(original.DeclaringType))
				{
					list.Insert(0, original.DeclaringType.MakeByRefType());
				}
				else
				{
					list.Insert(0, original.DeclaringType);
				}
			}
			Type type = (flag ? typeof(void) : AccessTools.GetReturnedType(original));
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(text, type, list.ToArray())
			{
				OwnerType = original.DeclaringType
			};
			int num = ((!original.IsStatic) ? 1 : 0) + (flag ? 1 : 0);
			if (flag)
			{
				dynamicMethodDefinition.Definition.Parameters[(!original.IsStatic) ? 1 : 0].Name = "retbuf";
			}
			if (!original.IsStatic)
			{
				dynamicMethodDefinition.Definition.Parameters[0].Name = "this";
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterDefinition parameterDefinition = dynamicMethodDefinition.Definition.Parameters[i + num];
				parameterDefinition.Attributes = (Mono.Cecil.ParameterAttributes)parameters[i].Attributes;
				parameterDefinition.Name = parameters[i].Name;
			}
			if (debug)
			{
				List<string> list2 = list.Select((Type p) => p.FullDescription()).ToList();
				if (list.Count == dynamicMethodDefinition.Definition.Parameters.Count)
				{
					for (int j = 0; j < list.Count; j++)
					{
						List<string> list3 = list2;
						int index = j;
						list3[index] = list3[index] + " " + dynamicMethodDefinition.Definition.Parameters[j].Name;
					}
				}
				FileLog.Log("### Replacement: static " + type.FullDescription() + " " + (original.DeclaringType?.FullName ?? "GLOBALTYPE") + "::" + text + "(" + list2.Join() + ")");
			}
			return dynamicMethodDefinition;
		}

		internal static LocalBuilder[] DeclareLocalVariables(ILGenerator il, MethodBase member)
		{
			IList<LocalVariableInfo> list = member.GetMethodBody()?.LocalVariables;
			if (list == null)
			{
				return new LocalBuilder[0];
			}
			return list.Select((LocalVariableInfo lvi) => il.DeclareLocal(lvi.LocalType, lvi.IsPinned)).ToArray();
		}

		private LocalBuilder DeclareLocalVariable(Type type, bool isReturnValue = false)
		{
			if (type.IsByRef && !isReturnValue)
			{
				type = type.GetElementType();
			}
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			if (AccessTools.IsClass(type))
			{
				LocalBuilder localBuilder = il.DeclareLocal(type);
				emitter.Emit(OpCodes.Ldnull);
				emitter.Emit(OpCodes.Stloc, localBuilder);
				return localBuilder;
			}
			if (AccessTools.IsStruct(type))
			{
				LocalBuilder localBuilder2 = il.DeclareLocal(type);
				emitter.Emit(OpCodes.Ldloca, localBuilder2);
				emitter.Emit(OpCodes.Initobj, type);
				return localBuilder2;
			}
			if (AccessTools.IsValue(type))
			{
				LocalBuilder localBuilder3 = il.DeclareLocal(type);
				if (type == typeof(float))
				{
					emitter.Emit(OpCodes.Ldc_R4, 0f);
				}
				else if (type == typeof(double))
				{
					emitter.Emit(OpCodes.Ldc_R8, 0.0);
				}
				else if (type == typeof(long) || type == typeof(ulong))
				{
					emitter.Emit(OpCodes.Ldc_I8, 0L);
				}
				else
				{
					emitter.Emit(OpCodes.Ldc_I4, 0);
				}
				emitter.Emit(OpCodes.Stloc, localBuilder3);
				return localBuilder3;
			}
			return null;
		}

		private static OpCode LoadIndOpCodeFor(Type type)
		{
			if (type.IsEnum)
			{
				return OpCodes.Ldind_I4;
			}
			if (type == typeof(float))
			{
				return OpCodes.Ldind_R4;
			}
			if (type == typeof(double))
			{
				return OpCodes.Ldind_R8;
			}
			if (type == typeof(byte))
			{
				return OpCodes.Ldind_U1;
			}
			if (type == typeof(ushort))
			{
				return OpCodes.Ldind_U2;
			}
			if (type == typeof(uint))
			{
				return OpCodes.Ldind_U4;
			}
			if (type == typeof(ulong))
			{
				return OpCodes.Ldind_I8;
			}
			if (type == typeof(sbyte))
			{
				return OpCodes.Ldind_I1;
			}
			if (type == typeof(short))
			{
				return OpCodes.Ldind_I2;
			}
			if (type == typeof(int))
			{
				return OpCodes.Ldind_I4;
			}
			if (type == typeof(long))
			{
				return OpCodes.Ldind_I8;
			}
			return OpCodes.Ldind_Ref;
		}

		private static OpCode StoreIndOpCodeFor(Type type)
		{
			if (type.IsEnum)
			{
				return OpCodes.Stind_I4;
			}
			if (type == typeof(float))
			{
				return OpCodes.Stind_R4;
			}
			if (type == typeof(double))
			{
				return OpCodes.Stind_R8;
			}
			if (type == typeof(byte))
			{
				return OpCodes.Stind_I1;
			}
			if (type == typeof(ushort))
			{
				return OpCodes.Stind_I2;
			}
			if (type == typeof(uint))
			{
				return OpCodes.Stind_I4;
			}
			if (type == typeof(ulong))
			{
				return OpCodes.Stind_I8;
			}
			if (type == typeof(sbyte))
			{
				return OpCodes.Stind_I1;
			}
			if (type == typeof(short))
			{
				return OpCodes.Stind_I2;
			}
			if (type == typeof(int))
			{
				return OpCodes.Stind_I4;
			}
			if (type == typeof(long))
			{
				return OpCodes.Stind_I8;
			}
			return OpCodes.Stind_Ref;
		}

		private void InitializeOutParameter(int argIndex, Type type)
		{
			if (type.IsByRef)
			{
				type = type.GetElementType();
			}
			emitter.Emit(OpCodes.Ldarg, argIndex);
			if (AccessTools.IsStruct(type))
			{
				emitter.Emit(OpCodes.Initobj, type);
			}
			else if (AccessTools.IsValue(type))
			{
				if (type == typeof(float))
				{
					emitter.Emit(OpCodes.Ldc_R4, 0f);
					emitter.Emit(OpCodes.Stind_R4);
				}
				else if (type == typeof(double))
				{
					emitter.Emit(OpCodes.Ldc_R8, 0.0);
					emitter.Emit(OpCodes.Stind_R8);
				}
				else if (type == typeof(long))
				{
					emitter.Emit(OpCodes.Ldc_I8, 0L);
					emitter.Emit(OpCodes.Stind_I8);
				}
				else
				{
					emitter.Emit(OpCodes.Ldc_I4, 0);
					emitter.Emit(OpCodes.Stind_I4);
				}
			}
			else
			{
				emitter.Emit(OpCodes.Ldnull);
				emitter.Emit(OpCodes.Stind_Ref);
			}
		}

		private bool EmitOriginalBaseMethod()
		{
			MethodInfo methodInfo = original as MethodInfo;
			if ((object)methodInfo != null)
			{
				emitter.Emit(OpCodes.Ldtoken, methodInfo);
			}
			else
			{
				ConstructorInfo constructorInfo = original as ConstructorInfo;
				if ((object)constructorInfo == null)
				{
					return false;
				}
				emitter.Emit(OpCodes.Ldtoken, constructorInfo);
			}
			Type reflectedType = original.ReflectedType;
			if (reflectedType.IsGenericType)
			{
				emitter.Emit(OpCodes.Ldtoken, reflectedType);
			}
			emitter.Emit(OpCodes.Call, reflectedType.IsGenericType ? m_GetMethodFromHandle2 : m_GetMethodFromHandle1);
			return true;
		}

		private void EmitCallParameter(MethodInfo patch, Dictionary<string, LocalBuilder> variables, LocalBuilder runOriginalVariable, bool allowFirsParamPassthrough, out LocalBuilder tmpObjectVar, List<KeyValuePair<LocalBuilder, Type>> tmpBoxVars)
		{
			tmpObjectVar = null;
			bool flag = !original.IsStatic;
			ParameterInfo[] parameters = original.GetParameters();
			string[] originalParameterNames = parameters.Select((ParameterInfo p) => p.Name).ToArray();
			List<ParameterInfo> list = patch.GetParameters().ToList();
			if (allowFirsParamPassthrough && patch.ReturnType != typeof(void) && list.Count > 0 && list[0].ParameterType == patch.ReturnType)
			{
				list.RemoveRange(0, 1);
			}
			foreach (ParameterInfo item in list)
			{
				if (item.Name == "__originalMethod")
				{
					if (!EmitOriginalBaseMethod())
					{
						emitter.Emit(OpCodes.Ldnull);
					}
					continue;
				}
				if (item.Name == "__runOriginal")
				{
					if (runOriginalVariable != null)
					{
						emitter.Emit(OpCodes.Ldloc, runOriginalVariable);
					}
					else
					{
						emitter.Emit(OpCodes.Ldc_I4_0);
					}
					continue;
				}
				if (item.Name == "__instance")
				{
					if (original.IsStatic)
					{
						emitter.Emit(OpCodes.Ldnull);
						continue;
					}
					bool num = (object)original.DeclaringType != null && AccessTools.IsStruct(original.DeclaringType);
					bool isByRef = item.ParameterType.IsByRef;
					if (num == isByRef)
					{
						emitter.Emit(OpCodes.Ldarg_0);
					}
					if (num && !isByRef)
					{
						emitter.Emit(OpCodes.Ldarg_0);
						emitter.Emit(OpCodes.Ldobj, original.DeclaringType);
					}
					if (!num && isByRef)
					{
						emitter.Emit(OpCodes.Ldarga, 0);
					}
					continue;
				}
				if (item.Name == "__args")
				{
					if (variables.TryGetValue("__args", out var value))
					{
						emitter.Emit(OpCodes.Ldloc, value);
					}
					else
					{
						emitter.Emit(OpCodes.Ldnull);
					}
					continue;
				}
				if (item.Name.StartsWith("___", StringComparison.Ordinal))
				{
					string text = item.Name.Substring("___".Length);
					FieldInfo fieldInfo;
					if (text.All(char.IsDigit))
					{
						fieldInfo = AccessTools.DeclaredField(original.DeclaringType, int.Parse(text));
						if ((object)fieldInfo == null)
						{
							throw new ArgumentException("No field found at given index in class " + (original.DeclaringType?.AssemblyQualifiedName ?? "null"), text);
						}
					}
					else
					{
						fieldInfo = AccessTools.Field(original.DeclaringType, text);
						if ((object)fieldInfo == null)
						{
							throw new ArgumentException("No such field defined in class " + (original.DeclaringType?.AssemblyQualifiedName ?? "null"), text);
						}
					}
					if (fieldInfo.IsStatic)
					{
						emitter.Emit(item.ParameterType.IsByRef ? OpCodes.Ldsflda : OpCodes.Ldsfld, fieldInfo);
						continue;
					}
					emitter.Emit(OpCodes.Ldarg_0);
					emitter.Emit(item.ParameterType.IsByRef ? OpCodes.Ldflda : OpCodes.Ldfld, fieldInfo);
					continue;
				}
				if (item.Name == "__state")
				{
					OpCode opcode = (item.ParameterType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc);
					if (variables.TryGetValue(patch.DeclaringType?.AssemblyQualifiedName ?? "null", out var value2))
					{
						emitter.Emit(opcode, value2);
					}
					else
					{
						emitter.Emit(OpCodes.Ldnull);
					}
					continue;
				}
				if (item.Name == "__result")
				{
					Type returnedType = AccessTools.GetReturnedType(original);
					if (returnedType == typeof(void))
					{
						throw new Exception("Cannot get result from void method " + original.FullDescription());
					}
					Type type = item.ParameterType;
					if (type.IsByRef && !returnedType.IsByRef)
					{
						type = type.GetElementType();
					}
					if (!type.IsAssignableFrom(returnedType))
					{
						throw new Exception("Cannot assign method return type " + returnedType.FullName + " to __result type " + type.FullName + " for method " + original.FullDescription());
					}
					OpCode opcode2 = ((item.ParameterType.IsByRef && !returnedType.IsByRef) ? OpCodes.Ldloca : OpCodes.Ldloc);
					if (returnedType.IsValueType && item.ParameterType == typeof(object).MakeByRefType())
					{
						opcode2 = OpCodes.Ldloc;
					}
					emitter.Emit(opcode2, variables["__result"]);
					if (returnedType.IsValueType)
					{
						if (item.ParameterType == typeof(object))
						{
							emitter.Emit(OpCodes.Box, returnedType);
						}
						else if (item.ParameterType == typeof(object).MakeByRefType())
						{
							emitter.Emit(OpCodes.Box, returnedType);
							tmpObjectVar = il.DeclareLocal(typeof(object));
							emitter.Emit(OpCodes.Stloc, tmpObjectVar);
							emitter.Emit(OpCodes.Ldloca, tmpObjectVar);
						}
					}
					continue;
				}
				if (variables.TryGetValue(item.Name, out var value3))
				{
					OpCode opcode3 = (item.ParameterType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc);
					emitter.Emit(opcode3, value3);
					continue;
				}
				int result;
				if (item.Name.StartsWith("__", StringComparison.Ordinal))
				{
					if (!int.TryParse(item.Name.Substring("__".Length), out result))
					{
						throw new Exception("Parameter " + item.Name + " does not contain a valid index");
					}
					if (result < 0 || result >= parameters.Length)
					{
						throw new Exception($"No parameter found at index {result}");
					}
				}
				else
				{
					result = patch.GetArgumentIndex(originalParameterNames, item);
					if (result == -1)
					{
						HarmonyMethod mergedFromType = HarmonyMethodExtensions.GetMergedFromType(item.ParameterType);
						MethodType? methodType = mergedFromType.methodType;
						if (!methodType.HasValue)
						{
							mergedFromType.methodType = MethodType.Normal;
						}
						MethodInfo methodInfo = mergedFromType.GetOriginalMethod() as MethodInfo;
						if ((object)methodInfo != null)
						{
							ConstructorInfo constructor = item.ParameterType.GetConstructor(new Type[2]
							{
								typeof(object),
								typeof(IntPtr)
							});
							if ((object)constructor != null)
							{
								Type declaringType = original.DeclaringType;
								if (methodInfo.IsStatic)
								{
									emitter.Emit(OpCodes.Ldnull);
								}
								else
								{
									emitter.Emit(OpCodes.Ldarg_0);
									if (declaringType != null && declaringType.IsValueType)
									{
										emitter.Emit(OpCodes.Ldobj, declaringType);
										emitter.Emit(OpCodes.Box, declaringType);
									}
								}
								if (!methodInfo.IsStatic && !mergedFromType.nonVirtualDelegate)
								{
									emitter.Emit(OpCodes.Dup);
									emitter.Emit(OpCodes.Ldvirtftn, methodInfo);
								}
								else
								{
									emitter.Emit(OpCodes.Ldftn, methodInfo);
								}
								emitter.Emit(OpCodes.Newobj, constructor);
								continue;
							}
						}
						throw new Exception("Parameter \"" + item.Name + "\" not found in method " + original.FullDescription());
					}
				}
				Type parameterType = parameters[result].ParameterType;
				Type type2 = (parameterType.IsByRef ? parameterType.GetElementType() : parameterType);
				Type parameterType2 = item.ParameterType;
				Type type3 = (parameterType2.IsByRef ? parameterType2.GetElementType() : parameterType2);
				bool flag2 = !parameters[result].IsOut && !parameterType.IsByRef;
				bool flag3 = !item.IsOut && !parameterType2.IsByRef;
				bool flag4 = type2.IsValueType && !type3.IsValueType;
				int arg = result + (flag ? 1 : 0) + (useStructReturnBuffer ? 1 : 0);
				if (flag2 == flag3)
				{
					emitter.Emit(OpCodes.Ldarg, arg);
					if (flag4)
					{
						if (flag3)
						{
							emitter.Emit(OpCodes.Box, type2);
							continue;
						}
						emitter.Emit(OpCodes.Ldobj, type2);
						emitter.Emit(OpCodes.Box, type2);
						LocalBuilder localBuilder = il.DeclareLocal(type3);
						emitter.Emit(OpCodes.Stloc, localBuilder);
						emitter.Emit(OpCodes.Ldloca_S, localBuilder);
						tmpBoxVars.Add(new KeyValuePair<LocalBuilder, Type>(localBuilder, type2));
					}
				}
				else if (flag2 && !flag3)
				{
					if (flag4)
					{
						emitter.Emit(OpCodes.Ldarg, arg);
						emitter.Emit(OpCodes.Box, type2);
						LocalBuilder local = il.DeclareLocal(type3);
						emitter.Emit(OpCodes.Stloc, local);
						emitter.Emit(OpCodes.Ldloca_S, local);
					}
					else
					{
						emitter.Emit(OpCodes.Ldarga, arg);
					}
				}
				else
				{
					emitter.Emit(OpCodes.Ldarg, arg);
					if (flag4)
					{
						emitter.Emit(OpCodes.Ldobj, type2);
						emitter.Emit(OpCodes.Box, type2);
					}
					else if (type2.IsValueType)
					{
						emitter.Emit(OpCodes.Ldobj, type2);
					}
					else
					{
						emitter.Emit(LoadIndOpCodeFor(parameters[result].ParameterType));
					}
				}
			}
		}

		private static bool PrefixAffectsOriginal(MethodInfo fix)
		{
			if (fix.ReturnType == typeof(bool))
			{
				return true;
			}
			return fix.GetParameters().Any(delegate(ParameterInfo p)
			{
				string name = p.Name;
				Type parameterType = p.ParameterType;
				switch (name)
				{
				case "__instance":
					return false;
				case "__originalMethod":
					return false;
				case "__state":
					return false;
				default:
					if (p.IsOut || p.IsRetval)
					{
						return true;
					}
					if (parameterType.IsByRef)
					{
						return true;
					}
					if (!AccessTools.IsValue(parameterType) && !AccessTools.IsStruct(parameterType))
					{
						return true;
					}
					return false;
				}
			});
		}

		private void AddPrefixes(Dictionary<string, LocalBuilder> variables, LocalBuilder runOriginalVariable)
		{
			prefixes.Do(delegate(MethodInfo fix)
			{
				Label? label = (PrefixAffectsOriginal(fix) ? new Label?(il.DefineLabel()) : null);
				if (label.HasValue)
				{
					emitter.Emit(OpCodes.Ldloc, runOriginalVariable);
					emitter.Emit(OpCodes.Brfalse, label.Value);
				}
				List<KeyValuePair<LocalBuilder, Type>> list = new List<KeyValuePair<LocalBuilder, Type>>();
				EmitCallParameter(fix, variables, runOriginalVariable, allowFirsParamPassthrough: false, out var tmpObjectVar, list);
				emitter.Emit(OpCodes.Call, fix);
				if (fix.GetParameters().Any((ParameterInfo p) => p.Name == "__args"))
				{
					RestoreArgumentArray(variables);
				}
				if (tmpObjectVar != null)
				{
					emitter.Emit(OpCodes.Ldloc, tmpObjectVar);
					emitter.Emit(OpCodes.Unbox_Any, AccessTools.GetReturnedType(original));
					emitter.Emit(OpCodes.Stloc, variables["__result"]);
				}
				list.Do(delegate(KeyValuePair<LocalBuilder, Type> tmpBoxVar)
				{
					emitter.Emit(original.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);
					emitter.Emit(OpCodes.Ldloc, tmpBoxVar.Key);
					emitter.Emit(OpCodes.Unbox_Any, tmpBoxVar.Value);
					emitter.Emit(OpCodes.Stobj, tmpBoxVar.Value);
				});
				Type type = fix.ReturnType;
				if (type != typeof(void))
				{
					if (type != typeof(bool))
					{
						throw new Exception($"Prefix patch {fix} has not \"bool\" or \"void\" return type: {fix.ReturnType}");
					}
					emitter.Emit(OpCodes.Stloc, runOriginalVariable);
				}
				if (label.HasValue)
				{
					emitter.MarkLabel(label.Value);
					emitter.Emit(OpCodes.Nop);
				}
			});
		}

		private bool AddPostfixes(Dictionary<string, LocalBuilder> variables, LocalBuilder runOriginalVariable, bool passthroughPatches)
		{
			bool result = false;
			postfixes.Where((MethodInfo fix) => passthroughPatches == (fix.ReturnType != typeof(void))).Do(delegate(MethodInfo fix)
			{
				List<KeyValuePair<LocalBuilder, Type>> list = new List<KeyValuePair<LocalBuilder, Type>>();
				EmitCallParameter(fix, variables, runOriginalVariable, allowFirsParamPassthrough: true, out var tmpObjectVar, list);
				emitter.Emit(OpCodes.Call, fix);
				if (fix.GetParameters().Any((ParameterInfo p) => p.Name == "__args"))
				{
					RestoreArgumentArray(variables);
				}
				if (tmpObjectVar != null)
				{
					emitter.Emit(OpCodes.Ldloc, tmpObjectVar);
					emitter.Emit(OpCodes.Unbox_Any, AccessTools.GetReturnedType(original));
					emitter.Emit(OpCodes.Stloc, variables["__result"]);
				}
				list.Do(delegate(KeyValuePair<LocalBuilder, Type> tmpBoxVar)
				{
					emitter.Emit(original.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);
					emitter.Emit(OpCodes.Ldloc, tmpBoxVar.Key);
					emitter.Emit(OpCodes.Unbox_Any, tmpBoxVar.Value);
					emitter.Emit(OpCodes.Stobj, tmpBoxVar.Value);
				});
				if (fix.ReturnType != typeof(void))
				{
					ParameterInfo parameterInfo = fix.GetParameters().FirstOrDefault();
					if (parameterInfo == null || !(fix.ReturnType == parameterInfo.ParameterType))
					{
						if (parameterInfo != null)
						{
							throw new Exception($"Return type of pass through postfix {fix} does not match type of its first parameter");
						}
						throw new Exception($"Postfix patch {fix} must have a \"void\" return type");
					}
					result = true;
				}
			});
			return result;
		}

		private bool AddFinalizers(Dictionary<string, LocalBuilder> variables, LocalBuilder runOriginalVariable, bool catchExceptions)
		{
			bool rethrowPossible = true;
			finalizers.Do(delegate(MethodInfo fix)
			{
				if (catchExceptions)
				{
					emitter.MarkBlockBefore(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock), out var _);
				}
				List<KeyValuePair<LocalBuilder, Type>> list = new List<KeyValuePair<LocalBuilder, Type>>();
				EmitCallParameter(fix, variables, runOriginalVariable, allowFirsParamPassthrough: false, out var tmpObjectVar, list);
				emitter.Emit(OpCodes.Call, fix);
				if (fix.GetParameters().Any((ParameterInfo p) => p.Name == "__args"))
				{
					RestoreArgumentArray(variables);
				}
				if (tmpObjectVar != null)
				{
					emitter.Emit(OpCodes.Ldloc, tmpObjectVar);
					emitter.Emit(OpCodes.Unbox_Any, AccessTools.GetReturnedType(original));
					emitter.Emit(OpCodes.Stloc, variables["__result"]);
				}
				list.Do(delegate(KeyValuePair<LocalBuilder, Type> tmpBoxVar)
				{
					emitter.Emit(original.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);
					emitter.Emit(OpCodes.Ldloc, tmpBoxVar.Key);
					emitter.Emit(OpCodes.Unbox_Any, tmpBoxVar.Value);
					emitter.Emit(OpCodes.Stobj, tmpBoxVar.Value);
				});
				if (fix.ReturnType != typeof(void))
				{
					emitter.Emit(OpCodes.Stloc, variables["__exception"]);
					rethrowPossible = false;
				}
				if (catchExceptions)
				{
					emitter.MarkBlockBefore(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock), out var _);
					emitter.Emit(OpCodes.Pop);
					emitter.MarkBlockAfter(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock));
				}
			});
			return rethrowPossible;
		}

		private void PrepareArgumentArray()
		{
			ParameterInfo[] parameters = original.GetParameters();
			int num = 0;
			ParameterInfo[] array = parameters;
			foreach (ParameterInfo parameterInfo in array)
			{
				int argIndex = num++ + ((!original.IsStatic) ? 1 : 0);
				if (parameterInfo.IsOut || parameterInfo.IsRetval)
				{
					InitializeOutParameter(argIndex, parameterInfo.ParameterType);
				}
			}
			emitter.Emit(OpCodes.Ldc_I4, parameters.Length);
			emitter.Emit(OpCodes.Newarr, typeof(object));
			num = 0;
			int num2 = 0;
			array = parameters;
			foreach (ParameterInfo obj in array)
			{
				int arg = num++ + ((!original.IsStatic) ? 1 : 0);
				Type type = obj.ParameterType;
				bool isByRef = type.IsByRef;
				if (isByRef)
				{
					type = type.GetElementType();
				}
				emitter.Emit(OpCodes.Dup);
				emitter.Emit(OpCodes.Ldc_I4, num2++);
				emitter.Emit(OpCodes.Ldarg, arg);
				if (isByRef)
				{
					if (AccessTools.IsStruct(type))
					{
						emitter.Emit(OpCodes.Ldobj, type);
					}
					else
					{
						emitter.Emit(LoadIndOpCodeFor(type));
					}
				}
				if (type.IsValueType)
				{
					emitter.Emit(OpCodes.Box, type);
				}
				emitter.Emit(OpCodes.Stelem_Ref);
			}
		}

		private void RestoreArgumentArray(Dictionary<string, LocalBuilder> variables)
		{
			ParameterInfo[] parameters = original.GetParameters();
			int num = 0;
			int num2 = 0;
			ParameterInfo[] array = parameters;
			foreach (ParameterInfo obj in array)
			{
				int arg = num++ + ((!original.IsStatic) ? 1 : 0);
				Type parameterType = obj.ParameterType;
				if (parameterType.IsByRef)
				{
					parameterType = parameterType.GetElementType();
					emitter.Emit(OpCodes.Ldarg, arg);
					emitter.Emit(OpCodes.Ldloc, variables["__args"]);
					emitter.Emit(OpCodes.Ldc_I4, num2);
					emitter.Emit(OpCodes.Ldelem_Ref);
					if (parameterType.IsValueType)
					{
						emitter.Emit(OpCodes.Unbox_Any, parameterType);
						if (AccessTools.IsStruct(parameterType))
						{
							emitter.Emit(OpCodes.Stobj, parameterType);
						}
						else
						{
							emitter.Emit(StoreIndOpCodeFor(parameterType));
						}
					}
					else
					{
						emitter.Emit(OpCodes.Castclass, parameterType);
						emitter.Emit(OpCodes.Stind_Ref);
					}
				}
				num2++;
			}
		}
	}
}

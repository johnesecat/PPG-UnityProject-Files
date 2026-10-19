using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	public class DetourRuntimeNETPlatform : DetourRuntimeILPlatform
	{
		private static readonly object[] _NoArgs = new object[0];

		private static readonly Type _RTDynamicMethod = typeof(DynamicMethod).GetNestedType("RTDynamicMethod", BindingFlags.NonPublic);

		private static readonly FieldInfo _RTDynamicMethod_m_owner = _RTDynamicMethod?.GetField("m_owner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private static readonly FieldInfo _DynamicMethod_m_method = typeof(DynamicMethod).GetField("m_method", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _DynamicMethod_GetMethodDescriptor = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo _RuntimeMethodHandle_m_value = typeof(RuntimeMethodHandle).GetField("m_value", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _IRuntimeMethodInfo_get_Value = typeof(RuntimeMethodHandle).Assembly.GetType("System.IRuntimeMethodInfo")?.GetMethod("get_Value");

		private static readonly MethodInfo _RuntimeHelpers__CompileMethod = typeof(RuntimeHelpers).GetMethod("_CompileMethod", BindingFlags.Static | BindingFlags.NonPublic) ?? typeof(RuntimeHelpers).GetMethod("CompileMethod", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesIntPtr;

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo;

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal;

		private static IntPtr ThePreStub;

		public override bool OnMethodCompiledWillBeCalled => false;

		public override event OnMethodCompiledEvent OnMethodCompiled;

		public override MethodBase GetIdentifiable(MethodBase method)
		{
			if (_RTDynamicMethod_m_owner != null && method.GetType() == _RTDynamicMethod)
			{
				return (MethodBase)_RTDynamicMethod_m_owner.GetValue(method);
			}
			return base.GetIdentifiable(method);
		}

		protected override RuntimeMethodHandle GetMethodHandle(MethodBase method)
		{
			DynamicMethod dynamicMethod = method as DynamicMethod;
			if ((object)dynamicMethod != null)
			{
				if (_RuntimeHelpers__CompileMethod_TakesIntPtr)
				{
					_RuntimeHelpers__CompileMethod.Invoke(null, new object[1] { ((RuntimeMethodHandle)_DynamicMethod_GetMethodDescriptor.Invoke(dynamicMethod, _NoArgs)).Value });
				}
				else if (_RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo)
				{
					_RuntimeHelpers__CompileMethod.Invoke(null, new object[1] { _RuntimeMethodHandle_m_value.GetValue((RuntimeMethodHandle)_DynamicMethod_GetMethodDescriptor.Invoke(dynamicMethod, _NoArgs)) });
				}
				else if (_RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal)
				{
					_RuntimeHelpers__CompileMethod.Invoke(null, new object[1] { _IRuntimeMethodInfo_get_Value.Invoke(_RuntimeMethodHandle_m_value.GetValue((RuntimeMethodHandle)_DynamicMethod_GetMethodDescriptor.Invoke(dynamicMethod, _NoArgs)), null) });
				}
				else
				{
					try
					{
						dynamicMethod.CreateDelegate(typeof(MulticastDelegate));
					}
					catch
					{
					}
				}
				if (_DynamicMethod_m_method != null)
				{
					return (RuntimeMethodHandle)_DynamicMethod_m_method.GetValue(method);
				}
				if (_DynamicMethod_GetMethodDescriptor != null)
				{
					return (RuntimeMethodHandle)_DynamicMethod_GetMethodDescriptor.Invoke(method, _NoArgs);
				}
			}
			return method.MethodHandle;
		}

		protected override void DisableInlining(MethodBase method, RuntimeMethodHandle handle)
		{
		}

		protected unsafe override IntPtr GetFunctionPointer(MethodBase method, RuntimeMethodHandle handle)
		{
			MMDbgLog.Log("mets: " + method.GetID());
			MMDbgLog.Log($"meth: 0x{(long)handle.Value:X16}");
			MMDbgLog.Log($"getf: 0x{(long)handle.GetFunctionPointer():X16}");
			bool flag = false;
			bool wasPreStub;
			IntPtr intPtr;
			while (true)
			{
				if (method.IsVirtual && (method.DeclaringType?.IsValueType ?? false))
				{
					MMDbgLog.Log($"ldfn: 0x{(long)method.GetLdftnPointer():X16}");
					bool flag2 = false;
					Type[] interfaces = method.DeclaringType.GetInterfaces();
					foreach (Type interfaceType in interfaces)
					{
						if (method.DeclaringType.GetInterfaceMap(interfaceType).TargetMethods.Contains(method))
						{
							flag2 = true;
							break;
						}
					}
					intPtr = method.GetLdftnPointer();
					if (!flag2)
					{
						return intPtr;
					}
				}
				else
				{
					intPtr = base.GetFunctionPointer(method, handle);
				}
				if (PlatformHelper.Is(Platform.ARM))
				{
					if (IntPtr.Size == 4)
					{
						break;
					}
					int num = 0;
					wasPreStub = false;
					IntPtr intPtr2 = WalkPrecode(intPtr);
					if (wasPreStub)
					{
						PrepareMethod(method, handle);
						continue;
					}
					while (intPtr2 != intPtr && num < 16)
					{
						num++;
						intPtr = intPtr2;
						wasPreStub = false;
						intPtr2 = WalkPrecode(intPtr);
						if (!wasPreStub)
						{
							continue;
						}
						goto IL_0164;
					}
					break;
				}
				if (IntPtr.Size == 4)
				{
					int num2 = (int)intPtr;
					if (*(byte*)num2 == 184 && *(byte*)(num2 + 5) == 144 && *(byte*)(num2 + 6) == 232 && *(byte*)(num2 + 11) == 233)
					{
						int num3 = num2 + 11;
						int num4 = *(int*)(num3 + 1) + (num3 + 1 + 4);
						intPtr = NotThePreStub(intPtr, (IntPtr)num4, out wasPreStub);
						if (wasPreStub)
						{
							PrepareMethod(method, handle);
							continue;
						}
						MMDbgLog.Log($"ngen: 0x{(long)intPtr:X8}");
					}
					num2 = (int)intPtr;
					if (*(byte*)num2 != 233 || *(byte*)(num2 + 5) != 95)
					{
						break;
					}
					int num5 = num2;
					int num6 = *(int*)(num5 + 1) + (num5 + 1 + 4);
					intPtr = NotThePreStub(intPtr, (IntPtr)num6, out wasPreStub);
					if (wasPreStub)
					{
						PrepareMethod(method, handle);
						continue;
					}
					MMDbgLog.Log($"ngen: 0x{(int)intPtr:X8}");
					break;
				}
				long num7 = (long)intPtr;
				if (*(uint*)num7 == 1959363912 && *(uint*)(num7 + 5) == 1224837960 && *(uint*)(num7 + 18) == 1958886217 && *(ushort*)(num7 + 23) == 47176)
				{
					intPtr = NotThePreStub(intPtr, (IntPtr)(*(long*)(num7 + 25)), out wasPreStub);
					if (!wasPreStub)
					{
						MMDbgLog.Log($"ngen: 0x{(long)intPtr:X16}");
						return intPtr;
					}
					PrepareMethod(method, handle);
				}
				else if (*(byte*)num7 == 233 && *(byte*)(num7 + 5) == 95)
				{
					long num8 = num7;
					long num9 = *(int*)(num8 + 1) + (num8 + 1 + 4);
					intPtr = NotThePreStub(intPtr, (IntPtr)num9, out wasPreStub);
					if (wasPreStub)
					{
						PrepareMethod(method, handle);
						continue;
					}
					int num10 = 0;
					while (true)
					{
						if (num10 < 16)
						{
							num7 = (long)intPtr + num10;
							if (*(ushort*)num7 == 47176 && *(ushort*)(num7 + 10) == 57599)
							{
								num9 = *(long*)(num7 + 2);
								intPtr = NotThePreStub(intPtr, (IntPtr)num9, out wasPreStub);
								if (wasPreStub)
								{
									PrepareMethod(method, handle);
									break;
								}
								num10 = -1;
							}
							else if ((*(ushort*)num7 & 0xFFF0) == 47168 && (*(uint*)(num7 + 10) & 0xF0FFFF) == 65382 && *(ushort*)(num7 + 13) == 34063 && (*(byte*)num7 & 0xF) == (*(byte*)(num7 + 12) & 0xF))
							{
								num8 = num7;
								num9 = *(int*)(num8 + 13 + 2) + (num8 + 13 + 2 + 4);
								intPtr = NotThePreStub(intPtr, (IntPtr)num9, out wasPreStub);
								if (wasPreStub)
								{
									PrepareMethod(method, handle);
									break;
								}
								num10 = -1;
							}
							num10++;
							continue;
						}
						MMDbgLog.Log($"ngen: 0x{(long)intPtr:X16}");
						return intPtr;
					}
				}
				else
				{
					if (*(byte*)num7 != 232 || flag)
					{
						break;
					}
					MMDbgLog.Log("Method thunk reset; regenerating");
					flag = true;
					long num11 = *(int*)(num7 + 1) + (num7 + 1 + 4);
					MMDbgLog.Log($"PrecodeFixupThunk: 0x{num11:X16}");
					PrepareMethod(method, handle);
				}
				continue;
				IL_0164:
				PrepareMethod(method, handle);
			}
			return intPtr;
			unsafe IntPtr WalkPrecode(IntPtr curr)
			{
				long num12 = (long)curr;
				if (*(uint*)num12 == 268435593 && *(uint*)(num12 + 4) == 2839556394u && *(uint*)(num12 + 8) == 3592356160u)
				{
					IntPtr ptrParsed = *(IntPtr*)(num12 + 16);
					return NotThePreStub(curr, ptrParsed, out wasPreStub);
				}
				if (*(uint*)num12 == 268435595 && *(uint*)(num12 + 4) == 2839556458u && *(uint*)(num12 + 8) == 3592356160u)
				{
					IntPtr ptrParsed2 = *(IntPtr*)(num12 + 16);
					return NotThePreStub(curr, ptrParsed2, out wasPreStub);
				}
				if (*(uint*)num12 == 268435468 && *(uint*)(num12 + 4) == 1476395115 && *(uint*)(num12 + 8) == 3592356192u)
				{
					IntPtr ptrParsed3 = *(IntPtr*)(num12 + 16);
					return NotThePreStub(curr, ptrParsed3, out wasPreStub);
				}
				if (*(uint*)num12 == 2432696336u && *(uint*)(num12 + 4) == 2432696352u && *(uint*)(num12 + 8) == 2432696833u && *(uint*)(num12 + 12) == 1476395120 && *(uint*)(num12 + 16) == 3592356352u)
				{
					IntPtr ptrParsed4 = *(IntPtr*)(num12 + 24);
					return NotThePreStub(curr, ptrParsed4, out wasPreStub);
				}
				return curr;
			}
		}

		private IntPtr NotThePreStub(IntPtr ptrGot, IntPtr ptrParsed, out bool wasPreStub)
		{
			if (ThePreStub == IntPtr.Zero)
			{
				ThePreStub = (IntPtr)(-2);
				MethodInfo methodInfo = typeof(HttpWebRequest).Assembly.GetType("System.Net.Connection")?.GetMethod("SubmitRequest", BindingFlags.Instance | BindingFlags.NonPublic);
				if (methodInfo != null)
				{
					ThePreStub = GetNativeStart(methodInfo);
					MMDbgLog.Log($"ThePreStub: 0x{(long)ThePreStub:X16}");
				}
				else if (PlatformHelper.Is(Platform.Windows))
				{
					ThePreStub = (IntPtr)(-1);
				}
			}
			wasPreStub = ptrParsed == ThePreStub;
			if (!wasPreStub)
			{
				return ptrParsed;
			}
			return ptrGot;
		}

		static DetourRuntimeNETPlatform()
		{
			MethodInfo runtimeHelpers__CompileMethod = _RuntimeHelpers__CompileMethod;
			_RuntimeHelpers__CompileMethod_TakesIntPtr = (((object)runtimeHelpers__CompileMethod != null) ? runtimeHelpers__CompileMethod.GetParameters()[0].ParameterType.FullName : null) == "System.IntPtr";
			MethodInfo runtimeHelpers__CompileMethod2 = _RuntimeHelpers__CompileMethod;
			_RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo = (((object)runtimeHelpers__CompileMethod2 != null) ? runtimeHelpers__CompileMethod2.GetParameters()[0].ParameterType.FullName : null) == "System.IRuntimeMethodInfo";
			MethodInfo runtimeHelpers__CompileMethod3 = _RuntimeHelpers__CompileMethod;
			_RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal = (((object)runtimeHelpers__CompileMethod3 != null) ? runtimeHelpers__CompileMethod3.GetParameters()[0].ParameterType.FullName : null) == "System.RuntimeMethodHandleInternal";
			ThePreStub = IntPtr.Zero;
		}
	}
}

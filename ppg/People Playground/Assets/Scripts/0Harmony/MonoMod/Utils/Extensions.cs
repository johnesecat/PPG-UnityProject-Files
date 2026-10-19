using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace MonoMod.Utils
{
	public static class Extensions
	{
		private static readonly Type t_Code = typeof(Code);

		private static readonly Type t_OpCodes = typeof(Mono.Cecil.Cil.OpCodes);

		private static readonly Dictionary<int, Mono.Cecil.Cil.OpCode> _ToLongOp = new Dictionary<int, Mono.Cecil.Cil.OpCode>();

		private static readonly Dictionary<int, Mono.Cecil.Cil.OpCode> _ToShortOp = new Dictionary<int, Mono.Cecil.Cil.OpCode>();

		private static readonly object[] _NoArgs = new object[0];

		private static readonly Dictionary<Type, FieldInfo> fmap_mono_assembly = new Dictionary<Type, FieldInfo>();

		private static readonly bool _MonoAssemblyNameHasArch = new AssemblyName("Dummy, ProcessorArchitecture=MSIL").ProcessorArchitecture == ProcessorArchitecture.MSIL;

		private static readonly Type _RTDynamicMethod = typeof(DynamicMethod).GetNestedType("RTDynamicMethod", BindingFlags.Public | BindingFlags.NonPublic);

		private static readonly Type t_ParamArrayAttribute = typeof(ParamArrayAttribute);

		private static readonly FieldInfo f_GenericParameter_position = typeof(GenericParameter).GetField("position", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo f_GenericParameter_type = typeof(GenericParameter).GetField("type", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly Dictionary<Type, int> _GetManagedSizeCache = new Dictionary<Type, int> { 
		{
			typeof(void),
			0
		} };

		private static MethodInfo _GetManagedSizeHelper;

		private static readonly Dictionary<MethodBase, Func<IntPtr>> _GetLdftnPointerCache = new Dictionary<MethodBase, Func<IntPtr>>();

		public static TypeDefinition SafeResolve(this TypeReference r)
		{
			try
			{
				return r.Resolve();
			}
			catch
			{
				return null;
			}
		}

		public static FieldDefinition SafeResolve(this FieldReference r)
		{
			try
			{
				return r.Resolve();
			}
			catch
			{
				return null;
			}
		}

		public static MethodDefinition SafeResolve(this MethodReference r)
		{
			try
			{
				return r.Resolve();
			}
			catch
			{
				return null;
			}
		}

		public static PropertyDefinition SafeResolve(this PropertyReference r)
		{
			try
			{
				return r.Resolve();
			}
			catch
			{
				return null;
			}
		}

		public static CustomAttribute GetCustomAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
		{
			if (cap == null || !cap.HasCustomAttributes)
			{
				return null;
			}
			foreach (CustomAttribute customAttribute in cap.CustomAttributes)
			{
				if (customAttribute.AttributeType.FullName == attribute)
				{
					return customAttribute;
				}
			}
			return null;
		}

		public static bool HasCustomAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
		{
			return cap.GetCustomAttribute(attribute) != null;
		}

		public static int GetInt(this Instruction instr)
		{
			Mono.Cecil.Cil.OpCode opCode = instr.OpCode;
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_M1)
			{
				return -1;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_0)
			{
				return 0;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_1)
			{
				return 1;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_2)
			{
				return 2;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_3)
			{
				return 3;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_4)
			{
				return 4;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_5)
			{
				return 5;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_6)
			{
				return 6;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_7)
			{
				return 7;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_8)
			{
				return 8;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_S)
			{
				return (sbyte)instr.Operand;
			}
			return (int)instr.Operand;
		}

		public static int? GetIntOrNull(this Instruction instr)
		{
			Mono.Cecil.Cil.OpCode opCode = instr.OpCode;
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_M1)
			{
				return -1;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_0)
			{
				return 0;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_1)
			{
				return 1;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_2)
			{
				return 2;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_3)
			{
				return 3;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_4)
			{
				return 4;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_5)
			{
				return 5;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_6)
			{
				return 6;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_7)
			{
				return 7;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_8)
			{
				return 8;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_S)
			{
				return (sbyte)instr.Operand;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4)
			{
				return (int)instr.Operand;
			}
			return null;
		}

		public static bool IsBaseMethodCall(this Mono.Cecil.Cil.MethodBody body, MethodReference called)
		{
			MethodDefinition method = body.Method;
			if (called == null)
			{
				return false;
			}
			TypeReference typeReference = called.DeclaringType;
			while (typeReference is TypeSpecification)
			{
				typeReference = ((TypeSpecification)typeReference).ElementType;
			}
			string patchFullName = typeReference.GetPatchFullName();
			bool flag = false;
			try
			{
				TypeDefinition typeDefinition = method.DeclaringType;
				while ((typeDefinition = typeDefinition.BaseType?.SafeResolve()) != null)
				{
					if (typeDefinition.GetPatchFullName() == patchFullName)
					{
						flag = true;
						break;
					}
				}
			}
			catch
			{
				flag = method.DeclaringType.GetPatchFullName() == patchFullName;
			}
			if (!flag)
			{
				return false;
			}
			return true;
		}

		public static bool IsCallvirt(this MethodReference method)
		{
			if (!method.HasThis)
			{
				return false;
			}
			if (method.DeclaringType.IsValueType)
			{
				return false;
			}
			return true;
		}

		public static bool IsStruct(this TypeReference type)
		{
			if (!type.IsValueType)
			{
				return false;
			}
			if (type.IsPrimitive)
			{
				return false;
			}
			return true;
		}

		public static Mono.Cecil.Cil.OpCode ToLongOp(this Mono.Cecil.Cil.OpCode op)
		{
			string name = Enum.GetName(t_Code, op.Code);
			if (!name.EndsWith("_S", StringComparison.Ordinal))
			{
				return op;
			}
			lock (_ToLongOp)
			{
				if (_ToLongOp.TryGetValue((int)op.Code, out var value))
				{
					return value;
				}
				return _ToLongOp[(int)op.Code] = ((Mono.Cecil.Cil.OpCode?)t_OpCodes.GetField(name.Substring(0, name.Length - 2))?.GetValue(null)) ?? op;
			}
		}

		public static Mono.Cecil.Cil.OpCode ToShortOp(this Mono.Cecil.Cil.OpCode op)
		{
			string name = Enum.GetName(t_Code, op.Code);
			if (name.EndsWith("_S", StringComparison.Ordinal))
			{
				return op;
			}
			lock (_ToShortOp)
			{
				if (_ToShortOp.TryGetValue((int)op.Code, out var value))
				{
					return value;
				}
				return _ToShortOp[(int)op.Code] = ((Mono.Cecil.Cil.OpCode?)t_OpCodes.GetField(name + "_S")?.GetValue(null)) ?? op;
			}
		}

		public static void RecalculateILOffsets(this MethodDefinition method)
		{
			if (method.HasBody)
			{
				int num = 0;
				for (int i = 0; i < method.Body.Instructions.Count; i++)
				{
					Instruction instruction = method.Body.Instructions[i];
					instruction.Offset = num;
					num += instruction.GetSize();
				}
			}
		}

		public static void FixShortLongOps(this MethodDefinition method)
		{
			if (!method.HasBody)
			{
				return;
			}
			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instruction = method.Body.Instructions[i];
				if (instruction.Operand is Instruction)
				{
					instruction.OpCode = instruction.OpCode.ToLongOp();
				}
			}
			method.RecalculateILOffsets();
			bool flag;
			do
			{
				flag = false;
				for (int j = 0; j < method.Body.Instructions.Count; j++)
				{
					Instruction instruction2 = method.Body.Instructions[j];
					Instruction instruction3 = instruction2.Operand as Instruction;
					if (instruction3 != null)
					{
						int num = instruction3.Offset - (instruction2.Offset + instruction2.GetSize());
						if (num == (sbyte)num)
						{
							Mono.Cecil.Cil.OpCode opCode = instruction2.OpCode;
							instruction2.OpCode = instruction2.OpCode.ToShortOp();
							flag = opCode != instruction2.OpCode;
						}
					}
				}
			}
			while (flag);
		}

		public static bool Is(this MemberInfo minfo, MemberReference mref)
		{
			return mref.Is(minfo);
		}

		public static bool Is(this MemberReference mref, MemberInfo minfo)
		{
			if (mref == null)
			{
				return false;
			}
			TypeReference typeReference = mref.DeclaringType;
			if (typeReference?.FullName == "<Module>")
			{
				typeReference = null;
			}
			GenericParameter genericParameter = mref as GenericParameter;
			if (genericParameter != null)
			{
				Type type = minfo as Type;
				if ((object)type == null)
				{
					return false;
				}
				if (!type.IsGenericParameter)
				{
					return (genericParameter.Owner as IGenericInstance)?.GenericArguments[genericParameter.Position].Is(type) ?? false;
				}
				return genericParameter.Position == type.GenericParameterPosition;
			}
			if (minfo.DeclaringType != null)
			{
				if (typeReference == null)
				{
					return false;
				}
				Type type2 = minfo.DeclaringType;
				if (minfo is Type && type2.IsGenericType && !type2.IsGenericTypeDefinition)
				{
					type2 = type2.GetGenericTypeDefinition();
				}
				if (!typeReference.Is(type2))
				{
					return false;
				}
			}
			else if (typeReference != null)
			{
				return false;
			}
			if (!(mref is TypeSpecification) && mref.Name != minfo.Name)
			{
				return false;
			}
			TypeReference typeReference2 = mref as TypeReference;
			if (typeReference2 != null)
			{
				Type type3 = minfo as Type;
				if ((object)type3 == null)
				{
					return false;
				}
				if (type3.IsGenericParameter)
				{
					return false;
				}
				GenericInstanceType genericInstanceType = mref as GenericInstanceType;
				if (genericInstanceType != null)
				{
					if (!type3.IsGenericType)
					{
						return false;
					}
					Collection<TypeReference> genericArguments = genericInstanceType.GenericArguments;
					Type[] genericArguments2 = type3.GetGenericArguments();
					if (genericArguments.Count != genericArguments2.Length)
					{
						return false;
					}
					for (int i = 0; i < genericArguments.Count; i++)
					{
						if (!genericArguments[i].Is(genericArguments2[i]))
						{
							return false;
						}
					}
					return genericInstanceType.ElementType.Is(type3.GetGenericTypeDefinition());
				}
				if (typeReference2.HasGenericParameters)
				{
					if (!type3.IsGenericType)
					{
						return false;
					}
					Collection<GenericParameter> genericParameters = typeReference2.GenericParameters;
					Type[] genericArguments3 = type3.GetGenericArguments();
					if (genericParameters.Count != genericArguments3.Length)
					{
						return false;
					}
					for (int j = 0; j < genericParameters.Count; j++)
					{
						if (!genericParameters[j].Is(genericArguments3[j]))
						{
							return false;
						}
					}
				}
				else if (type3.IsGenericType)
				{
					return false;
				}
				ArrayType arrayType = mref as ArrayType;
				if (arrayType != null)
				{
					if (!type3.IsArray)
					{
						return false;
					}
					if (arrayType.Dimensions.Count == type3.GetArrayRank())
					{
						return arrayType.ElementType.Is(type3.GetElementType());
					}
					return false;
				}
				ByReferenceType byReferenceType = mref as ByReferenceType;
				if (byReferenceType != null)
				{
					if (!type3.IsByRef)
					{
						return false;
					}
					return byReferenceType.ElementType.Is(type3.GetElementType());
				}
				PointerType pointerType = mref as PointerType;
				if (pointerType != null)
				{
					if (!type3.IsPointer)
					{
						return false;
					}
					return pointerType.ElementType.Is(type3.GetElementType());
				}
				TypeSpecification typeSpecification = mref as TypeSpecification;
				if (typeSpecification != null)
				{
					return typeSpecification.ElementType.Is(type3.HasElementType ? type3.GetElementType() : type3);
				}
				if (typeReference != null)
				{
					return mref.Name == type3.Name;
				}
				return mref.FullName == type3.FullName.Replace("+", "/", StringComparison.Ordinal);
			}
			if (minfo is Type)
			{
				return false;
			}
			MethodReference methodRef = mref as MethodReference;
			if (methodRef != null)
			{
				MethodBase methodBase = minfo as MethodBase;
				if ((object)methodBase == null)
				{
					return false;
				}
				Collection<ParameterDefinition> parameters = methodRef.Parameters;
				ParameterInfo[] parameters2 = methodBase.GetParameters();
				if (parameters.Count != parameters2.Length)
				{
					return false;
				}
				GenericInstanceMethod genericInstanceMethod = mref as GenericInstanceMethod;
				if (genericInstanceMethod != null)
				{
					if (!methodBase.IsGenericMethod)
					{
						return false;
					}
					Collection<TypeReference> genericArguments4 = genericInstanceMethod.GenericArguments;
					Type[] genericArguments5 = methodBase.GetGenericArguments();
					if (genericArguments4.Count != genericArguments5.Length)
					{
						return false;
					}
					for (int k = 0; k < genericArguments4.Count; k++)
					{
						if (!genericArguments4[k].Is(genericArguments5[k]))
						{
							return false;
						}
					}
					return genericInstanceMethod.ElementMethod.Is((methodBase as MethodInfo)?.GetGenericMethodDefinition() ?? methodBase);
				}
				if (methodRef.HasGenericParameters)
				{
					if (!methodBase.IsGenericMethod)
					{
						return false;
					}
					Collection<GenericParameter> genericParameters2 = methodRef.GenericParameters;
					Type[] genericArguments6 = methodBase.GetGenericArguments();
					if (genericParameters2.Count != genericArguments6.Length)
					{
						return false;
					}
					for (int l = 0; l < genericParameters2.Count; l++)
					{
						if (!genericParameters2[l].Is(genericArguments6[l]))
						{
							return false;
						}
					}
				}
				else if (methodBase.IsGenericMethod)
				{
					return false;
				}
				Relinker relinker = null;
				relinker = delegate(IMetadataTokenProvider paramMemberRef, IGenericParameterProvider ctx)
				{
					TypeReference typeReference4 = paramMemberRef as TypeReference;
					return (typeReference4 == null) ? paramMemberRef : ResolveParameter(typeReference4);
				};
				if (!methodRef.ReturnType.Relink(relinker, null).Is((methodBase as MethodInfo)?.ReturnType ?? typeof(void)) && !methodRef.ReturnType.Is((methodBase as MethodInfo)?.ReturnType ?? typeof(void)))
				{
					return false;
				}
				for (int m = 0; m < parameters.Count; m++)
				{
					if (!parameters[m].ParameterType.Relink(relinker, null).Is(parameters2[m].ParameterType) && !parameters[m].ParameterType.Is(parameters2[m].ParameterType))
					{
						return false;
					}
				}
				return true;
			}
			if (minfo is MethodInfo)
			{
				return false;
			}
			if (mref is FieldReference != minfo is FieldInfo)
			{
				return false;
			}
			if (mref is PropertyReference != minfo is PropertyInfo)
			{
				return false;
			}
			if (mref is EventReference != minfo is EventInfo)
			{
				return false;
			}
			return true;
			TypeReference ResolveParameter(TypeReference paramTypeRef)
			{
				GenericParameter genericParameter2 = paramTypeRef as GenericParameter;
				if (genericParameter2 != null)
				{
					if (genericParameter2.Owner is MethodReference)
					{
						GenericInstanceMethod genericInstanceMethod2 = methodRef as GenericInstanceMethod;
						if (genericInstanceMethod2 != null)
						{
							return genericInstanceMethod2.GenericArguments[genericParameter2.Position];
						}
					}
					TypeReference typeReference3 = genericParameter2.Owner as TypeReference;
					if (typeReference3 != null)
					{
						GenericInstanceType genericInstanceType2 = methodRef.DeclaringType as GenericInstanceType;
						if (genericInstanceType2 != null && typeReference3.FullName == genericInstanceType2.ElementType.FullName)
						{
							return genericInstanceType2.GenericArguments[genericParameter2.Position];
						}
					}
					return paramTypeRef;
				}
				if (paramTypeRef == methodRef.DeclaringType.GetElementType())
				{
					return methodRef.DeclaringType;
				}
				return paramTypeRef;
			}
		}

		public static IMetadataTokenProvider ImportReference(this ModuleDefinition mod, IMetadataTokenProvider mtp)
		{
			if (mtp is TypeReference)
			{
				return mod.ImportReference((TypeReference)mtp);
			}
			if (mtp is FieldReference)
			{
				return mod.ImportReference((FieldReference)mtp);
			}
			if (mtp is MethodReference)
			{
				return mod.ImportReference((MethodReference)mtp);
			}
			return mtp;
		}

		public static void AddRange<T>(this Collection<T> list, IEnumerable<T> other)
		{
			foreach (T item in other)
			{
				list.Add(item);
			}
		}

		public static void AddRange(this IDictionary dict, IDictionary other)
		{
			foreach (DictionaryEntry item in other)
			{
				dict.Add(item.Key, item.Value);
			}
		}

		public static void AddRange<K, V>(this IDictionary<K, V> dict, IDictionary<K, V> other)
		{
			foreach (KeyValuePair<K, V> item in other)
			{
				dict.Add(item.Key, item.Value);
			}
		}

		public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> other)
		{
			foreach (KeyValuePair<K, V> item in other)
			{
				dict.Add(item.Key, item.Value);
			}
		}

		public static void InsertRange<T>(this Collection<T> list, int index, IEnumerable<T> other)
		{
			foreach (T item in other)
			{
				list.Insert(index++, item);
			}
		}

		public static bool IsCompatible(this Type type, Type other)
		{
			if (!type._IsCompatible(other))
			{
				return other._IsCompatible(type);
			}
			return true;
		}

		private static bool _IsCompatible(this Type type, Type other)
		{
			if (type == other)
			{
				return true;
			}
			if (type.IsAssignableFrom(other))
			{
				return true;
			}
			if (other.IsEnum && type.IsCompatible(Enum.GetUnderlyingType(other)))
			{
				return true;
			}
			if ((other.IsPointer || other.IsByRef) && type == typeof(IntPtr))
			{
				return true;
			}
			return false;
		}

		public static T GetDeclaredMember<T>(this T member) where T : MemberInfo
		{
			if (member.DeclaringType == member.ReflectedType)
			{
				return member;
			}
			int metadataToken = member.MetadataToken;
			MemberInfo[] members = member.DeclaringType.GetMembers((BindingFlags)(-1));
			foreach (MemberInfo memberInfo in members)
			{
				if (memberInfo.MetadataToken == metadataToken)
				{
					return (T)memberInfo;
				}
			}
			return member;
		}

		public unsafe static void SetMonoCorlibInternal(this Assembly asm, bool value)
		{
			if (!ReflectionHelper.IsMono)
			{
				return;
			}
			Type type = asm?.GetType();
			if (type == null)
			{
				return;
			}
			FieldInfo value2;
			lock (fmap_mono_assembly)
			{
				if (!fmap_mono_assembly.TryGetValue(type, out value2))
				{
					value2 = type.GetField("_mono_assembly", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? type.GetField("dynamic_assembly", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					fmap_mono_assembly[type] = value2;
				}
			}
			if (!(value2 == null))
			{
				AssemblyName assemblyName = new AssemblyName(asm.FullName);
				lock (ReflectionHelper.AssemblyCache)
				{
					WeakReference value3 = new WeakReference(asm);
					ReflectionHelper.AssemblyCache[asm.GetRuntimeHashedFullName()] = value3;
					ReflectionHelper.AssemblyCache[assemblyName.FullName] = value3;
					ReflectionHelper.AssemblyCache[assemblyName.Name] = value3;
				}
				long num = 0L;
				object value4 = value2.GetValue(asm);
				if (value4 is IntPtr)
				{
					IntPtr intPtr = (IntPtr)value4;
					num = (long)intPtr;
				}
				else if (value4 is UIntPtr)
				{
					UIntPtr uIntPtr = (UIntPtr)value4;
					num = (long)(ulong)uIntPtr;
				}
				int num2 = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 20 + 4 + 4 + 4 + (_MonoAssemblyNameHasArch ? ((!ReflectionHelper.IsCore) ? ((IntPtr.Size == 4) ? 12 : 16) : ((IntPtr.Size == 4) ? 20 : 24)) : (ReflectionHelper.IsCore ? 16 : 8)) + IntPtr.Size + IntPtr.Size + 1 + 1 + 1;
				byte* ptr = (byte*)(num + num2);
				*ptr = (byte)(value ? 1 : 0);
			}
		}

		public static bool IsDynamicMethod(this MethodBase method)
		{
			if (_RTDynamicMethod != null)
			{
				if (!(method is DynamicMethod))
				{
					return method.GetType() == _RTDynamicMethod;
				}
				return true;
			}
			if (method is DynamicMethod)
			{
				return true;
			}
			if (method.MetadataToken != 0 || !method.IsStatic || !method.IsPublic || (method.Attributes & System.Reflection.MethodAttributes.PrivateScope) != 0)
			{
				return false;
			}
			MethodInfo[] methods = method.DeclaringType.GetMethods(BindingFlags.Static | BindingFlags.Public);
			foreach (MethodInfo methodInfo in methods)
			{
				if (method == methodInfo)
				{
					return false;
				}
			}
			return true;
		}

		public static object SafeGetTarget(this WeakReference weak)
		{
			try
			{
				return weak.Target;
			}
			catch (InvalidOperationException)
			{
				return null;
			}
		}

		public static bool SafeGetIsAlive(this WeakReference weak)
		{
			try
			{
				return weak.IsAlive;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}

		public static T CreateDelegate<T>(this MethodBase method) where T : Delegate
		{
			return (T)method.CreateDelegate(typeof(T), null);
		}

		public static T CreateDelegate<T>(this MethodBase method, object target) where T : Delegate
		{
			return (T)method.CreateDelegate(typeof(T), target);
		}

		public static Delegate CreateDelegate(this MethodBase method, Type delegateType)
		{
			return method.CreateDelegate(delegateType, null);
		}

		public static Delegate CreateDelegate(this MethodBase method, Type delegateType, object target)
		{
			if (!typeof(Delegate).IsAssignableFrom(delegateType))
			{
				throw new ArgumentException("Type argument must be a delegate type!");
			}
			DynamicMethod dynamicMethod = method as DynamicMethod;
			if ((object)dynamicMethod != null)
			{
				return dynamicMethod.CreateDelegate(delegateType, target);
			}
			RuntimeMethodHandle methodHandle = method.MethodHandle;
			RuntimeHelpers.PrepareMethod(methodHandle);
			IntPtr functionPointer = methodHandle.GetFunctionPointer();
			return (Delegate)Activator.CreateInstance(delegateType, target, functionPointer);
		}

		public static MethodDefinition FindMethod(this TypeDefinition type, string id, bool simple = true)
		{
			if (simple && !id.Contains(" ", StringComparison.Ordinal))
			{
				foreach (MethodDefinition method in type.Methods)
				{
					if (method.GetID(null, null, withType: true, simple: true) == id)
					{
						return method;
					}
				}
				foreach (MethodDefinition method2 in type.Methods)
				{
					if (method2.GetID(null, null, withType: false, simple: true) == id)
					{
						return method2;
					}
				}
			}
			foreach (MethodDefinition method3 in type.Methods)
			{
				if (method3.GetID() == id)
				{
					return method3;
				}
			}
			foreach (MethodDefinition method4 in type.Methods)
			{
				if (method4.GetID(null, null, withType: false) == id)
				{
					return method4;
				}
			}
			return null;
		}

		public static MethodDefinition FindMethodDeep(this TypeDefinition type, string id, bool simple = true)
		{
			MethodDefinition methodDefinition = type.FindMethod(id, simple);
			if (methodDefinition == null)
			{
				TypeReference baseType = type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				TypeDefinition typeDefinition = baseType.Resolve();
				if (typeDefinition == null)
				{
					return null;
				}
				methodDefinition = typeDefinition.FindMethodDeep(id, simple);
			}
			return methodDefinition;
		}

		public static MethodInfo FindMethod(this Type type, string id, bool simple = true)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo[] array;
			if (simple && !id.Contains(" ", StringComparison.Ordinal))
			{
				array = methods;
				foreach (MethodInfo methodInfo in array)
				{
					if (methodInfo.GetID(null, null, withType: true, proxyMethod: false, simple: true) == id)
					{
						return methodInfo;
					}
				}
				array = methods;
				foreach (MethodInfo methodInfo2 in array)
				{
					if (methodInfo2.GetID(null, null, withType: false, proxyMethod: false, simple: true) == id)
					{
						return methodInfo2;
					}
				}
			}
			array = methods;
			foreach (MethodInfo methodInfo3 in array)
			{
				if (methodInfo3.GetID() == id)
				{
					return methodInfo3;
				}
			}
			array = methods;
			foreach (MethodInfo methodInfo4 in array)
			{
				if (methodInfo4.GetID(null, null, withType: false) == id)
				{
					return methodInfo4;
				}
			}
			return null;
		}

		public static MethodInfo FindMethodDeep(this Type type, string id, bool simple = true)
		{
			MethodInfo methodInfo = type.FindMethod(id, simple);
			if ((object)methodInfo == null)
			{
				Type baseType = type.BaseType;
				if ((object)baseType == null)
				{
					return null;
				}
				methodInfo = baseType.FindMethodDeep(id, simple);
			}
			return methodInfo;
		}

		public static PropertyDefinition FindProperty(this TypeDefinition type, string name)
		{
			foreach (PropertyDefinition property in type.Properties)
			{
				if (property.Name == name)
				{
					return property;
				}
			}
			return null;
		}

		public static PropertyDefinition FindPropertyDeep(this TypeDefinition type, string name)
		{
			PropertyDefinition propertyDefinition = type.FindProperty(name);
			if (propertyDefinition == null)
			{
				TypeReference baseType = type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				TypeDefinition typeDefinition = baseType.Resolve();
				if (typeDefinition == null)
				{
					return null;
				}
				propertyDefinition = typeDefinition.FindPropertyDeep(name);
			}
			return propertyDefinition;
		}

		public static FieldDefinition FindField(this TypeDefinition type, string name)
		{
			foreach (FieldDefinition field in type.Fields)
			{
				if (field.Name == name)
				{
					return field;
				}
			}
			return null;
		}

		public static FieldDefinition FindFieldDeep(this TypeDefinition type, string name)
		{
			FieldDefinition fieldDefinition = type.FindField(name);
			if (fieldDefinition == null)
			{
				TypeReference baseType = type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				TypeDefinition typeDefinition = baseType.Resolve();
				if (typeDefinition == null)
				{
					return null;
				}
				fieldDefinition = typeDefinition.FindFieldDeep(name);
			}
			return fieldDefinition;
		}

		public static EventDefinition FindEvent(this TypeDefinition type, string name)
		{
			foreach (EventDefinition @event in type.Events)
			{
				if (@event.Name == name)
				{
					return @event;
				}
			}
			return null;
		}

		public static EventDefinition FindEventDeep(this TypeDefinition type, string name)
		{
			EventDefinition eventDefinition = type.FindEvent(name);
			if (eventDefinition == null)
			{
				TypeReference baseType = type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				TypeDefinition typeDefinition = baseType.Resolve();
				if (typeDefinition == null)
				{
					return null;
				}
				eventDefinition = typeDefinition.FindEventDeep(name);
			}
			return eventDefinition;
		}

		public static string GetID(this MethodReference method, string name = null, string type = null, bool withType = true, bool simple = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (simple)
			{
				if (withType && (type != null || method.DeclaringType != null))
				{
					stringBuilder.Append(type ?? method.DeclaringType.GetPatchFullName()).Append("::");
				}
				stringBuilder.Append(name ?? method.Name);
				return stringBuilder.ToString();
			}
			stringBuilder.Append(method.ReturnType.GetPatchFullName()).Append(" ");
			if (withType && (type != null || method.DeclaringType != null))
			{
				stringBuilder.Append(type ?? method.DeclaringType.GetPatchFullName()).Append("::");
			}
			stringBuilder.Append(name ?? method.Name);
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			if (genericInstanceMethod != null && genericInstanceMethod.GenericArguments.Count != 0)
			{
				stringBuilder.Append("<");
				Collection<TypeReference> genericArguments = genericInstanceMethod.GenericArguments;
				for (int i = 0; i < genericArguments.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(genericArguments[i].GetPatchFullName());
				}
				stringBuilder.Append(">");
			}
			else if (method.GenericParameters.Count != 0)
			{
				stringBuilder.Append("<");
				Collection<GenericParameter> genericParameters = method.GenericParameters;
				for (int j = 0; j < genericParameters.Count; j++)
				{
					if (j > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(genericParameters[j].Name);
				}
				stringBuilder.Append(">");
			}
			stringBuilder.Append("(");
			if (method.HasParameters)
			{
				Collection<ParameterDefinition> parameters = method.Parameters;
				for (int k = 0; k < parameters.Count; k++)
				{
					ParameterDefinition parameterDefinition = parameters[k];
					if (k > 0)
					{
						stringBuilder.Append(",");
					}
					if (parameterDefinition.ParameterType.IsSentinel)
					{
						stringBuilder.Append("...,");
					}
					stringBuilder.Append(parameterDefinition.ParameterType.GetPatchFullName());
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static string GetID(this Mono.Cecil.CallSite method)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(method.ReturnType.GetPatchFullName()).Append(" ");
			stringBuilder.Append("(");
			if (method.HasParameters)
			{
				Collection<ParameterDefinition> parameters = method.Parameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					ParameterDefinition parameterDefinition = parameters[i];
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					if (parameterDefinition.ParameterType.IsSentinel)
					{
						stringBuilder.Append("...,");
					}
					stringBuilder.Append(parameterDefinition.ParameterType.GetPatchFullName());
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static string GetID(this MethodBase method, string name = null, string type = null, bool withType = true, bool proxyMethod = false, bool simple = false)
		{
			while (method is MethodInfo && method.IsGenericMethod && !method.IsGenericMethodDefinition)
			{
				method = ((MethodInfo)method).GetGenericMethodDefinition();
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (simple)
			{
				if (withType && (type != null || method.DeclaringType != null))
				{
					stringBuilder.Append(type ?? method.DeclaringType.FullName).Append("::");
				}
				stringBuilder.Append(name ?? method.Name);
				return stringBuilder.ToString();
			}
			stringBuilder.Append((method as MethodInfo)?.ReturnType?.FullName ?? "System.Void").Append(" ");
			if (withType && (type != null || method.DeclaringType != null))
			{
				stringBuilder.Append(type ?? method.DeclaringType.FullName.Replace("+", "/", StringComparison.Ordinal)).Append("::");
			}
			stringBuilder.Append(name ?? method.Name);
			if (method.ContainsGenericParameters)
			{
				stringBuilder.Append("<");
				Type[] genericArguments = method.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(genericArguments[i].Name);
				}
				stringBuilder.Append(">");
			}
			stringBuilder.Append("(");
			ParameterInfo[] parameters = method.GetParameters();
			for (int j = (proxyMethod ? 1 : 0); j < parameters.Length; j++)
			{
				ParameterInfo parameterInfo = parameters[j];
				if (j > (proxyMethod ? 1 : 0))
				{
					stringBuilder.Append(",");
				}
				bool flag;
				try
				{
					flag = parameterInfo.GetCustomAttributes(t_ParamArrayAttribute, inherit: false).Length != 0;
				}
				catch (NotSupportedException)
				{
					flag = false;
				}
				if (flag)
				{
					stringBuilder.Append("...,");
				}
				stringBuilder.Append(parameterInfo.ParameterType.FullName);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static string GetPatchName(this MemberReference mr)
		{
			Mono.Cecil.ICustomAttributeProvider obj = mr as Mono.Cecil.ICustomAttributeProvider;
			return ((obj != null) ? obj.GetPatchName() : null) ?? mr.Name;
		}

		public static string GetPatchFullName(this MemberReference mr)
		{
			Mono.Cecil.ICustomAttributeProvider obj = mr as Mono.Cecil.ICustomAttributeProvider;
			return ((obj != null) ? obj.GetPatchFullName(mr) : null) ?? mr.FullName;
		}

		private static string GetPatchName(this Mono.Cecil.ICustomAttributeProvider cap)
		{
			CustomAttribute customAttribute = cap.GetCustomAttribute("MonoMod.MonoModPatch");
			string text;
			if (customAttribute != null)
			{
				text = (string)customAttribute.ConstructorArguments[0].Value;
				int num = text.LastIndexOf('.');
				if (num != -1 && num != text.Length - 1)
				{
					text = text.Substring(num + 1);
				}
				return text;
			}
			text = ((MemberReference)cap).Name;
			if (!text.StartsWith("patch_", StringComparison.Ordinal))
			{
				return text;
			}
			return text.Substring(6);
		}

		private static string GetPatchFullName(this Mono.Cecil.ICustomAttributeProvider cap, MemberReference mr)
		{
			TypeReference typeReference = cap as TypeReference;
			if (typeReference != null)
			{
				CustomAttribute customAttribute = cap.GetCustomAttribute("MonoMod.MonoModPatch");
				string text;
				if (customAttribute != null)
				{
					text = (string)customAttribute.ConstructorArguments[0].Value;
				}
				else
				{
					text = ((MemberReference)cap).Name;
					text = (text.StartsWith("patch_", StringComparison.Ordinal) ? text.Substring(6) : text);
				}
				if (text.StartsWith("global::", StringComparison.Ordinal))
				{
					text = text.Substring(8);
				}
				else if (!text.Contains(".", StringComparison.Ordinal) && !text.Contains("/", StringComparison.Ordinal))
				{
					if (!string.IsNullOrEmpty(typeReference.Namespace))
					{
						text = typeReference.Namespace + "." + text;
					}
					else if (typeReference.IsNested)
					{
						text = typeReference.DeclaringType.GetPatchFullName() + "/" + text;
					}
				}
				if (mr is TypeSpecification)
				{
					List<TypeSpecification> list = new List<TypeSpecification>();
					TypeSpecification typeSpecification = (TypeSpecification)mr;
					do
					{
						list.Add(typeSpecification);
					}
					while ((typeSpecification = typeSpecification.ElementType as TypeSpecification) != null);
					StringBuilder stringBuilder = new StringBuilder(text.Length + list.Count * 4);
					stringBuilder.Append(text);
					for (int num = list.Count - 1; num > -1; num--)
					{
						typeSpecification = list[num];
						if (typeSpecification.IsByReference)
						{
							stringBuilder.Append("&");
						}
						else if (typeSpecification.IsPointer)
						{
							stringBuilder.Append("*");
						}
						else if (!typeSpecification.IsPinned && !typeSpecification.IsSentinel)
						{
							if (typeSpecification.IsArray)
							{
								ArrayType arrayType = (ArrayType)typeSpecification;
								if (arrayType.IsVector)
								{
									stringBuilder.Append("[]");
								}
								else
								{
									stringBuilder.Append("[");
									for (int i = 0; i < arrayType.Dimensions.Count; i++)
									{
										if (i > 0)
										{
											stringBuilder.Append(",");
										}
										stringBuilder.Append(arrayType.Dimensions[i].ToString());
									}
									stringBuilder.Append("]");
								}
							}
							else if (typeSpecification.IsRequiredModifier)
							{
								stringBuilder.Append("modreq(").Append(((RequiredModifierType)typeSpecification).ModifierType).Append(")");
							}
							else if (typeSpecification.IsOptionalModifier)
							{
								stringBuilder.Append("modopt(").Append(((OptionalModifierType)typeSpecification).ModifierType).Append(")");
							}
							else if (typeSpecification.IsGenericInstance)
							{
								GenericInstanceType genericInstanceType = (GenericInstanceType)typeSpecification;
								stringBuilder.Append("<");
								for (int j = 0; j < genericInstanceType.GenericArguments.Count; j++)
								{
									if (j > 0)
									{
										stringBuilder.Append(",");
									}
									stringBuilder.Append(genericInstanceType.GenericArguments[j].GetPatchFullName());
								}
								stringBuilder.Append(">");
							}
							else
							{
								if (!typeSpecification.IsFunctionPointer)
								{
									throw new NotSupportedException($"MonoMod can't handle TypeSpecification: {typeReference.FullName} ({typeReference.GetType()})");
								}
								FunctionPointerType functionPointerType = (FunctionPointerType)typeSpecification;
								stringBuilder.Append(" ").Append(functionPointerType.ReturnType.GetPatchFullName()).Append(" *(");
								if (functionPointerType.HasParameters)
								{
									for (int k = 0; k < functionPointerType.Parameters.Count; k++)
									{
										ParameterDefinition parameterDefinition = functionPointerType.Parameters[k];
										if (k > 0)
										{
											stringBuilder.Append(",");
										}
										if (parameterDefinition.ParameterType.IsSentinel)
										{
											stringBuilder.Append("...,");
										}
										stringBuilder.Append(parameterDefinition.ParameterType.FullName);
									}
								}
								stringBuilder.Append(")");
							}
						}
					}
					text = stringBuilder.ToString();
				}
				return text;
			}
			FieldReference fieldReference = cap as FieldReference;
			if (fieldReference != null)
			{
				return fieldReference.FieldType.GetPatchFullName() + " " + fieldReference.DeclaringType.GetPatchFullName() + "::" + cap.GetPatchName();
			}
			if (cap is MethodReference)
			{
				throw new InvalidOperationException("GetPatchFullName not supported on MethodReferences - use GetID instead");
			}
			throw new InvalidOperationException($"GetPatchFullName not supported on type {cap.GetType()}");
		}

		public static MethodDefinition Clone(this MethodDefinition o, MethodDefinition c = null)
		{
			if (o == null)
			{
				return null;
			}
			if (c == null)
			{
				c = new MethodDefinition(o.Name, o.Attributes, o.ReturnType);
			}
			c.Name = o.Name;
			c.Attributes = o.Attributes;
			c.ReturnType = o.ReturnType;
			c.DeclaringType = o.DeclaringType;
			c.MetadataToken = c.MetadataToken;
			c.Body = o.Body?.Clone(c);
			c.Attributes = o.Attributes;
			c.ImplAttributes = o.ImplAttributes;
			c.PInvokeInfo = o.PInvokeInfo;
			c.IsPreserveSig = o.IsPreserveSig;
			c.IsPInvokeImpl = o.IsPInvokeImpl;
			foreach (GenericParameter genericParameter2 in o.GenericParameters)
			{
				c.GenericParameters.Add(genericParameter2.Clone());
			}
			foreach (ParameterDefinition parameter in o.Parameters)
			{
				c.Parameters.Add(parameter.Clone());
			}
			foreach (CustomAttribute customAttribute in o.CustomAttributes)
			{
				c.CustomAttributes.Add(customAttribute.Clone());
			}
			foreach (MethodReference @override in o.Overrides)
			{
				c.Overrides.Add(@override);
			}
			if (c.Body != null)
			{
				foreach (Instruction instruction in c.Body.Instructions)
				{
					GenericParameter genericParameter = instruction.Operand as GenericParameter;
					int index;
					if (genericParameter != null && (index = o.GenericParameters.IndexOf(genericParameter)) != -1)
					{
						instruction.Operand = c.GenericParameters[index];
					}
					else
					{
						ParameterDefinition parameterDefinition = instruction.Operand as ParameterDefinition;
						if (parameterDefinition != null && (index = o.Parameters.IndexOf(parameterDefinition)) != -1)
						{
							instruction.Operand = c.Parameters[index];
						}
					}
				}
				return c;
			}
			return c;
		}

		public static Mono.Cecil.Cil.MethodBody Clone(this Mono.Cecil.Cil.MethodBody bo, MethodDefinition m)
		{
			if (bo == null)
			{
				return null;
			}
			Mono.Cecil.Cil.MethodBody bc = new Mono.Cecil.Cil.MethodBody(m);
			bc.MaxStackSize = bo.MaxStackSize;
			bc.InitLocals = bo.InitLocals;
			bc.LocalVarToken = bo.LocalVarToken;
			bc.Instructions.AddRange(bo.Instructions.Select(delegate(Instruction o)
			{
				Instruction instruction2 = Instruction.Create(Mono.Cecil.Cil.OpCodes.Nop);
				instruction2.OpCode = o.OpCode;
				instruction2.Operand = o.Operand;
				instruction2.Offset = o.Offset;
				return instruction2;
			}));
			foreach (Instruction instruction3 in bc.Instructions)
			{
				Instruction instruction = instruction3.Operand as Instruction;
				if (instruction != null)
				{
					instruction3.Operand = bc.Instructions[bo.Instructions.IndexOf(instruction)];
					continue;
				}
				Instruction[] array = instruction3.Operand as Instruction[];
				if (array != null)
				{
					instruction3.Operand = array.Select((Instruction i) => bc.Instructions[bo.Instructions.IndexOf(i)]).ToArray();
				}
			}
			bc.ExceptionHandlers.AddRange(bo.ExceptionHandlers.Select((Mono.Cecil.Cil.ExceptionHandler o) => new Mono.Cecil.Cil.ExceptionHandler(o.HandlerType)
			{
				TryStart = ((o.TryStart == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.TryStart)]),
				TryEnd = ((o.TryEnd == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.TryEnd)]),
				FilterStart = ((o.FilterStart == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.FilterStart)]),
				HandlerStart = ((o.HandlerStart == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.HandlerStart)]),
				HandlerEnd = ((o.HandlerEnd == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.HandlerEnd)]),
				CatchType = o.CatchType
			}));
			bc.Variables.AddRange(bo.Variables.Select((VariableDefinition o) => new VariableDefinition(o.VariableType)));
			m.CustomDebugInformations.AddRange(bo.Method.CustomDebugInformations);
			m.DebugInformation.SequencePoints.AddRange(bo.Method.DebugInformation.SequencePoints.Select((SequencePoint o) => new SequencePoint(bc.Instructions.FirstOrDefault((Instruction i) => i.Offset == o.Offset), o.Document)
			{
				StartLine = o.StartLine,
				StartColumn = o.StartColumn,
				EndLine = o.EndLine,
				EndColumn = o.EndColumn
			}));
			return bc;
		}

		public static GenericParameter Update(this GenericParameter param, int position, GenericParameterType type)
		{
			f_GenericParameter_position.SetValue(param, position);
			f_GenericParameter_type.SetValue(param, type);
			return param;
		}

		public static GenericParameter ResolveGenericParameter(this IGenericParameterProvider provider, GenericParameter orig)
		{
			if (provider is GenericParameter && ((GenericParameter)provider).Name == orig.Name)
			{
				return (GenericParameter)provider;
			}
			foreach (GenericParameter genericParameter in provider.GenericParameters)
			{
				if (genericParameter.Name == orig.Name)
				{
					return genericParameter;
				}
			}
			int position = orig.Position;
			if (provider is MethodReference && orig.DeclaringMethod != null)
			{
				if (position < provider.GenericParameters.Count)
				{
					return provider.GenericParameters[position];
				}
				return orig.Clone().Update(position, GenericParameterType.Method);
			}
			if (provider is TypeReference && orig.DeclaringType != null)
			{
				if (position < provider.GenericParameters.Count)
				{
					return provider.GenericParameters[position];
				}
				return orig.Clone().Update(position, GenericParameterType.Type);
			}
			object obj = (provider as TypeSpecification)?.ElementType.ResolveGenericParameter(orig);
			if (obj == null)
			{
				MemberReference obj2 = provider as MemberReference;
				if (obj2 == null)
				{
					return null;
				}
				TypeReference declaringType = obj2.DeclaringType;
				if (declaringType == null)
				{
					return null;
				}
				obj = declaringType.ResolveGenericParameter(orig);
			}
			return (GenericParameter)obj;
		}

		public static IMetadataTokenProvider Relink(this IMetadataTokenProvider mtp, Relinker relinker, IGenericParameterProvider context)
		{
			if (mtp is TypeReference)
			{
				return ((TypeReference)mtp).Relink(relinker, context);
			}
			if (mtp is GenericParameterConstraint)
			{
				return ((GenericParameterConstraint)mtp).Relink(relinker, context);
			}
			if (mtp is MethodReference)
			{
				return ((MethodReference)mtp).Relink(relinker, context);
			}
			if (mtp is FieldReference)
			{
				return ((FieldReference)mtp).Relink(relinker, context);
			}
			if (mtp is ParameterDefinition)
			{
				return ((ParameterDefinition)mtp).Relink(relinker, context);
			}
			if (mtp is Mono.Cecil.CallSite)
			{
				return ((Mono.Cecil.CallSite)mtp).Relink(relinker, context);
			}
			throw new InvalidOperationException($"MonoMod can't handle metadata token providers of the type {mtp.GetType()}");
		}

		public static TypeReference Relink(this TypeReference type, Relinker relinker, IGenericParameterProvider context)
		{
			if (type == null)
			{
				return null;
			}
			TypeSpecification typeSpecification = type as TypeSpecification;
			if (typeSpecification != null)
			{
				TypeReference type2 = typeSpecification.ElementType.Relink(relinker, context);
				if (type.IsSentinel)
				{
					return new SentinelType(type2);
				}
				if (type.IsByReference)
				{
					return new ByReferenceType(type2);
				}
				if (type.IsPointer)
				{
					return new PointerType(type2);
				}
				if (type.IsPinned)
				{
					return new PinnedType(type2);
				}
				if (type.IsArray)
				{
					ArrayType arrayType = new ArrayType(type2, ((ArrayType)type).Rank);
					for (int i = 0; i < arrayType.Rank; i++)
					{
						arrayType.Dimensions[i] = ((ArrayType)type).Dimensions[i];
					}
					return arrayType;
				}
				if (type.IsRequiredModifier)
				{
					return new RequiredModifierType(((RequiredModifierType)type).ModifierType.Relink(relinker, context), type2);
				}
				if (type.IsOptionalModifier)
				{
					return new OptionalModifierType(((OptionalModifierType)type).ModifierType.Relink(relinker, context), type2);
				}
				if (type.IsGenericInstance)
				{
					GenericInstanceType genericInstanceType = new GenericInstanceType(type2);
					{
						foreach (TypeReference genericArgument in ((GenericInstanceType)type).GenericArguments)
						{
							genericInstanceType.GenericArguments.Add(genericArgument?.Relink(relinker, context));
						}
						return genericInstanceType;
					}
				}
				if (type.IsFunctionPointer)
				{
					FunctionPointerType functionPointerType = (FunctionPointerType)type;
					functionPointerType.ReturnType = functionPointerType.ReturnType.Relink(relinker, context);
					for (int j = 0; j < functionPointerType.Parameters.Count; j++)
					{
						functionPointerType.Parameters[j].ParameterType = functionPointerType.Parameters[j].ParameterType.Relink(relinker, context);
					}
					return functionPointerType;
				}
				throw new NotSupportedException($"MonoMod can't handle TypeSpecification: {type.FullName} ({type.GetType()})");
			}
			if (type.IsGenericParameter && context != null)
			{
				GenericParameter genericParameter = context.ResolveGenericParameter((GenericParameter)type);
				if (genericParameter == null)
				{
					throw new RelinkTargetNotFoundException(string.Format("{0} {1} (context: {2})", "MonoMod relinker failed finding", type.FullName, context), type, context);
				}
				for (int k = 0; k < genericParameter.Constraints.Count; k++)
				{
					if (!genericParameter.Constraints[k].GetConstraintType().IsGenericInstance)
					{
						genericParameter.Constraints[k] = genericParameter.Constraints[k].Relink(relinker, context);
					}
				}
				return genericParameter;
			}
			return (TypeReference)relinker(type, context);
		}

		public static GenericParameterConstraint Relink(this GenericParameterConstraint constraint, Relinker relinker, IGenericParameterProvider context)
		{
			if (constraint == null)
			{
				return null;
			}
			GenericParameterConstraint genericParameterConstraint = new GenericParameterConstraint(constraint.ConstraintType.Relink(relinker, context));
			foreach (CustomAttribute customAttribute in constraint.CustomAttributes)
			{
				genericParameterConstraint.CustomAttributes.Add(customAttribute.Relink(relinker, context));
			}
			return genericParameterConstraint;
		}

		public static IMetadataTokenProvider Relink(this MethodReference method, Relinker relinker, IGenericParameterProvider context)
		{
			if (method.IsGenericInstance)
			{
				GenericInstanceMethod obj = (GenericInstanceMethod)method;
				GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod((MethodReference)obj.ElementMethod.Relink(relinker, context));
				foreach (TypeReference genericArgument in obj.GenericArguments)
				{
					genericInstanceMethod.GenericArguments.Add(genericArgument.Relink(relinker, context));
				}
				return (MethodReference)relinker(genericInstanceMethod, context);
			}
			MethodReference methodReference = new MethodReference(method.Name, method.ReturnType, method.DeclaringType.Relink(relinker, context));
			methodReference.CallingConvention = method.CallingConvention;
			methodReference.ExplicitThis = method.ExplicitThis;
			methodReference.HasThis = method.HasThis;
			foreach (GenericParameter genericParameter in method.GenericParameters)
			{
				methodReference.GenericParameters.Add(genericParameter.Relink(relinker, context));
			}
			methodReference.ReturnType = methodReference.ReturnType?.Relink(relinker, methodReference);
			foreach (ParameterDefinition parameter in method.Parameters)
			{
				parameter.ParameterType = parameter.ParameterType.Relink(relinker, method);
				methodReference.Parameters.Add(parameter);
			}
			return (MethodReference)relinker(methodReference, context);
		}

		public static Mono.Cecil.CallSite Relink(this Mono.Cecil.CallSite method, Relinker relinker, IGenericParameterProvider context)
		{
			Mono.Cecil.CallSite callSite = new Mono.Cecil.CallSite(method.ReturnType);
			callSite.CallingConvention = method.CallingConvention;
			callSite.ExplicitThis = method.ExplicitThis;
			callSite.HasThis = method.HasThis;
			callSite.ReturnType = callSite.ReturnType?.Relink(relinker, context);
			foreach (ParameterDefinition parameter in method.Parameters)
			{
				parameter.ParameterType = parameter.ParameterType.Relink(relinker, context);
				callSite.Parameters.Add(parameter);
			}
			return (Mono.Cecil.CallSite)relinker(callSite, context);
		}

		public static IMetadataTokenProvider Relink(this FieldReference field, Relinker relinker, IGenericParameterProvider context)
		{
			TypeReference typeReference = field.DeclaringType.Relink(relinker, context);
			return relinker(new FieldReference(field.Name, field.FieldType.Relink(relinker, typeReference), typeReference), context);
		}

		public static ParameterDefinition Relink(this ParameterDefinition param, Relinker relinker, IGenericParameterProvider context)
		{
			param = (param.Method as MethodReference)?.Parameters[param.Index] ?? param;
			ParameterDefinition parameterDefinition = new ParameterDefinition(param.Name, param.Attributes, param.ParameterType.Relink(relinker, context))
			{
				IsIn = param.IsIn,
				IsLcid = param.IsLcid,
				IsOptional = param.IsOptional,
				IsOut = param.IsOut,
				IsReturnValue = param.IsReturnValue,
				MarshalInfo = param.MarshalInfo
			};
			if (param.HasConstant)
			{
				parameterDefinition.Constant = param.Constant;
			}
			return parameterDefinition;
		}

		public static ParameterDefinition Clone(this ParameterDefinition param)
		{
			ParameterDefinition parameterDefinition = new ParameterDefinition(param.Name, param.Attributes, param.ParameterType)
			{
				IsIn = param.IsIn,
				IsLcid = param.IsLcid,
				IsOptional = param.IsOptional,
				IsOut = param.IsOut,
				IsReturnValue = param.IsReturnValue,
				MarshalInfo = param.MarshalInfo
			};
			if (param.HasConstant)
			{
				parameterDefinition.Constant = param.Constant;
			}
			foreach (CustomAttribute customAttribute in param.CustomAttributes)
			{
				parameterDefinition.CustomAttributes.Add(customAttribute.Clone());
			}
			return parameterDefinition;
		}

		public static CustomAttribute Relink(this CustomAttribute attrib, Relinker relinker, IGenericParameterProvider context)
		{
			CustomAttribute customAttribute = new CustomAttribute((MethodReference)attrib.Constructor.Relink(relinker, context));
			foreach (CustomAttributeArgument constructorArgument in attrib.ConstructorArguments)
			{
				customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(constructorArgument.Type.Relink(relinker, context), constructorArgument.Value));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument field in attrib.Fields)
			{
				customAttribute.Fields.Add(new Mono.Cecil.CustomAttributeNamedArgument(field.Name, new CustomAttributeArgument(field.Argument.Type.Relink(relinker, context), field.Argument.Value)));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument property in attrib.Properties)
			{
				customAttribute.Properties.Add(new Mono.Cecil.CustomAttributeNamedArgument(property.Name, new CustomAttributeArgument(property.Argument.Type.Relink(relinker, context), property.Argument.Value)));
			}
			return customAttribute;
		}

		public static CustomAttribute Clone(this CustomAttribute attrib)
		{
			CustomAttribute customAttribute = new CustomAttribute(attrib.Constructor);
			foreach (CustomAttributeArgument constructorArgument in attrib.ConstructorArguments)
			{
				customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(constructorArgument.Type, constructorArgument.Value));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument field in attrib.Fields)
			{
				customAttribute.Fields.Add(new Mono.Cecil.CustomAttributeNamedArgument(field.Name, new CustomAttributeArgument(field.Argument.Type, field.Argument.Value)));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument property in attrib.Properties)
			{
				customAttribute.Properties.Add(new Mono.Cecil.CustomAttributeNamedArgument(property.Name, new CustomAttributeArgument(property.Argument.Type, property.Argument.Value)));
			}
			return customAttribute;
		}

		public static GenericParameter Relink(this GenericParameter param, Relinker relinker, IGenericParameterProvider context)
		{
			GenericParameter genericParameter = new GenericParameter(param.Name, param.Owner)
			{
				Attributes = param.Attributes
			}.Update(param.Position, param.Type);
			foreach (CustomAttribute customAttribute in param.CustomAttributes)
			{
				genericParameter.CustomAttributes.Add(customAttribute.Relink(relinker, context));
			}
			foreach (GenericParameterConstraint constraint in param.Constraints)
			{
				genericParameter.Constraints.Add(constraint.Relink(relinker, context));
			}
			return genericParameter;
		}

		public static GenericParameter Clone(this GenericParameter param)
		{
			GenericParameter genericParameter = new GenericParameter(param.Name, param.Owner)
			{
				Attributes = param.Attributes
			}.Update(param.Position, param.Type);
			foreach (CustomAttribute customAttribute in param.CustomAttributes)
			{
				genericParameter.CustomAttributes.Add(customAttribute.Clone());
			}
			foreach (GenericParameterConstraint constraint in param.Constraints)
			{
				genericParameter.Constraints.Add(constraint);
			}
			return genericParameter;
		}

		public static int GetManagedSize(this Type t)
		{
			if (_GetManagedSizeCache.TryGetValue(t, out var value))
			{
				return value;
			}
			if (_GetManagedSizeHelper == null)
			{
				Assembly assembly;
				using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("MonoMod.Utils.GetManagedSizeHelper", new ModuleParameters
				{
					Kind = ModuleKind.Dll
				}))
				{
					TypeDefinition typeDefinition = new TypeDefinition("MonoMod.Utils", "GetManagedSizeHelper", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.Sealed)
					{
						BaseType = moduleDefinition.TypeSystem.Object
					};
					moduleDefinition.Types.Add(typeDefinition);
					MethodDefinition methodDefinition = new MethodDefinition("GetManagedSizeHelper", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.HideBySig, moduleDefinition.TypeSystem.Int32);
					GenericParameter genericParameter = new GenericParameter("T", methodDefinition);
					methodDefinition.GenericParameters.Add(genericParameter);
					typeDefinition.Methods.Add(methodDefinition);
					ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
					iLProcessor.Emit(Mono.Cecil.Cil.OpCodes.Sizeof, genericParameter);
					iLProcessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
					assembly = ReflectionHelper.Load(moduleDefinition);
				}
				_GetManagedSizeHelper = assembly.GetType("MonoMod.Utils.GetManagedSizeHelper").GetMethod("GetManagedSizeHelper");
			}
			value = _GetManagedSizeHelper.MakeGenericMethod(t).CreateDelegate<Func<int>>()();
			lock (_GetManagedSizeCache)
			{
				return _GetManagedSizeCache[t] = value;
			}
		}

		public static Type GetThisParamType(this MethodBase method)
		{
			Type type = method.DeclaringType;
			if (type.IsValueType)
			{
				type = type.MakeByRefType();
			}
			return type;
		}

		public static IntPtr GetLdftnPointer(this MethodBase m)
		{
			if (_GetLdftnPointerCache.TryGetValue(m, out var value))
			{
				return value();
			}
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("GetLdftnPointer<" + m.GetID(null, null, withType: true, proxyMethod: false, simple: true) + ">", typeof(IntPtr), Type.EmptyTypes);
			ILProcessor iLProcessor = dynamicMethodDefinition.GetILProcessor();
			iLProcessor.Emit(Mono.Cecil.Cil.OpCodes.Ldftn, dynamicMethodDefinition.Definition.Module.ImportReference(m));
			iLProcessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
			lock (_GetLdftnPointerCache)
			{
				Func<IntPtr> func2 = (_GetLdftnPointerCache[m] = dynamicMethodDefinition.Generate().CreateDelegate<Func<IntPtr>>());
				return func2();
			}
		}
	}
}

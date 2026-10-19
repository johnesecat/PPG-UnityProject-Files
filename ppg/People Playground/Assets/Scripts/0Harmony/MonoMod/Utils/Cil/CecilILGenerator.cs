using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MonoMod.Utils.Cil
{
	public sealed class CecilILGenerator : ILGeneratorShim
	{
		private class LabelInfo
		{
			public bool Emitted;

			public Instruction Instruction = Instruction.Create(Mono.Cecil.Cil.OpCodes.Nop);

			public readonly List<Instruction> Branches = new List<Instruction>();
		}

		private class LabelledExceptionHandler
		{
			public Label TryStart = NullLabel;

			public Label TryEnd = NullLabel;

			public Label HandlerStart = NullLabel;

			public Label HandlerEnd = NullLabel;

			public Label FilterStart = NullLabel;

			public ExceptionHandlerType HandlerType;

			public TypeReference ExceptionType;
		}

		private class ExceptionHandlerChain
		{
			private readonly CecilILGenerator IL;

			private readonly Label _Start;

			public readonly Label SkipAll;

			private Label _SkipHandler;

			private LabelledExceptionHandler _Prev;

			private LabelledExceptionHandler _Handler;

			public ExceptionHandlerChain(CecilILGenerator il)
			{
				IL = il;
				_Start = il.DefineLabel();
				il.MarkLabel(_Start);
				SkipAll = il.DefineLabel();
			}

			public LabelledExceptionHandler BeginHandler(ExceptionHandlerType type)
			{
				LabelledExceptionHandler labelledExceptionHandler = (_Prev = _Handler);
				if (labelledExceptionHandler != null)
				{
					EndHandler(labelledExceptionHandler);
				}
				IL.Emit(System.Reflection.Emit.OpCodes.Leave, _SkipHandler = IL.DefineLabel());
				Label label = IL.DefineLabel();
				IL.MarkLabel(label);
				LabelledExceptionHandler labelledExceptionHandler2 = (_Handler = new LabelledExceptionHandler
				{
					TryStart = _Start,
					TryEnd = label,
					HandlerType = type,
					HandlerEnd = _SkipHandler
				});
				if (type == ExceptionHandlerType.Filter)
				{
					labelledExceptionHandler2.FilterStart = label;
				}
				else
				{
					labelledExceptionHandler2.HandlerStart = label;
				}
				return labelledExceptionHandler2;
			}

			public void EndHandler(LabelledExceptionHandler handler)
			{
				Label skipHandler = _SkipHandler;
				switch (handler.HandlerType)
				{
				case ExceptionHandlerType.Filter:
					IL.Emit(System.Reflection.Emit.OpCodes.Endfilter);
					break;
				case ExceptionHandlerType.Finally:
					IL.Emit(System.Reflection.Emit.OpCodes.Endfinally);
					break;
				default:
					IL.Emit(System.Reflection.Emit.OpCodes.Leave, skipHandler);
					break;
				}
				IL.MarkLabel(skipHandler);
				IL._ExceptionHandlersToMark.Add(handler);
			}

			public void End()
			{
				EndHandler(_Handler);
				IL.MarkLabel(SkipAll);
			}
		}

		private static readonly ConstructorInfo c_LocalBuilder;

		private static readonly FieldInfo f_LocalBuilder_position;

		private static readonly FieldInfo f_LocalBuilder_is_pinned;

		private static int c_LocalBuilder_params;

		private static readonly Dictionary<short, Mono.Cecil.Cil.OpCode> _MCCOpCodes;

		private static Label NullLabel;

		public readonly ILProcessor IL;

		private readonly Dictionary<Label, LabelInfo> _LabelInfos = new Dictionary<Label, LabelInfo>();

		private readonly List<LabelInfo> _LabelsToMark = new List<LabelInfo>();

		private readonly List<LabelledExceptionHandler> _ExceptionHandlersToMark = new List<LabelledExceptionHandler>();

		private readonly Dictionary<LocalBuilder, VariableDefinition> _Variables = new Dictionary<LocalBuilder, VariableDefinition>();

		private readonly Stack<ExceptionHandlerChain> _ExceptionHandlers = new Stack<ExceptionHandlerChain>();

		private int labelCounter;

		private int _ILOffset;

		public override int ILOffset => _ILOffset;

		unsafe static CecilILGenerator()
		{
			c_LocalBuilder = (from c in typeof(LocalBuilder).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				orderby c.GetParameters().Length descending
				select c).First();
			f_LocalBuilder_position = typeof(LocalBuilder).GetField("position", BindingFlags.Instance | BindingFlags.NonPublic);
			f_LocalBuilder_is_pinned = typeof(LocalBuilder).GetField("is_pinned", BindingFlags.Instance | BindingFlags.NonPublic);
			c_LocalBuilder_params = c_LocalBuilder.GetParameters().Length;
			_MCCOpCodes = new Dictionary<short, Mono.Cecil.Cil.OpCode>();
			FieldInfo[] fields = typeof(Mono.Cecil.Cil.OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
			for (int i = 0; i < fields.Length; i++)
			{
				Mono.Cecil.Cil.OpCode value = (Mono.Cecil.Cil.OpCode)fields[i].GetValue(null);
				_MCCOpCodes[value.Value] = value;
			}
			Label nullLabel = default(Label);
			*(int*)(&nullLabel) = -1;
			NullLabel = nullLabel;
		}

		public CecilILGenerator(ILProcessor il)
		{
			IL = il;
		}

		private Mono.Cecil.Cil.OpCode _(System.Reflection.Emit.OpCode opcode)
		{
			return _MCCOpCodes[opcode.Value];
		}

		private LabelInfo _(Label handle)
		{
			if (!_LabelInfos.TryGetValue(handle, out var value))
			{
				return null;
			}
			return value;
		}

		private VariableDefinition _(LocalBuilder handle)
		{
			return _Variables[handle];
		}

		private TypeReference _(Type info)
		{
			return IL.Body.Method.Module.ImportReference(info);
		}

		private FieldReference _(FieldInfo info)
		{
			return IL.Body.Method.Module.ImportReference(info);
		}

		private MethodReference _(MethodBase info)
		{
			return IL.Body.Method.Module.ImportReference(info);
		}

		private Instruction ProcessLabels(Instruction ins)
		{
			if (_LabelsToMark.Count != 0)
			{
				foreach (LabelInfo item in _LabelsToMark)
				{
					foreach (Instruction branch in item.Branches)
					{
						object operand = branch.Operand;
						if (!(operand is Instruction))
						{
							Instruction[] array = operand as Instruction[];
							if (array == null)
							{
								continue;
							}
							for (int i = 0; i < array.Length; i++)
							{
								if (array[i] == item.Instruction)
								{
									array[i] = ins;
									break;
								}
							}
						}
						else
						{
							branch.Operand = ins;
						}
					}
					item.Emitted = true;
					item.Instruction = ins;
				}
				_LabelsToMark.Clear();
			}
			if (_ExceptionHandlersToMark.Count != 0)
			{
				foreach (LabelledExceptionHandler item2 in _ExceptionHandlersToMark)
				{
					IL.Body.ExceptionHandlers.Add(new Mono.Cecil.Cil.ExceptionHandler(item2.HandlerType)
					{
						TryStart = _(item2.TryStart)?.Instruction,
						TryEnd = _(item2.TryEnd)?.Instruction,
						HandlerStart = _(item2.HandlerStart)?.Instruction,
						HandlerEnd = _(item2.HandlerEnd)?.Instruction,
						FilterStart = _(item2.FilterStart)?.Instruction,
						CatchType = item2.ExceptionType
					});
				}
				_ExceptionHandlersToMark.Clear();
			}
			return ins;
		}

		public unsafe override Label DefineLabel()
		{
			Label label = default(Label);
			*(int*)(&label) = labelCounter++;
			_LabelInfos[label] = new LabelInfo();
			return label;
		}

		public override void MarkLabel(Label loc)
		{
			if (_LabelInfos.TryGetValue(loc, out var value) && !value.Emitted)
			{
				_LabelsToMark.Add(value);
			}
		}

		public override LocalBuilder DeclareLocal(Type type)
		{
			return DeclareLocal(type, pinned: false);
		}

		public override LocalBuilder DeclareLocal(Type type, bool pinned)
		{
			int count = IL.Body.Variables.Count;
			object obj;
			if (c_LocalBuilder_params != 4)
			{
				if (c_LocalBuilder_params != 3)
				{
					if (c_LocalBuilder_params != 2)
					{
						if (c_LocalBuilder_params != 0)
						{
							throw new NotSupportedException();
						}
						obj = c_LocalBuilder.Invoke(new object[0]);
					}
					else
					{
						obj = c_LocalBuilder.Invoke(new object[2] { type, null });
					}
				}
				else
				{
					obj = c_LocalBuilder.Invoke(new object[3] { count, type, null });
				}
			}
			else
			{
				obj = c_LocalBuilder.Invoke(new object[4] { count, type, null, pinned });
			}
			LocalBuilder localBuilder = (LocalBuilder)obj;
			f_LocalBuilder_position?.SetValue(localBuilder, (ushort)count);
			f_LocalBuilder_is_pinned?.SetValue(localBuilder, pinned);
			TypeReference typeReference = _(type);
			if (pinned)
			{
				typeReference = new PinnedType(typeReference);
			}
			VariableDefinition variableDefinition = new VariableDefinition(typeReference);
			IL.Body.Variables.Add(variableDefinition);
			_Variables[localBuilder] = variableDefinition;
			return localBuilder;
		}

		private void Emit(Instruction ins)
		{
			ins.Offset = _ILOffset;
			_ILOffset += ins.GetSize();
			IL.Append(ProcessLabels(ins));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode)
		{
			Emit(IL.Create(_(opcode)));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, byte arg)
		{
			if (opcode.OperandType == System.Reflection.Emit.OperandType.ShortInlineVar || opcode.OperandType == System.Reflection.Emit.OperandType.InlineVar)
			{
				_EmitInlineVar(_(opcode), arg);
			}
			else
			{
				Emit(IL.Create(_(opcode), arg));
			}
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, sbyte arg)
		{
			if (opcode.OperandType == System.Reflection.Emit.OperandType.ShortInlineVar || opcode.OperandType == System.Reflection.Emit.OperandType.InlineVar)
			{
				_EmitInlineVar(_(opcode), arg);
			}
			else
			{
				Emit(IL.Create(_(opcode), arg));
			}
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, short arg)
		{
			if (opcode.OperandType == System.Reflection.Emit.OperandType.ShortInlineVar || opcode.OperandType == System.Reflection.Emit.OperandType.InlineVar)
			{
				_EmitInlineVar(_(opcode), arg);
			}
			else
			{
				Emit(IL.Create(_(opcode), arg));
			}
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, int arg)
		{
			if (opcode.OperandType == System.Reflection.Emit.OperandType.ShortInlineVar || opcode.OperandType == System.Reflection.Emit.OperandType.InlineVar)
			{
				_EmitInlineVar(_(opcode), arg);
			}
			else if (opcode.Name.EndsWith(".s", StringComparison.Ordinal))
			{
				Emit(IL.Create(_(opcode), (sbyte)arg));
			}
			else
			{
				Emit(IL.Create(_(opcode), arg));
			}
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, long arg)
		{
			Emit(IL.Create(_(opcode), arg));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, float arg)
		{
			Emit(IL.Create(_(opcode), arg));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, double arg)
		{
			Emit(IL.Create(_(opcode), arg));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, string arg)
		{
			Emit(IL.Create(_(opcode), arg));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, Type arg)
		{
			Emit(IL.Create(_(opcode), _(arg)));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, FieldInfo arg)
		{
			Emit(IL.Create(_(opcode), _(arg)));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, ConstructorInfo arg)
		{
			Emit(IL.Create(_(opcode), _(arg)));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, MethodInfo arg)
		{
			Emit(IL.Create(_(opcode), _(arg)));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, Label label)
		{
			LabelInfo labelInfo = _(label);
			Instruction instruction = IL.Create(_(opcode), _(label).Instruction);
			labelInfo.Branches.Add(instruction);
			Emit(ProcessLabels(instruction));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, Label[] labels)
		{
			IEnumerable<LabelInfo> enumerable = labels.Distinct().Select(_);
			Instruction instruction = IL.Create(_(opcode), enumerable.Select((LabelInfo labelInfo) => labelInfo.Instruction).ToArray());
			foreach (LabelInfo item in enumerable)
			{
				item.Branches.Add(instruction);
			}
			Emit(ProcessLabels(instruction));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, LocalBuilder local)
		{
			Emit(IL.Create(_(opcode), _(local)));
		}

		public override void Emit(System.Reflection.Emit.OpCode opcode, SignatureHelper signature)
		{
			Emit(IL.Create(_(opcode), IL.Body.Method.Module.ImportCallSite(signature)));
		}

		public void Emit(System.Reflection.Emit.OpCode opcode, ICallSiteGenerator signature)
		{
			Emit(IL.Create(_(opcode), IL.Body.Method.Module.ImportCallSite(signature)));
		}

		private void _EmitInlineVar(Mono.Cecil.Cil.OpCode opcode, int index)
		{
			switch (opcode.OperandType)
			{
			case Mono.Cecil.Cil.OperandType.InlineArg:
			case Mono.Cecil.Cil.OperandType.ShortInlineArg:
				Emit(IL.Create(opcode, IL.Body.Method.Parameters[index]));
				break;
			case Mono.Cecil.Cil.OperandType.InlineVar:
			case Mono.Cecil.Cil.OperandType.ShortInlineVar:
				Emit(IL.Create(opcode, IL.Body.Variables[index]));
				break;
			default:
				throw new NotSupportedException($"Unsupported SRE InlineVar -> Cecil {opcode.OperandType} for {opcode} {index}");
			}
		}

		public override void EmitCall(System.Reflection.Emit.OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
		{
			Emit(IL.Create(_(opcode), _(methodInfo)));
		}

		public override void EmitCalli(System.Reflection.Emit.OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			throw new NotSupportedException();
		}

		public override void EmitCalli(System.Reflection.Emit.OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			throw new NotSupportedException();
		}

		public override void EmitWriteLine(FieldInfo field)
		{
			if (field.IsStatic)
			{
				Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Ldsfld, _(field)));
			}
			else
			{
				Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Ldarg_0));
				Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Ldfld, _(field)));
			}
			Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Call, _(typeof(Console).GetMethod("WriteLine", new Type[1] { field.FieldType }))));
		}

		public override void EmitWriteLine(LocalBuilder localBuilder)
		{
			Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Ldloc, _(localBuilder)));
			Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Call, _(typeof(Console).GetMethod("WriteLine", new Type[1] { localBuilder.LocalType }))));
		}

		public override void EmitWriteLine(string value)
		{
			Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Ldstr, value));
			Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Call, _(typeof(Console).GetMethod("WriteLine", new Type[1] { typeof(string) }))));
		}

		public override void ThrowException(Type type)
		{
			Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Newobj, _(type.GetConstructor(Type.EmptyTypes))));
			Emit(IL.Create(Mono.Cecil.Cil.OpCodes.Throw));
		}

		public override Label BeginExceptionBlock()
		{
			ExceptionHandlerChain exceptionHandlerChain = new ExceptionHandlerChain(this);
			_ExceptionHandlers.Push(exceptionHandlerChain);
			return exceptionHandlerChain.SkipAll;
		}

		public override void BeginCatchBlock(Type exceptionType)
		{
			_ExceptionHandlers.Peek().BeginHandler(ExceptionHandlerType.Catch).ExceptionType = ((exceptionType == null) ? null : _(exceptionType));
		}

		public override void BeginExceptFilterBlock()
		{
			_ExceptionHandlers.Peek().BeginHandler(ExceptionHandlerType.Filter);
		}

		public override void BeginFaultBlock()
		{
			_ExceptionHandlers.Peek().BeginHandler(ExceptionHandlerType.Fault);
		}

		public override void BeginFinallyBlock()
		{
			_ExceptionHandlers.Peek().BeginHandler(ExceptionHandlerType.Finally);
		}

		public override void EndExceptionBlock()
		{
			_ExceptionHandlers.Pop().End();
		}

		public override void BeginScope()
		{
		}

		public override void EndScope()
		{
		}

		public override void UsingNamespace(string usingNamespace)
		{
		}
	}
}

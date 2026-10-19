using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ceras.Formatters
{
	internal class DelegateFormatter<T> : IFormatter<T>, IFormatter where T : Delegate
	{
		private readonly IFormatter<MethodInfo> _methodInfoFormatter;

		private readonly IFormatter<object> _targetFormatter;

		private readonly bool _allowStatic;

		private readonly bool _allowInstance;

		public DelegateFormatter(CerasSerializer ceras)
		{
			_methodInfoFormatter = ceras.GetFormatter<MethodInfo>();
			_targetFormatter = ceras.GetFormatter<object>();
			_allowStatic = (ceras.Config.Advanced.DelegateSerialization & DelegateSerializationFlags.AllowStatic) != 0;
			_allowInstance = (ceras.Config.Advanced.DelegateSerialization & DelegateSerializationFlags.AllowInstance) != 0;
		}

		public void Serialize(ref byte[] buffer, ref int offset, T del)
		{
			object target = del.Target;
			if (target != null)
			{
				if (!_allowInstance)
				{
					ThrowInstance(del, target);
				}
				if (target.GetType().GetCustomAttribute<CompilerGeneratedAttribute>() != null)
				{
					throw new InvalidOperationException($"The delegate '{del}' is targeting a 'lambda'. This makes it impossible to serialize because the compiler can (and will!) merge all lambda \"closures\" of the containing method or type, which is very dangerous even in the most simple scenarios. For more information of what exactly this means you should read this: 'https://github.com/rikimaru0345/Ceras/issues/11'. If you have a good use-case and/or a solution for the problems described in the link, open an issue on GitHub or join the Discord server...");
				}
			}
			else if (!_allowStatic)
			{
				ThrowStatic(del);
			}
			_targetFormatter.Serialize(ref buffer, ref offset, target);
			Delegate[] invocationList = del.GetInvocationList();
			if (invocationList.Length != 1)
			{
				throw new InvalidOperationException($"The delegate cannot be serialized, its 'invocation list' must have exactly one target, but it has '{invocationList.Length}'.");
			}
			_methodInfoFormatter.Serialize(ref buffer, ref offset, del.Method);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref T value)
		{
			object value2 = null;
			_targetFormatter.Deserialize(buffer, ref offset, ref value2);
			MethodInfo value3 = null;
			_methodInfoFormatter.Deserialize(buffer, ref offset, ref value3);
			if (value3.IsStatic && !_allowStatic)
			{
				ThrowStatic(null);
			}
			if (!value3.IsStatic && !_allowInstance)
			{
				ThrowInstance(null, value2);
			}
			if (value2 == null)
			{
				value = (T)Delegate.CreateDelegate(typeof(T), value3, throwOnBindFailure: true);
			}
			else
			{
				value = (T)Delegate.CreateDelegate(typeof(T), value2, value3, throwOnBindFailure: true);
			}
		}

		private static void ThrowStatic(T delegateValue)
		{
			throw new InvalidOperationException($"The delegate '{delegateValue}' can not be serialized/deserialized as it references a static method and your settings in 'config.Advanced.DelegateSerialization' don't allow serialization of static-delegates. Change the setting in your config, or exclude the member, ...");
		}

		private static void ThrowInstance(T delegateValue, object instance)
		{
			throw new InvalidOperationException($"The delegate '{delegateValue}' can not be serialized/deserialized as it references an instance method (targeting the object '{instance}') and your settings in 'config.Advanced.DelegateSerialization' don't allow serialization of instance-delegates. Change the setting in your config, or exclude the member, ...");
		}
	}
}

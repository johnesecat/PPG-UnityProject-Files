using System;
using System.Reflection;
using Ceras.Formatters;

namespace Ceras.Resolvers
{
	public sealed class ReflectionFormatterResolver : IFormatterResolver
	{
		private readonly CerasSerializer _ceras;

		public ReflectionFormatterResolver(CerasSerializer ceras)
		{
			_ceras = ceras;
		}

		public IFormatter GetFormatter(Type type)
		{
			if (typeof(MemberInfo).IsAssignableFrom(type))
			{
				return (IFormatter)Activator.CreateInstance(typeof(MemberInfoFormatter<>).MakeGenericType(type), _ceras);
			}
			if (typeof(MulticastDelegate).IsAssignableFrom(type))
			{
				if (_ceras.Config.Advanced.DelegateSerialization == DelegateSerializationFlags.Off)
				{
					throw new InvalidOperationException("The type '" + type.FullName + "' can not be serialized because it is a delegate; and 'config.Advanced.DelegateSerialization' is turned off.");
				}
				CerasSerializer.AddFormatterConstructedType(type);
				return (IFormatter)Activator.CreateInstance(typeof(DelegateFormatter<>).MakeGenericType(type), _ceras);
			}
			return null;
		}
	}
}

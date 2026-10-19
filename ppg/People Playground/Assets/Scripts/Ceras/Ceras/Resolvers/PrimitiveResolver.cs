using System;
using System.Collections.Generic;
using Ceras.Formatters;

namespace Ceras.Resolvers
{
	public sealed class PrimitiveResolver : IFormatterResolver
	{
		private static Dictionary<Type, IFormatter> _primitiveFormatters = new Dictionary<Type, IFormatter>
		{
			[typeof(bool)] = new BoolFormatter(),
			[typeof(byte)] = new ByteFormatter(),
			[typeof(sbyte)] = new SByteFormatter(),
			[typeof(char)] = new CharFormatter(),
			[typeof(short)] = new Int16Formatter(),
			[typeof(ushort)] = new UInt16Formatter(),
			[typeof(int)] = new Int32Formatter(),
			[typeof(uint)] = new UInt32Formatter(),
			[typeof(long)] = new Int64Formatter(),
			[typeof(ulong)] = new UInt64Formatter(),
			[typeof(float)] = new FloatFormatter(),
			[typeof(double)] = new DoubleFormatter(),
			[typeof(IntPtr)] = new IntPtrFormatter(),
			[typeof(UIntPtr)] = new UIntPtrFormatter()
		};

		private readonly CerasSerializer _ceras;

		public PrimitiveResolver(CerasSerializer ceras)
		{
			_ceras = ceras;
		}

		public IFormatter GetFormatter(Type type)
		{
			if (_primitiveFormatters.TryGetValue(type, out var value))
			{
				return value;
			}
			if (type.IsEnum)
			{
				return (IFormatter)Activator.CreateInstance(typeof(EnumFormatter<>).MakeGenericType(type), _ceras);
			}
			return null;
		}
	}
}

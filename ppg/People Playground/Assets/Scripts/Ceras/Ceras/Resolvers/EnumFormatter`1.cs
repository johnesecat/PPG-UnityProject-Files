using System;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Formatters;

namespace Ceras.Resolvers
{
	public sealed class EnumFormatter<T> : IFormatter<T>, IFormatter where T : Enum
	{
		private delegate void WriteEnum(ref byte[] buffer, ref int offset, T enumVal);

		private delegate void ReadEnum(byte[] buffer, ref int offset, out T enumVal);

		private WriteEnum _enumWriter;

		private ReadEnum _enumReader;

		public EnumFormatter(CerasSerializer serializer)
		{
			if (serializer.Config.Advanced.AotMode != 0)
			{
				throw new InvalidOperationException("The default enum formatter can not be used in AoT mode. Ceras should have automatically selected an alternative formatter, so this must be a bug. Please report it on GitHub!");
			}
			ParameterExpression parameterExpression = Expression.Parameter(typeof(byte[]).MakeByRefType(), "buffer");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(int).MakeByRefType(), "offset");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T), "value");
			Type enumUnderlyingType = typeof(T).GetEnumUnderlyingType();
			IFormatter specificFormatter = serializer.GetSpecificFormatter(enumUnderlyingType);
			MethodInfo method = specificFormatter.GetType().GetMethod("Serialize", BindingFlags.Instance | BindingFlags.Public);
			UnaryExpression arg = Expression.Convert(parameterExpression3, enumUnderlyingType);
			MethodCallExpression body = Expression.Call(Expression.Constant(specificFormatter), method, parameterExpression, parameterExpression2, arg);
			_enumWriter = Expression.Lambda<WriteEnum>(body, new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
			ParameterExpression parameterExpression4 = Expression.Parameter(typeof(byte[]), "buffer");
			ParameterExpression parameterExpression5 = Expression.Parameter(typeof(T).MakeByRefType(), "value");
			MethodInfo method2 = specificFormatter.GetType().GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public);
			ParameterExpression parameterExpression6 = Expression.Variable(enumUnderlyingType, "temp");
			MethodCallExpression methodCallExpression = Expression.Call(Expression.Constant(specificFormatter), method2, parameterExpression4, parameterExpression2, parameterExpression6);
			UnaryExpression right = Expression.Convert(parameterExpression6, typeof(T));
			BinaryExpression binaryExpression = Expression.Assign(parameterExpression5, right);
			BlockExpression body2 = Expression.Block(new ParameterExpression[1] { parameterExpression6 }, methodCallExpression, binaryExpression);
			_enumReader = Expression.Lambda<ReadEnum>(body2, new ParameterExpression[3] { parameterExpression4, parameterExpression2, parameterExpression5 }).Compile();
		}

		public void Serialize(ref byte[] buffer, ref int offset, T value)
		{
			_enumWriter(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref T value)
		{
			_enumReader(buffer, ref offset, out value);
		}
	}
}

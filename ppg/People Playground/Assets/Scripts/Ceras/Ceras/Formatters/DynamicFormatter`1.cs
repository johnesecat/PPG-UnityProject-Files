using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Helpers;

namespace Ceras.Formatters
{
	internal sealed class DynamicFormatter<T> : IFormatter<T>, IFormatter
	{
		private const int FieldSizePrefixBytes = 4;

		private static readonly Type _sizeType = typeof(uint);

		private static readonly MethodInfo _sizeWriteMethod = typeof(SerializerBinary).GetMethod("WriteUInt32Fixed");

		private static readonly MethodInfo _sizeReadMethod = typeof(SerializerBinary).GetMethod("ReadUInt32Fixed");

		private static readonly MethodInfo _offsetMismatchMethod = ReflectionHelper.GetMethod(() => ThrowOffsetMismatch(0, 0, 0));

		private readonly CerasSerializer _ceras;

		private SerializeDelegate<T> _serializer;

		private DeserializeDelegate<T> _deserializer;

		public DynamicFormatter(CerasSerializer serializer, bool isStatic)
		{
			_ceras = serializer;
			Type typeFromHandle = typeof(T);
			BannedTypes.ThrowIfNonspecific(typeFromHandle);
			Schema schema = (isStatic ? _ceras.GetStaticTypeMetaData(typeFromHandle).PrimarySchema : _ceras.GetTypeMetaData(typeFromHandle).PrimarySchema);
			_ceras.Config.GetTypeConfig(typeFromHandle, isStatic).VerifyConstructionMethod();
			if (!schema.IsPrimary)
			{
				throw new InvalidOperationException("Non-Primary Schema requires SchemaFormatter instead of DynamicFormatter!");
			}
			if (schema.Members.Count == 0)
			{
				_serializer = delegate
				{
				};
				_deserializer = delegate
				{
				};
			}
			else
			{
				_serializer = GenerateSerializer(_ceras, schema, isSchemaFormatter: false, isStatic).Compile();
				_deserializer = GenerateDeserializer(_ceras, schema, isSchemaFormatter: false, isStatic).Compile();
			}
		}

		public void Serialize(ref byte[] buffer, ref int offset, T value)
		{
			_serializer(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref T value)
		{
			_deserializer(buffer, ref offset, ref value);
		}

		internal static Expression<SerializeDelegate<T>> GenerateSerializer(CerasSerializer ceras, Schema schema, bool isSchemaFormatter, bool isStatic)
		{
			List<SchemaMember> members = schema.Members;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(byte[]).MakeByRefType(), "buffer");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(int).MakeByRefType(), "offset");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T), "value");
			if (isStatic)
			{
				parameterExpression3 = null;
			}
			List<Expression> list = new List<Expression>();
			List<ParameterExpression> list2 = new List<ParameterExpression>();
			ParameterExpression parameterExpression4 = null;
			ParameterExpression parameterExpression5 = null;
			if (isSchemaFormatter)
			{
				list2.Add(parameterExpression4 = Expression.Variable(typeof(int), "startPos"));
				list2.Add(parameterExpression5 = Expression.Variable(typeof(int), "size"));
			}
			Dictionary<Type, ConstantExpression> dictionary = new Dictionary<Type, ConstantExpression>();
			foreach (SchemaMember item2 in members.Where((SchemaMember m) => !m.IsSkip).DistinctBy((SchemaMember m) => m.MemberType))
			{
				dictionary.Add(item2.MemberType, Expression.Constant(ceras.GetReferenceFormatter(item2.MemberType)));
			}
			foreach (SchemaMember item3 in members)
			{
				if (!item3.IsSkip)
				{
					ConstantExpression constantExpression = dictionary[item3.MemberType];
					MethodInfo method = constantExpression.Value.GetType().GetMethod("Serialize");
					MemberExpression arg = Expression.MakeMemberAccess(parameterExpression3, item3.MemberInfo);
					if (!isSchemaFormatter)
					{
						MethodCallExpression item = Expression.Call(constantExpression, method, parameterExpression, parameterExpression2, arg);
						list.Add(item);
						continue;
					}
					list.Add(Expression.Assign(parameterExpression4, parameterExpression2));
					list.Add(Expression.AddAssign(parameterExpression2, Expression.Constant(4)));
					list.Add(Expression.Call(constantExpression, method, parameterExpression, parameterExpression2, Expression.MakeMemberAccess(parameterExpression3, item3.MemberInfo)));
					list.Add(Expression.Assign(parameterExpression5, Expression.Subtract(Expression.Subtract(parameterExpression2, parameterExpression4), Expression.Constant(4))));
					list.Add(Expression.Assign(parameterExpression2, parameterExpression4));
					list.Add(Expression.Call(_sizeWriteMethod, parameterExpression, parameterExpression2, Expression.Convert(parameterExpression5, _sizeType)));
					list.Add(Expression.Assign(parameterExpression2, Expression.Add(Expression.Add(parameterExpression4, parameterExpression5), Expression.Constant(4))));
				}
			}
			BlockExpression body = Expression.Block(list2, list);
			if (isStatic)
			{
				parameterExpression3 = Expression.Parameter(typeof(T), "value");
			}
			return Expression.Lambda<SerializeDelegate<T>>(body, new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 });
		}

		internal static Expression<DeserializeDelegate<T>> GenerateDeserializer(CerasSerializer ceras, Schema schema, bool isSchemaFormatter, bool isStatic)
		{
			bool flag = isSchemaFormatter && ceras.Config.VersionTolerance.VerifySizes;
			List<SchemaMember> members = schema.Members;
			TypeConfig typeConfig = ceras.Config.GetTypeConfig(schema.Type, isStatic);
			TypeConstruction typeConstruction = typeConfig.TypeConstruction;
			bool hasDataArguments = typeConstruction.HasDataArguments;
			HashSet<ParameterExpression> hashSet = null;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(byte[]), "buffer");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(int).MakeByRefType(), "offset");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(T).MakeByRefType(), "value");
			if (isStatic)
			{
				parameterExpression3 = null;
			}
			List<Expression> list = new List<Expression>();
			List<ParameterExpression> list2 = new List<ParameterExpression>(schema.Members.Count);
			MethodInfo onAfterDeserialize = GetOnAfterDeserialize(schema.Type);
			ParameterExpression parameterExpression4 = null;
			ParameterExpression parameterExpression5 = null;
			if (isSchemaFormatter)
			{
				list2.Add(parameterExpression4 = Expression.Variable(typeof(int), "blockSize"));
				list2.Add(parameterExpression5 = Expression.Variable(typeof(int), "offsetStart"));
			}
			Dictionary<MemberInfo, ParameterExpression> memberInfoToLocal = new Dictionary<MemberInfo, ParameterExpression>();
			foreach (SchemaMember item in members)
			{
				if (!item.IsSkip)
				{
					ParameterExpression parameterExpression6 = Expression.Variable(item.MemberType, item.MemberName + "_local");
					list2.Add(parameterExpression6);
					memberInfoToLocal.Add(item.MemberInfo, parameterExpression6);
				}
			}
			Dictionary<Type, ConstantExpression> dictionary = new Dictionary<Type, ConstantExpression>();
			foreach (SchemaMember item2 in members.Where((SchemaMember m) => !m.IsSkip).DistinctBy((SchemaMember m) => m.MemberType))
			{
				dictionary.Add(item2.MemberType, Expression.Constant(ceras.GetReferenceFormatter(item2.MemberType)));
			}
			foreach (SchemaMember item3 in members)
			{
				if (!hasDataArguments && !item3.IsSkip)
				{
					ParameterExpression left = memberInfoToLocal[item3.MemberInfo];
					list.Add(Expression.Assign(left, Expression.MakeMemberAccess(parameterExpression3, item3.MemberInfo)));
				}
			}
			foreach (SchemaMember item4 in members)
			{
				if (isSchemaFormatter)
				{
					MethodCallExpression expression = Expression.Call(_sizeReadMethod, parameterExpression, parameterExpression2);
					list.Add(Expression.Assign(parameterExpression4, Expression.Convert(expression, typeof(int))));
					if (flag)
					{
						list.Add(Expression.Assign(parameterExpression5, parameterExpression2));
					}
					if (item4.IsSkip)
					{
						list.Add(Expression.AddAssign(parameterExpression2, parameterExpression4));
						continue;
					}
				}
				if (item4.IsSkip && !isSchemaFormatter)
				{
					throw new InvalidOperationException("DynamicFormatter can not skip members in non-schema mode");
				}
				ConstantExpression constantExpression = dictionary[item4.MemberType];
				MethodInfo method = constantExpression.Value.GetType().GetMethod("Deserialize");
				ParameterExpression arg = memberInfoToLocal[item4.MemberInfo];
				list.Add(Expression.Call(constantExpression, method, parameterExpression, parameterExpression2, arg));
				if (isSchemaFormatter && flag)
				{
					list.Add(Expression.IfThen(Expression.NotEqual(Expression.Add(parameterExpression5, parameterExpression4), parameterExpression2), Expression.Call(null, _offsetMismatchMethod, parameterExpression5, parameterExpression2, parameterExpression4)));
				}
			}
			if (hasDataArguments)
			{
				MemberParameterPair[] memberParameters = (from m in schema.Members
					where !m.IsSkip
					select new
					{
						m = m,
						local = memberInfoToLocal[m.MemberInfo]
					}).Select(<>h__TransparentIdentifier0 =>
				{
					MemberParameterPair result = default(MemberParameterPair);
					result.LocalVar = <>h__TransparentIdentifier0.local;
					result.Member = <>h__TransparentIdentifier0.m.MemberInfo;
					return result;
				}).ToArray();
				hashSet = new HashSet<ParameterExpression>();
				typeConstruction.EmitConstruction(schema, list, parameterExpression3, hashSet, memberParameters);
			}
			foreach (SchemaMember i in OrderMembersForWriteBack(members))
			{
				if (i.IsSkip)
				{
					continue;
				}
				ParameterExpression parameterExpression7 = memberInfoToLocal[i.MemberInfo];
				Type memberType = i.MemberType;
				if ((hashSet != null && hashSet.Contains(parameterExpression7)) || i.IsSkip)
				{
					continue;
				}
				FieldInfo fieldInfo = i.MemberInfo as FieldInfo;
				if ((object)fieldInfo != null)
				{
					if (fieldInfo.IsInitOnly)
					{
						ReadonlyFieldHandling readonlyFieldHandling = typeConfig.Members.First((MemberConfig x) => x.Member == i.MemberInfo).ComputeReadonlyHandling();
						DynamicFormatterHelpers.EmitReadonlyWriteBack(memberType, readonlyFieldHandling, fieldInfo, parameterExpression3, parameterExpression7, list);
					}
					else
					{
						list.Add(Expression.Assign(Expression.Field(parameterExpression3, fieldInfo), parameterExpression7));
					}
				}
				else
				{
					MethodInfo setMethod = ((PropertyInfo)i.MemberInfo).GetSetMethod(nonPublic: true);
					list.Add(Expression.Call(parameterExpression3, setMethod, parameterExpression7));
				}
			}
			if (onAfterDeserialize != null)
			{
				list.Add(Expression.Call(parameterExpression3, onAfterDeserialize));
			}
			BlockExpression body = Expression.Block(list2, list);
			if (isStatic)
			{
				parameterExpression3 = Expression.Parameter(typeof(T).MakeByRefType(), "value");
			}
			return Expression.Lambda<DeserializeDelegate<T>>(body, new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 });
		}

		private static MethodInfo GetOnAfterDeserialize(Type type)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.ReturnType == typeof(void) && methodInfo.GetParameters().Length == 0 && methodInfo.GetCustomAttribute<OnAfterDeserializeAttribute>() != null)
				{
					return methodInfo;
				}
			}
			return null;
		}

		private static IEnumerable<SchemaMember> OrderMembersForWriteBack(List<SchemaMember> members)
		{
			return from m in members
				orderby m.WriteBackOrder, members.IndexOf(m)
				select m;
		}

		private static void ThrowOffsetMismatch(int startOffset, int offset, int blockSize)
		{
			throw new InvalidOperationException($"The data being read is corrupted. The amount of data read did not match the expected block-size! BlockStart:{startOffset} BlockSize:{blockSize} CurrentOffset:{offset}");
		}
	}
}

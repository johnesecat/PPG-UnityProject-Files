using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Exceptions;
using Ceras.Helpers;

namespace Ceras.Formatters
{
	internal static class DynamicFormatterHelpers
	{
		private static readonly MethodInfo _setValue = typeof(FieldInfo).GetMethod("SetValue", BindingFlags.Instance | BindingFlags.Public, null, new Type[2]
		{
			typeof(object),
			typeof(object)
		}, new ParameterModifier[2]);

		internal static void EmitReadonlyWriteBack(Type type, ReadonlyFieldHandling readonlyFieldHandling, FieldInfo fieldInfo, ParameterExpression refValueArg, ParameterExpression tempStore, List<Expression> block)
		{
			if (readonlyFieldHandling == ReadonlyFieldHandling.ExcludeFromSerialization)
			{
				throw new InvalidOperationException("Error while trying to generate a deserializer for the field '" + fieldInfo.DeclaringType.FriendlyName(fullName: true) + "." + fieldInfo.Name + "': the field is readonly, but ReadonlyFieldHandling is turned off in the configuration.");
			}
			if (type.IsValueType)
			{
				block.Add(Expression.IfThenElse(ifFalse: (readonlyFieldHandling != ReadonlyFieldHandling.ForcedOverwrite) ? ((Expression)Expression.Throw(Expression.Constant(new CerasException("The value-type in field '" + fieldInfo.Name + "' does not match the expected value, but the field is readonly and overwriting is not allowed in the configuration. Make the field writeable or enable 'ForcedOverwrite' in the serializer settings to allow Ceras to overwrite the readonly-field.")))) : ((Expression)Expression.Call(Expression.Constant(fieldInfo), _setValue, refValueArg, Expression.Convert(tempStore, typeof(object)))), test: Expression.Equal(tempStore, Expression.MakeMemberAccess(refValueArg, fieldInfo)), ifTrue: Expression.Empty()));
			}
			else
			{
				block.Add(Expression.IfThenElse(ifFalse: (readonlyFieldHandling != ReadonlyFieldHandling.ForcedOverwrite) ? ((Expression)Expression.Throw(Expression.Constant(new CerasException("The reference in the readonly-field '" + fieldInfo.Name + "' would have to be overwritten, but forced overwriting is not enabled in the serializer settings. Either make the field writeable or enable ForcedOverwrite in the ReadonlyFieldHandling-setting.")))) : ((Expression)Expression.Call(Expression.Constant(fieldInfo), _setValue, refValueArg, tempStore)), test: Expression.ReferenceEqual(tempStore, Expression.MakeMemberAccess(refValueArg, fieldInfo)), ifTrue: Expression.Empty()));
			}
		}

		internal static void EmitBatchReadWrite()
		{
		}
	}
}

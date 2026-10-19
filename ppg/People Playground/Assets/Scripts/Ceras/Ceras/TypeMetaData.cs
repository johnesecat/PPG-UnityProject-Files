using System;
using System.Collections.Generic;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras
{
	internal class TypeMetaData
	{
		public readonly Type Type;

		public readonly bool IsFrameworkType;

		public readonly bool IsPrimitive;

		public readonly bool IsValueType;

		public readonly TypeConfig TypeConfig;

		public IFormatter SpecificFormatter;

		public IFormatter ReferenceFormatter;

		public Schema CurrentSchema;

		public Schema PrimarySchema;

		public readonly List<Schema> SecondarySchemata = new List<Schema>();

		public readonly List<ISchemaTaintedFormatter> OnSchemaChangeTargets = new List<ISchemaTaintedFormatter>();

		public bool HasSchema
		{
			get
			{
				if (!IsPrimitive)
				{
					return !IsFrameworkType;
				}
				return false;
			}
		}

		public TypeMetaData(Type type, TypeConfig typeConfig, bool isFrameworkType, bool isSerializationPrimitive)
		{
			Type = type;
			IsFrameworkType = isFrameworkType;
			IsPrimitive = isSerializationPrimitive;
			IsValueType = type.IsValueType;
			TypeConfig = typeConfig;
		}
	}
}

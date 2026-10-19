using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ceras.Helpers
{
	internal class Schema
	{
		private int _hash = -1;

		public Type Type { get; }

		public TypeConfig TypeConfig { get; }

		public bool IsStatic { get; }

		public bool IsPrimary { get; }

		public List<SchemaMember> Members { get; } = new List<SchemaMember>();


		public Schema(bool isPrimary, Type type, TypeConfig typeConfig, bool isStatic)
		{
			IsPrimary = isPrimary;
			Type = type;
			TypeConfig = typeConfig;
			IsStatic = isStatic;
		}

		protected bool Equals(Schema other)
		{
			if (Type != other.Type)
			{
				return false;
			}
			if (Members.Count != other.Members.Count)
			{
				return false;
			}
			for (int i = 0; i < Members.Count; i++)
			{
				SchemaMember schemaMember = Members[i];
				SchemaMember schemaMember2 = other.Members[i];
				if (schemaMember.PersistentName != schemaMember2.PersistentName)
				{
					return false;
				}
				if (schemaMember.IsSkip != schemaMember2.IsSkip)
				{
					return false;
				}
				if (schemaMember.MemberInfo != schemaMember2.MemberInfo)
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((Schema)obj);
		}

		public override int GetHashCode()
		{
			if (_hash == -1)
			{
				string text = Type.FullName + string.Join("", Members.Select((SchemaMember m) => m.IsSkip ? "skip" : (m.MemberType.FullName + m.MemberInfo.Name)));
				_hash = text.GetHashCode();
			}
			return _hash;
		}

		internal static MemberInfo FindMemberInType(Type type, string name, bool isStatic)
		{
			foreach (MemberInfo item in isStatic ? type.GetAllStaticDataMembers() : type.GetAllDataMembers())
			{
				FieldInfo fieldInfo = item as FieldInfo;
				if ((object)fieldInfo != null)
				{
					if (IsMatch(fieldInfo, name))
					{
						return fieldInfo;
					}
					continue;
				}
				PropertyInfo propertyInfo = item as PropertyInfo;
				if ((object)propertyInfo != null && IsMatch(propertyInfo, name))
				{
					return propertyInfo;
				}
			}
			return null;
		}

		private static bool IsMatch(MemberInfo member, string name)
		{
			if (member.Name == name)
			{
				return true;
			}
			PreviousNameAttribute customAttribute = member.GetCustomAttribute<PreviousNameAttribute>();
			if (customAttribute != null)
			{
				if (customAttribute.Name == name)
				{
					return true;
				}
				if (customAttribute.AlternativeNames.Any((string n) => n == name))
				{
					return true;
				}
			}
			return false;
		}
	}
}

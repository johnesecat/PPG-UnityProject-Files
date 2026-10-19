using System;
using System.Reflection;

namespace Ceras.Helpers
{
	internal class SchemaMember
	{
		public string PersistentName { get; }

		public MemberInfo Member { get; }

		public int WriteBackOrder { get; }

		public MemberInfo MemberInfo => Member;

		public Type MemberType
		{
			get
			{
				FieldInfo fieldInfo = Member as FieldInfo;
				if ((object)fieldInfo == null)
				{
					return ((PropertyInfo)Member).PropertyType;
				}
				return fieldInfo.FieldType;
			}
		}

		public string MemberName => Member.Name;

		public bool IsSkip => MemberInfo == null;

		public SchemaMember(string persistentName, MemberInfo memberInfo, int writeBackOrder)
		{
			if (memberInfo == null)
			{
				throw new ArgumentNullException("memberInfo");
			}
			if (memberInfo.DeclaringType == null)
			{
				throw new Exception("declaring type is null");
			}
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if ((object)propertyInfo != null && (!propertyInfo.CanRead || !propertyInfo.CanWrite))
			{
				throw new Exception("property must be readable and writable");
			}
			PersistentName = persistentName;
			Member = memberInfo;
			WriteBackOrder = writeBackOrder;
		}

		public SchemaMember(string persistentName)
		{
			PersistentName = persistentName;
			Member = null;
			WriteBackOrder = 0;
		}

		public override string ToString()
		{
			string text = PersistentName ?? "";
			if (IsSkip)
			{
				text = "[SKIP] " + text;
			}
			return text;
		}
	}
}

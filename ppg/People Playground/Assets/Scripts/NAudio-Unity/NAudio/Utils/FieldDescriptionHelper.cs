using System;
using System.Reflection;

namespace NAudio.Utils
{
	public static class FieldDescriptionHelper
	{
		public static string Describe(global::System.Type t, Guid guid)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			FieldInfo[] fields = t.GetFields((BindingFlags)24);
			foreach (FieldInfo val in fields)
			{
				if (!val.get_IsPublic() || !val.get_IsStatic() || val.get_FieldType() != typeof(Guid) || !((Guid)val.GetValue((object)null) == guid))
				{
					continue;
				}
				object[] customAttributes = ((MemberInfo)val).GetCustomAttributes(false);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					FieldDescriptionAttribute fieldDescriptionAttribute = customAttributes[j] as FieldDescriptionAttribute;
					if (fieldDescriptionAttribute != null)
					{
						return fieldDescriptionAttribute.Description;
					}
				}
				return ((MemberInfo)val).get_Name();
			}
			return ((object)(Guid)(ref guid)).ToString();
		}
	}
}

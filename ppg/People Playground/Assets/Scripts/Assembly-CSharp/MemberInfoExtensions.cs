using System;
using System.Linq;
using System.Reflection;

public static class MemberInfoExtensions
{
	public static T GetCustomAttribute<T>(this MemberInfo mem) where T : Attribute
	{
		return mem.GetCustomAttributes(typeof(T), inherit: true).FirstOrDefault() as T;
	}
}

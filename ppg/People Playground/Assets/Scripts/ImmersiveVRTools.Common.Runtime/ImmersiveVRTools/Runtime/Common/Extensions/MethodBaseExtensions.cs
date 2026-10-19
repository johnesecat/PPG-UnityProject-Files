using System.Linq;
using System.Reflection;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class MethodBaseExtensions
	{
		public static string ResolveFullName(this MethodBase method)
		{
			if (method == null)
			{
				return string.Empty;
			}
			return method.ReflectedType.FullName + "." + method.Name + "(" + string.Join(",", (from o in method.GetParameters()
				select $"{o.ParameterType}").ToArray()) + ")";
		}
	}
}

using System.Reflection;
using Mono.Cecil;

namespace MonoMod.Utils
{
	internal class DynamicMethodReference : MethodReference
	{
		public MethodInfo DynamicMethod;

		public DynamicMethodReference(ModuleDefinition module, MethodInfo dm)
			: base("", module.TypeSystem.Void)
		{
			DynamicMethod = dm;
		}
	}
}

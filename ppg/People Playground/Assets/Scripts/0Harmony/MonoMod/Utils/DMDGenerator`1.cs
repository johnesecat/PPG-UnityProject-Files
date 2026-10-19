using System.Reflection;
using System.Reflection.Emit;

namespace MonoMod.Utils
{
	public abstract class DMDGenerator<TSelf> : _IDMDGenerator where TSelf : DMDGenerator<TSelf>, new()
	{
		private static TSelf _Instance;

		protected abstract MethodInfo _Generate(DynamicMethodDefinition dmd, object context);

		MethodInfo _IDMDGenerator.Generate(DynamicMethodDefinition dmd, object context)
		{
			return _Postbuild(_Generate(dmd, context));
		}

		public static MethodInfo Generate(DynamicMethodDefinition dmd, object context = null)
		{
			return _Postbuild((_Instance ?? (_Instance = new TSelf()))._Generate(dmd, context));
		}

		internal static MethodInfo _Postbuild(MethodInfo mi)
		{
			if (mi == null)
			{
				return null;
			}
			if (ReflectionHelper.IsMono && !(mi is DynamicMethod) && mi.DeclaringType != null)
			{
				Module module = mi?.Module;
				if (module == null)
				{
					return mi;
				}
				Assembly assembly = module.Assembly;
				if (assembly?.GetType() == null)
				{
					return mi;
				}
				assembly.SetMonoCorlibInternal(value: true);
			}
			return mi;
		}
	}
}

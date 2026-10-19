using System;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras.Resolvers
{
	public sealed class ReinterpretFormatterResolver : IFormatterResolver
	{
		private readonly CerasSerializer _ceras;

		public ReinterpretFormatterResolver(CerasSerializer ceras)
		{
			_ceras = ceras;
		}

		public IFormatter GetFormatter(Type type)
		{
			if (!_ceras.Config.Advanced.UseReinterpretFormatter)
			{
				return null;
			}
			if (!ReflectionHelper.IsBlittableType(type))
			{
				return null;
			}
			return (IFormatter)Activator.CreateInstance(typeof(ReinterpretFormatter<>).MakeGenericType(type));
		}
	}
}

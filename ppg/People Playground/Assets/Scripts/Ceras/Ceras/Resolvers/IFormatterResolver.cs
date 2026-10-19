using System;
using Ceras.Formatters;

namespace Ceras.Resolvers
{
	public interface IFormatterResolver
	{
		IFormatter GetFormatter(Type type);
	}
}

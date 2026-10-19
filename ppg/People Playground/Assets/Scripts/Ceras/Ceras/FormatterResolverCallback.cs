using System;
using Ceras.Formatters;

namespace Ceras
{
	public delegate IFormatter FormatterResolverCallback(CerasSerializer ceras, Type typeToBeFormatted);
}

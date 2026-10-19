using System;
using System.Collections.Generic;
using Ceras.Resolvers;

namespace Ceras
{
	public interface ICerasAdvanced
	{
		byte[] SerializeStatic(Type type);

		void DeserializeStatic(Type type, byte[] buffer);

		Type PeekType(byte[] buffer);

		IEnumerable<IFormatterResolver> GetFormatterResolvers();

		IFormatterResolver GetFormatterResolver<TResolver>() where TResolver : IFormatterResolver;
	}
}

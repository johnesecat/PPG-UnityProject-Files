using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ceras.Helpers
{
	internal sealed class TypeDictionaryDebugView<V>
	{
		private readonly TypeDictionary<V> _dictionary;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<Type, V>[] Items => _dictionary.ToArray();

		public TypeDictionaryDebugView(TypeDictionary<V> dictionary)
		{
			_dictionary = dictionary ?? throw new ArgumentNullException("dictionary");
		}
	}
}

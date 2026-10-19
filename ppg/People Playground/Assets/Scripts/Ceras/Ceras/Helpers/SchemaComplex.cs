using System.Collections.Generic;

namespace Ceras.Helpers
{
	internal class SchemaComplex
	{
		private readonly List<Schema> _schemata;

		private readonly int _hash;

		public SchemaComplex(List<Schema> schemata)
		{
			_schemata = schemata;
			_hash = CalculateHash();
		}

		private int CalculateHash()
		{
			int num = 17;
			for (int i = 0; i < _schemata.Count; i++)
			{
				num = num * 31 + _schemata[i].GetHashCode();
			}
			return num;
		}

		public override int GetHashCode()
		{
			return _hash;
		}

		public override bool Equals(object obj)
		{
			SchemaComplex schemaComplex = obj as SchemaComplex;
			if (schemaComplex == null)
			{
				return false;
			}
			if (_hash != schemaComplex._hash)
			{
				return false;
			}
			if (_schemata.Count != schemaComplex._schemata.Count)
			{
				return false;
			}
			for (int i = 0; i < _schemata.Count; i++)
			{
				if (!_schemata[i].Equals(schemaComplex._schemata[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}

using System.Runtime.CompilerServices;

namespace Steamworks.Data
{
	internal struct NumericalFilter
	{
		public string Key
		{
			[IsReadOnly]
			get;
			internal set; }

		public int Value
		{
			[IsReadOnly]
			get;
			internal set; }

		public LobbyComparison Comparer
		{
			[IsReadOnly]
			get;
			internal set; }

		internal NumericalFilter(string k, int v, LobbyComparison c)
		{
			Key = k;
			Value = v;
			Comparer = c;
		}
	}
}

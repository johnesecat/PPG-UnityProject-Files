using System.Collections.Generic;

namespace Ceras.Formatters
{
	public abstract class CollectionByListProxyFormatter<TCollection, TItem> : CollectionByProxyFormatter<TCollection, TItem, List<TItem>> where TCollection : ICollection<TItem>
	{
		protected sealed override List<TItem> CreateProxy(int knownSize)
		{
			return new List<TItem>();
		}

		protected sealed override void AddToProxy(List<TItem> builder, TItem item)
		{
			builder.Add(item);
		}
	}
}

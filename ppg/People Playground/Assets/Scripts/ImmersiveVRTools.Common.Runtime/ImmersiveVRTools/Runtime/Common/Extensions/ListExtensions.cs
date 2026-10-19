using System.Collections.Generic;
using System.Linq;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class ListExtensions
	{
		public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
		{
			return (from x in source.Select((T x, int i) => new
				{
					Index = i,
					Value = x
				})
				group x by x.Index / chunkSize into x
				select x.Select(v => v.Value).ToList()).ToList();
		}
	}
}

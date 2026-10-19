using System.Collections.Generic;
using UnityEngine;

public class ResolutionComparer : IEqualityComparer<Resolution>
{
	public bool Equals(Resolution x, Resolution y)
	{
		if (x.height == y.height)
		{
			return x.width == y.width;
		}
		return false;
	}

	public int GetHashCode(Resolution obj)
	{
		return obj.height.GetHashCode() ^ obj.width.GetHashCode();
	}
}

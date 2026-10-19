using System;

namespace Linefy
{
	[Serializable]
	public struct PolygonCorner
	{
		public int position;

		public int uv;

		public int color;

		public PolygonCorner(int position, int uv, int color)
		{
			this.position = position;
			this.uv = uv;
			this.color = color;
		}

		public override string ToString()
		{
			return $" <{position},{uv},{color}> ";
		}
	}
}

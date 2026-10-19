using System;
using UnityEngine;

namespace Linefy
{
	[Serializable]
	public struct Polygon
	{
		public int smoothingGroup;

		public int materialId;

		public PolygonCorner[] corners;

		public PolygonCorner this[int idx]
		{
			get
			{
				return corners[idx];
			}
			set
			{
				corners[idx] = value;
			}
		}

		public int CornersCount => corners.Length;

		public bool isValid
		{
			get
			{
				if (corners.Length < 3)
				{
					return false;
				}
				for (int i = 0; i < corners.Length; i++)
				{
					for (int j = 0; j < corners.Length; j++)
					{
						if (i != j && corners[i].position == corners[j].position)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public Polygon(int sg, int materialId, int cornersCount)
		{
			smoothingGroup = sg;
			this.materialId = materialId;
			corners = new PolygonCorner[cornersCount];
		}

		[Obsolete(" Obsolete constructor , use Polygon(int sg, int materialId, int cornersCount) instead ")]
		public Polygon(int sg, string str, int cornersCount)
		{
			smoothingGroup = sg;
			materialId = 0;
			corners = new PolygonCorner[cornersCount];
		}

		public Polygon(int cornersCount)
		{
			smoothingGroup = 0;
			materialId = 0;
			corners = new PolygonCorner[cornersCount];
		}

		public Polygon(params PolygonCorner[] corners)
		{
			smoothingGroup = 0;
			materialId = 0;
			this.corners = corners;
		}

		public Polygon(int sg, int materialId, params PolygonCorner[] corners)
		{
			smoothingGroup = sg;
			this.materialId = materialId;
			this.corners = corners;
		}

		public void SetCorner(int idx, int pos, int uv, int color)
		{
			corners[idx] = new PolygonCorner(pos, uv, color);
		}

		public void ClampCornerIndices(int posLength, int colorsLength, int uvLength)
		{
			for (int i = 0; i < corners.Length; i++)
			{
				corners[i].position = Mathf.Clamp(corners[i].position, 0, posLength);
				corners[i].color = Mathf.Clamp(corners[i].color, 0, colorsLength);
				corners[i].uv = Mathf.Clamp(corners[i].uv, 0, uvLength);
			}
		}

		public override string ToString()
		{
			string text = "";
			for (int i = 0; i < corners.Length; i++)
			{
				text += corners[i].ToString();
			}
			return $"sg:{smoothingGroup} mat:{materialId} corners:{text}";
		}
	}
}

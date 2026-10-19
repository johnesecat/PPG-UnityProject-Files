using System.Collections.Generic;
using UnityEngine;

namespace Linefy.Internal
{
	public class RectDistribution
	{
		private struct Cell
		{
			public float size;

			public Vector2 position;

			public Vector2 center;

			public bool isNull;

			public Vector2 pointPos;

			public int[] adjacents;

			public void Draw(bool drawGrid, bool drawPoint)
			{
				if (drawGrid)
				{
					Vector2 vector = position;
					Vector2 vector2 = new Vector2(position.x, position.y + size);
					Vector2 vector3 = new Vector2(position.x + size, position.y + size);
					Vector2 vector4 = new Vector2(position.x + size, position.y);
					Color color = new Color(1f, 1f, 1f, 0.1f);
					Debug.DrawLine(vector, vector2, color);
					Debug.DrawLine(vector2, vector3, color);
					Debug.DrawLine(vector3, vector4, color);
					Debug.DrawLine(vector4, vector, color);
				}
				if (drawPoint && !isNull)
				{
					Utility.DebugDrawPoint(pointPos, 0.1f, Color.red);
				}
			}
		}

		private Cell[] cells;

		private Vector2 size;

		private Vector2 halfSize;

		private float cellSize;

		private int xCellsCount;

		private int yCellsCount;

		public Vector2[] result;

		private List<int> spiral;

		private int GetCellIdx(int x, int y)
		{
			int num = x + y * xCellsCount;
			if (num >= cells.Length)
			{
				num = -1;
			}
			return num;
		}

		public RectDistribution(Vector2 size, int itemsCount, int relaxPasses)
		{
			this.size = size;
			float f = size.x * size.y / (float)itemsCount;
			cellSize = Mathf.Sqrt(f);
			float num = cellSize / 2f;
			xCellsCount = Mathf.CeilToInt(size.x / cellSize);
			yCellsCount = Mathf.CeilToInt(size.y / cellSize);
			float num2 = (float)(-xCellsCount) * cellSize * 0.5f;
			float num3 = (float)(-yCellsCount) * cellSize * 0.5f;
			cells = new Cell[xCellsCount * yCellsCount];
			for (int i = 0; i < yCellsCount; i++)
			{
				for (int j = 0; j < xCellsCount; j++)
				{
					int cellIdx = GetCellIdx(j, i);
					cells[cellIdx].size = cellSize;
					cells[cellIdx].position = new Vector2(num2 + (float)j * cellSize, num3 + (float)i * cellSize);
					cells[cellIdx].center = cells[cellIdx].position + new Vector2(cellSize, cellSize) / 2f;
					Vector2 pointPos = cells[cellIdx].center + Random.insideUnitCircle * num;
					cells[cellIdx].pointPos = pointPos;
					int[] adjacents = new int[8]
					{
						GetCellIdx(j - 1, i - 1),
						GetCellIdx(j - 1, i),
						GetCellIdx(j - 1, i + 1),
						GetCellIdx(j, i + 1),
						GetCellIdx(j + 1, i + 1),
						GetCellIdx(j + 1, i),
						GetCellIdx(j + 1, i - 1),
						GetCellIdx(j, i - 1)
					};
					cells[cellIdx].adjacents = adjacents;
				}
			}
			spiral = SpiralIndices();
			int num4 = cells.Length - itemsCount;
			float num5 = (float)(xCellsCount * 2 + yCellsCount * 2 - 4) / (float)num4;
			float num6 = 0f;
			for (int k = 0; k < num4; k++)
			{
				int num7 = spiral[(int)num6];
				cells[num7].isNull = true;
				num6 += num5;
			}
			for (int l = 0; l < relaxPasses; l++)
			{
				Relax(1f / (float)relaxPasses);
			}
			result = new Vector2[itemsCount];
			int num8 = 0;
			for (int num9 = spiral.Count - 1; num9 >= 0; num9--)
			{
				int num10 = spiral[num9];
				if (!cells[num10].isNull)
				{
					result[num8] = cells[num10].pointPos;
					num8++;
					if (num8 >= result.Length)
					{
						break;
					}
				}
			}
		}

		private void Relax(float power)
		{
			for (int i = 0; i < spiral.Count; i++)
			{
				if (cells[i].isNull)
				{
					continue;
				}
				Cell cell = cells[i];
				for (int j = 0; j < cell.adjacents.Length; j++)
				{
					if (cell.adjacents[j] < 0)
					{
						continue;
					}
					Cell cell2 = cells[cell.adjacents[j]];
					if (!cell2.isNull)
					{
						Vector2 vector = cell2.pointPos - cell.pointPos;
						float magnitude = vector.magnitude;
						vector /= magnitude;
						if (magnitude < cellSize)
						{
							float num = (cellSize - magnitude) / 2f * power;
							cell2.pointPos += vector * num;
							cell.pointPos -= vector * num;
							cells[cell.adjacents[j]] = cell2;
						}
					}
				}
				cells[i] = cell;
			}
		}

		public void DrawDebug(bool drawGrid)
		{
			if (drawGrid)
			{
				halfSize = size / 2f;
				Vector2 vector = new Vector2(0f - halfSize.x, 0f - halfSize.y);
				Vector2 vector2 = new Vector2(0f - halfSize.x, halfSize.y);
				Vector2 vector3 = new Vector2(halfSize.x, halfSize.y);
				Vector2 vector4 = new Vector2(halfSize.x, 0f - halfSize.y);
				Debug.DrawLine(vector, vector2, Color.yellow);
				Debug.DrawLine(vector2, vector3, Color.yellow);
				Debug.DrawLine(vector3, vector4, Color.yellow);
				Debug.DrawLine(vector4, vector, Color.yellow);
			}
			for (int i = 0; i < cells.Length; i++)
			{
				cells[i].Draw(drawGrid, drawPoint: true);
			}
		}

		private List<int> SpiralIndices()
		{
			List<int> list = new List<int>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 1;
			int num5 = 1;
			int num6 = xCellsCount - 1;
			int num7 = yCellsCount - 1;
			int num8 = 0;
			for (int i = 0; i < cells.Length; i++)
			{
				int cellIdx = GetCellIdx(num, num2);
				list.Add(cellIdx);
				num += num3;
				num2 += num4;
				if (num4 == 1 && num2 == num7)
				{
					num3 = 1;
					num4 = 0;
					num7--;
				}
				if (num3 == 1 && num == num6)
				{
					num3 = 0;
					num4 = -1;
					num6--;
				}
				if (num4 == -1 && num2 == num8)
				{
					num3 = -1;
					num4 = 0;
					num8++;
				}
				if (num3 == -1 && num == num5)
				{
					num3 = 0;
					num4 = 1;
					num5++;
				}
			}
			return list;
		}
	}
}

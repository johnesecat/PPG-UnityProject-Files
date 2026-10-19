using System.Collections.Generic;
using UnityEngine;

namespace Linefy
{
	public class Distribution2d
	{
		private class Cell
		{
			public int x;

			public int y;

			public float xFrom;

			public float xTo;

			public float yFrom;

			public float yTo;

			public List<Sample> samples = new List<Sample>();

			public Cell(int x, int y, float xFrom, float xTo, float yFrom, float yTo)
			{
				this.x = x;
				this.y = y;
				this.xFrom = xFrom;
				this.xTo = xTo;
				this.yFrom = yFrom;
				this.yTo = yTo;
			}

			public override string ToString()
			{
				return $"x:{xFrom},{xTo} y:{yFrom},{yTo}";
			}

			public Vector2 GetRandomPos()
			{
				float num = Mathf.Lerp(xFrom, xTo, Random.value);
				float num2 = Mathf.Lerp(yFrom, yTo, Random.value);
				return new Vector2(num, num2);
			}
		}

		private class Sample
		{
			public int index;

			public Cell cell;

			public Vector2 position;

			public float distance;

			public Sample(int index)
			{
				this.index = index;
			}
		}

		private int _samplesCount;

		private Vector2 _size;

		private int cellsXCount;

		private int cellsYCount;

		private Vector2 cellSizeStep;

		private float area;

		private Sample[] samples;

		private int _quality;

		private Cell[] cells;

		private Cell[] shuffledCells;

		private float cellSize;

		private Cell[] adjacentsCells = new Cell[9];

		private List<Sample> adjacentSamples = new List<Sample>();

		private Lines cellsWireframe;

		private Dots samplesDots;

		public int samplesCount => _samplesCount;

		public Vector2 size => _size;

		public int quality => _quality;

		private Cell this[int x, int y]
		{
			get
			{
				if (x < 0 || x >= cellsXCount)
				{
					return null;
				}
				if (y < 0 || y >= cellsYCount)
				{
					return null;
				}
				int num = x + y * cellsXCount;
				if (num >= cells.Length || num < 0)
				{
					Debug.LogErrorFormat("out of range idx:{0} x:{1} y:{2} cells.Length:{3}", num, x, y, cells.Length);
					return null;
				}
				return cells[num];
			}
			set
			{
				if (x >= 0 && x < cellsXCount && y >= 0 && y < cellsYCount)
				{
					int num = x + y * cellsXCount;
					if (num >= cells.Length || num < 0)
					{
						Debug.LogErrorFormat("out of range idx:{0} x:{1} y:{2} cells.Length:{3}", num, x, y, cells.Length);
					}
					else
					{
						cells[num] = value;
					}
				}
			}
		}

		public Vector2 this[int sampleIndex]
		{
			get
			{
				if (sampleIndex < 0 || sampleIndex >= _samplesCount)
				{
					Debug.LogWarningFormat("out of range sampleIndex {0} samples count:{1}", sampleIndex, _samplesCount);
				}
				return samples[sampleIndex].position;
			}
		}

		public Distribution2d(int samplesCount, Vector2 size, int quality)
		{
			_quality = quality;
			_samplesCount = Mathf.Max(samplesCount, 1);
			_size = new Vector2(Mathf.Max(size.x, 0f), Mathf.Max(size.y, 0f));
			area = _size.x * _size.y;
			float f = area / (float)samplesCount;
			cellSize = Mathf.Sqrt(f);
			cellsXCount = Mathf.CeilToInt(_size.x / cellSize);
			cellsYCount = Mathf.CeilToInt(_size.y / cellSize);
			cellSizeStep = new Vector2(_size.x / (float)cellsXCount, _size.y / (float)cellsYCount);
			cells = new Cell[cellsXCount * cellsYCount];
			shuffledCells = new Cell[cells.Length];
			for (int i = 0; i < cellsYCount; i++)
			{
				for (int j = 0; j < cellsXCount; j++)
				{
					float num = (float)j * cellSizeStep.x;
					float xTo = num + cellSizeStep.x;
					float num2 = (float)i * cellSizeStep.y;
					float yTo = num2 + cellSizeStep.y;
					this[j, i] = new Cell(j, i, num, xTo, num2, yTo);
				}
			}
			cells.CopyTo(shuffledCells, 0);
			for (int k = 0; k < shuffledCells.Length / 2; k++)
			{
				int num3 = Random.Range(0, shuffledCells.Length);
				int num4 = Random.Range(0, shuffledCells.Length);
				Cell cell = shuffledCells[num3];
				Cell cell2 = shuffledCells[num4];
				shuffledCells[num3] = cell2;
				shuffledCells[num4] = cell;
			}
			samples = new Sample[samplesCount];
			for (int l = 0; l < samples.Length; l++)
			{
				samples[l] = new Sample(l);
				samples[l].position = shuffledCells[l].GetRandomPos();
			}
			FillSamplesAndCells();
			for (int m = 0; m < _quality; m++)
			{
				Relax();
			}
		}

		private void FillAdjacentCell(Cell cell)
		{
			int x = cell.x;
			int y = cell.y;
			adjacentsCells[0] = this[x - 1, y - 1];
			adjacentsCells[1] = this[x - 1, y];
			adjacentsCells[2] = this[x - 1, y + 1];
			adjacentsCells[3] = this[x, y + 1];
			adjacentsCells[4] = this[x + 1, y + 1];
			adjacentsCells[5] = this[x + 1, y];
			adjacentsCells[6] = this[x + 1, y - 1];
			adjacentsCells[7] = this[x, y - 1];
			adjacentsCells[8] = cell;
		}

		private void Relax()
		{
			for (int i = 0; i < samples.Length; i++)
			{
				Sample sample = samples[i];
				FillAdjacentCell(sample.cell);
				Cell[] array = adjacentsCells;
				foreach (Cell cell in array)
				{
					if (cell == null)
					{
						continue;
					}
					foreach (Sample sample2 in cell.samples)
					{
						SpreadTwoSample(sample, sample2, cellSize);
					}
				}
			}
			FillSamplesAndCells();
		}

		private void SpreadTwoSample(Sample a, Sample b, float minDist)
		{
			float num = Vector2.Distance(a.position, b.position);
			num = (b.position - a.position).magnitude;
			if (num < minDist)
			{
				Vector2 normalized = (b.position - a.position).normalized;
				float num2 = (minDist - num) / 2f;
				a.position -= normalized * num2;
				b.position += normalized * num2;
			}
		}

		private void FillSamplesAndCells()
		{
			for (int i = 0; i < cells.Length; i++)
			{
				cells[i].samples.Clear();
			}
			for (int j = 0; j < samples.Length; j++)
			{
				Vector2 position = samples[j].position;
				int value = Mathf.FloorToInt(position.x / cellSizeStep.x);
				value = Mathf.Clamp(value, 0, cellsXCount - 1);
				int value2 = Mathf.FloorToInt(position.y / cellSizeStep.y);
				value2 = Mathf.Clamp(value2, 0, cellsYCount - 1);
				Cell cell = this[value, value2];
				if (cell == null)
				{
					Debug.LogFormat("cell for sample {0} == null");
				}
				Sample sample = samples[j];
				cell.samples.Add(sample);
				sample.cell = cell;
			}
		}

		public void GetAdjacentSamples(List<int> result, int sampleIndex)
		{
			result.Clear();
			adjacentSamples.Clear();
			Sample sample = samples[sampleIndex];
			FillAdjacentCell(sample.cell);
			Cell[] array = adjacentsCells;
			foreach (Cell cell in array)
			{
				if (cell == null)
				{
					continue;
				}
				foreach (Sample sample2 in cell.samples)
				{
					if (sample2 != sample)
					{
						sample2.distance = Vector2.Distance(sample2.position, sample.position);
						adjacentSamples.Add(sample2);
					}
				}
			}
			adjacentSamples.Sort(distanceSorter);
			foreach (Sample adjacentSample in adjacentSamples)
			{
				result.Add(adjacentSample.index);
			}
		}

		private int distanceSorter(Sample a, Sample b)
		{
			return (int)Mathf.Sign(a.distance - b.distance);
		}

		public void DrawDebug(Matrix4x4 matrix, float wireTransparency, float dotSampleSize)
		{
			if (cellsWireframe == null)
			{
				cellsWireframe = new Lines(cellsXCount + 1 + cellsYCount + 1, transparent: true, 1f, 2f, Color.white);
				int num = 0;
				for (int i = 0; i <= cellsXCount; i++)
				{
					Vector2 vector = new Vector2((float)i * cellSizeStep.x, 0f);
					Vector2 vector2 = new Vector2(vector.x, _size.y);
					cellsWireframe.SetPosition(num, vector, vector2);
					num++;
				}
				for (int j = 0; j <= cellsYCount; j++)
				{
					Vector2 vector3 = new Vector2(0f, (float)j * cellSizeStep.y);
					Vector2 vector4 = new Vector2(_size.x, vector3.y);
					cellsWireframe.SetPosition(num, vector3, vector4);
					num++;
				}
			}
			if (samplesDots == null)
			{
				samplesDots = new Dots(samplesCount, transparent: true);
				for (int k = 0; k < samplesCount; k++)
				{
					samplesDots[k] = new Dot(samples[k].position, 8f, 0, Color.red);
				}
			}
			cellsWireframe.colorMultiplier = new Color(1f, 1f, 1f, wireTransparency);
			cellsWireframe.Draw(matrix);
			samplesDots.widthMultiplier = dotSampleSize;
			samplesDots.Draw(matrix);
		}
	}
}

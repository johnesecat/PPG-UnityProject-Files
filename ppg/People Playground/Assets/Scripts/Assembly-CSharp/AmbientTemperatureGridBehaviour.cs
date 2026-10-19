using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AmbientTemperatureGridBehaviour : MonoBehaviour
{
	public class Cell
	{
		public float ReadTemperature;

		public float WriteTemperature;

		public bool CanCreateNeighbours = true;

		public bool CanTransferTemperature = true;

		public Cell(float t)
		{
			WriteTemperature = (ReadTemperature = t);
		}

		public void Set(float t)
		{
			WriteTemperature = t;
		}

		public void ForceSet(float t)
		{
			ReadTemperature = (WriteTemperature = t);
		}

		public float Get()
		{
			return ReadTemperature;
		}

		public void Sync()
		{
			ReadTemperature = WriteTemperature;
		}
	}

	public static AmbientTemperatureGridBehaviour Instance;

	public ConcurrentDictionary<Vector2Int, Cell> World = new ConcurrentDictionary<Vector2Int, Cell>();

	public float ObjectToAirTransferRate = 0.002f;

	public float AirToObjectTransferRate = 0.002f;

	public float AmbientTemperatureTransferRate = 0.002f;

	public int GridSize = 1;

	public ConcurrentDictionary<Vector2Int, byte> BlockedCells = new ConcurrentDictionary<Vector2Int, byte>();

	private volatile bool diffuseIsFree = true;

	private void Awake()
	{
		Instance = this;
	}

	[Obsolete]
	public void ComputeBlockedCellPositions()
	{
		Debug.Log("Computing blocked ambient temperature cell positions...");
		BlockedCells.Clear();
		Bounds boundingBox = Global.main.CameraControlBehaviour.BoundingBox;
		Debug.DrawLine(boundingBox.min, boundingBox.max, Color.cyan, 15f);
		int num = Mathf.FloorToInt(boundingBox.min.x / (float)GridSize) * GridSize;
		int num2 = Mathf.CeilToInt(boundingBox.max.x / (float)GridSize) * GridSize;
		int num3 = Mathf.FloorToInt(boundingBox.min.y / (float)GridSize) * GridSize;
		int num4 = Mathf.CeilToInt(boundingBox.max.y / (float)GridSize) * GridSize;
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				Collider2D collider2D = Physics2D.OverlapPoint(GridToWorldPoint(i, j));
				if ((bool)collider2D && collider2D.gameObject.layer == 11)
				{
					BlockedCells.TryAdd(new Vector2Int(i, j), 0);
				}
			}
		}
	}

	private void Diffuse(object state)
	{
		diffuseIsFree = false;
		foreach (KeyValuePair<Vector2Int, Cell> item in World)
		{
			item.Value.Set(GetAverageNeighbouringTemperature(item.Value, item.Key));
		}
		foreach (KeyValuePair<Vector2Int, Cell> item2 in World)
		{
			item2.Value.Sync();
		}
		diffuseIsFree = true;
	}

	private void FixedUpdate()
	{
		if (UserPreferenceManager.Current.AmbientTemperatureTransfer && diffuseIsFree)
		{
			diffuseIsFree = false;
			ThreadPool.QueueUserWorkItem(Diffuse);
		}
	}

	private float GetAverageNeighbouringTemperature(Cell center, Vector2Int v)
	{
		float num = center.Get();
		float temperatureAt = GetTemperatureAt(v.x - 1, v.y);
		float temperatureAt2 = GetTemperatureAt(v.x + 1, v.y);
		float temperatureAt3 = GetTemperatureAt(v.x, v.y + 1);
		float temperatureAt4 = GetTemperatureAt(v.x, v.y - 1);
		if (center.CanCreateNeighbours && MapConfig.Instance.Bounds.BoundingBox.Contains(new Vector3(v.x, v.y, 0f)) && isDifferenceAboveThreshold(num, PhysicalBehaviour.AmbientTemperature))
		{
			Vector2Int vector2Int = new Vector2Int(v.x - 1, v.y);
			if (isDifferenceAboveThreshold(temperatureAt, num) && !World.ContainsKey(vector2Int))
			{
				createCell(vector2Int);
			}
			vector2Int = new Vector2Int(v.x + 1, v.y);
			if (isDifferenceAboveThreshold(temperatureAt2, num) && !World.ContainsKey(vector2Int))
			{
				createCell(vector2Int);
			}
			vector2Int = new Vector2Int(v.x, v.y + 1);
			if (isDifferenceAboveThreshold(temperatureAt3, num) && !World.ContainsKey(vector2Int))
			{
				createCell(vector2Int);
			}
			vector2Int = new Vector2Int(v.x, v.y - 1);
			if (isDifferenceAboveThreshold(temperatureAt4, num) && !World.ContainsKey(vector2Int))
			{
				createCell(vector2Int);
			}
		}
		return (temperatureAt + temperatureAt2 + temperatureAt3 + temperatureAt4 + num) / 5f;
		void createCell(Vector2Int pos)
		{
			center.CanCreateNeighbours = false;
			Cell value = new Cell(PhysicalBehaviour.AmbientTemperature);
			World.TryAdd(pos, value);
		}
		bool isDifferenceAboveThreshold(float a, float b)
		{
			return Mathf.Abs(Mathf.Clamp(a, -350f, 350f) - Mathf.Clamp(b, -350f, 350f)) > 300f;
		}
	}

	[Obsolete]
	private bool IsInsideWorldWall(Vector2Int gridPoint)
	{
		return BlockedCells.ContainsKey(gridPoint);
	}

	public float GetTemperatureAt(int x, int y)
	{
		if (World.TryGetValue(new Vector2Int(x, y), out var value) && value.CanTransferTemperature)
		{
			return value.Get();
		}
		return PhysicalBehaviour.AmbientTemperature;
	}

	public float GetTemperatureAtPoint(Vector2 worldPoint)
	{
		Vector2Int vector2Int = WorldToGridPoint(worldPoint.x, worldPoint.y);
		return GetTemperatureAt(vector2Int.x, vector2Int.y);
	}

	public void ChangeTemperatureAt(Vector2 worldPoint, float celsiusChange)
	{
		Vector2Int key = WorldToGridPoint(worldPoint.x, worldPoint.y);
		if (World.TryGetValue(key, out var value) && value.CanTransferTemperature)
		{
			float num = value.Get();
			value.Set(num + celsiusChange);
		}
	}

	public void SetTemperatureAt(Vector2 worldPoint, float celsius)
	{
		Vector2Int key = WorldToGridPoint(worldPoint.x, worldPoint.y);
		if (World.TryGetValue(key, out var value) && value.CanTransferTemperature)
		{
			value.Set(celsius);
		}
	}

	public void ChangeTemperatureAt(int x, int y, float celsiusChange)
	{
		if (World.TryGetValue(new Vector2Int(x, y), out var value) && value.CanTransferTemperature)
		{
			float num = value.Get();
			value.ForceSet(num + celsiusChange);
		}
	}

	public void SetTemperatureAt(int x, int y, float celsius)
	{
		if (World.TryGetValue(new Vector2Int(x, y), out var value) && value.CanTransferTemperature)
		{
			value.Set(celsius);
		}
	}

	public void EnsureExistence(int x, int y)
	{
		Vector2Int key = new Vector2Int(x, y);
		if (!World.ContainsKey(key))
		{
			World.TryAdd(key, new Cell(PhysicalBehaviour.AmbientTemperature));
		}
	}

	public void TransferHeat(PhysicalBehaviour phys)
	{
		if (!phys.SimulateTemperature || !phys.spriteRenderer)
		{
			return;
		}
		Bounds bounds = phys.spriteRenderer.bounds;
		Vector3 center = bounds.center;
		Vector3 size = bounds.size;
		float num = Mathf.Min(size.x, 30f);
		float num2 = Mathf.Min(size.y, 30f);
		int num3 = Mathf.CeilToInt(num);
		int num4 = Mathf.CeilToInt(num2);
		int num5 = num3 * num4;
		float multiplier = 1f / (float)num5;
		float num6 = num / 2f;
		float num7 = num2 / 2f;
		for (int i = 0; i < num3; i++)
		{
			for (int j = 0; j < num4; j++)
			{
				float x = center.x + (float)i - num6;
				float y = center.y + (float)j - num7;
				Vector2Int key = WorldToGridPoint(x, y);
				if (!World.TryGetValue(key, out var value))
				{
					value = new Cell(PhysicalBehaviour.AmbientTemperature);
					World.TryAdd(key, value);
				}
				Transfer(phys, value, multiplier);
			}
		}
	}

	public Vector2Int WorldToGridPoint(float x, float y)
	{
		return new Vector2Int(Mathf.RoundToInt(x / (float)GridSize), Mathf.RoundToInt(y / (float)GridSize));
	}

	public Vector2 GridToWorldPoint(int x, int y)
	{
		return new Vector2(x * GridSize, y * GridSize);
	}

	private void Transfer(PhysicalBehaviour phys, Cell cell, float multiplier = 1f)
	{
		if (phys.SimulateTemperature)
		{
			float temperature = phys.Temperature;
			float num = cell.Get();
			float heatCapacity = phys.GetHeatCapacity();
			phys.Temperature = Mathf.Lerp(temperature, num, AirToObjectTransferRate / heatCapacity * multiplier);
			cell.ForceSet(Mathf.Lerp(num, temperature, ObjectToAirTransferRate * heatCapacity * multiplier));
		}
	}
}

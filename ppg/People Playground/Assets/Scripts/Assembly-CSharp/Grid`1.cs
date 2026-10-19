using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Grid<T> : IEnumerable<(int X, int Y, T Value)>, IEnumerable
{
	public readonly int Width;

	public readonly int Height;

	public T[] Flat;

	public Grid(int width, int height)
	{
		Width = width;
		Height = height;
		Flat = new T[width * height];
	}

	public Grid(int width, int height, T[] flat)
	{
		if (flat.Length != width * height)
		{
			throw new ArgumentException("Given array is not of size width * height");
		}
		Width = width;
		Height = height;
		Flat = flat;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T Get(int x, int y)
	{
		return Flat[GetIndexFrom(x, y)];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGet(int x, int y, out T cell)
	{
		cell = default(T);
		if (x < 0 || x >= Width)
		{
			return false;
		}
		if (y < 0 || y >= Height)
		{
			return false;
		}
		cell = Flat[GetIndexFrom(x, y)];
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set(int x, int y, T value)
	{
		Flat[GetIndexFrom(x, y)] = value;
	}

	public IEnumerator<(int X, int Y, T Value)> GetEnumerator()
	{
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				yield return (x, y, Get(x, y));
			}
		}
	}

	public bool BlockEquals(Grid<T> block, int top = 0, int left = 0)
	{
		foreach (var item4 in block)
		{
			int item = item4.X;
			int item2 = item4.Y;
			T item3 = item4.Value;
			int num = item + left;
			int num2 = item2 + top;
			if (num < 0 || num >= Width)
			{
				return false;
			}
			if (num2 < 0 || num2 >= Height)
			{
				return false;
			}
			if (!Get(num, num2).Equals(item3))
			{
				return false;
			}
		}
		return true;
	}

	public void WriteBlock(Grid<T> block, int top, int left)
	{
		foreach (var item4 in block)
		{
			int item = item4.X;
			int item2 = item4.Y;
			T item3 = item4.Value;
			int x = item + left;
			int y = item2 + top;
			Set(x, y, item3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetIndexFrom(int x, int y)
	{
		if (x < 0 || x >= Width)
		{
			throw new ArgumentOutOfRangeException($"X ({x}) is out of range (0, {Width - 1})");
		}
		if (y < 0 || y >= Height)
		{
			throw new ArgumentOutOfRangeException($"Y ({y}) is out of range (0, {Height - 1})");
		}
		return IndexFromCoordinates(x, y, Width);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexFromCoordinates(int x, int y, int w)
	{
		return w * y + x;
	}

	public static T GetAt(T[] flat, int x, int y, int w)
	{
		return flat[IndexFromCoordinates(x, y, w)];
	}

	public static void SetAt(T[] flat, int x, int y, int w, T val)
	{
		flat[IndexFromCoordinates(x, y, w)] = val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GetCoordinatesFromIndex(int index, out int x, out int y)
	{
		if (index >= Width * Height)
		{
			throw new ArgumentOutOfRangeException($"Index ({index}) is out of range ({Width * Height})");
		}
		GetCoordinatesFromIndex(index, Width, out x, out y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GetCoordinatesFromIndex(int index, int w, out int x, out int y)
	{
		x = index % w;
		y = index / w;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

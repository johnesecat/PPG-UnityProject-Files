using System;

namespace Ceras.Formatters
{
	public sealed class MultiDimensionalArrayFormatter<TItem> : IFormatter<Array>, IFormatter, IFormatter<TItem[,]>
	{
		private readonly uint _maxCount;

		private readonly IFormatter<TItem> _itemFormatter;

		public MultiDimensionalArrayFormatter(CerasSerializer serializer, uint maxCount)
		{
			_maxCount = maxCount;
			Type typeFromHandle = typeof(TItem);
			_itemFormatter = (IFormatter<TItem>)serializer.GetReferenceFormatter(typeFromHandle);
		}

		private static void ReadLastDimension2D(byte[] buffer, ref int offset, IFormatter<TItem> formatter, TItem[,] array, int[] index, int max)
		{
			for (int i = 0; i < max; i++)
			{
				formatter.Deserialize(buffer, ref offset, ref array[index[0], i]);
			}
		}

		private static void ReadLastDimension3D(byte[] buffer, ref int offset, IFormatter<TItem> formatter, TItem[,,] array, int[] index, int max)
		{
			for (int i = 0; i < max; i++)
			{
				formatter.Deserialize(buffer, ref offset, ref array[index[0], index[1], i]);
			}
		}

		private static void ReadLastDimension4D(byte[] buffer, ref int offset, IFormatter<TItem> formatter, TItem[,,,] array, int[] index, int max)
		{
			for (int i = 0; i < max; i++)
			{
				formatter.Deserialize(buffer, ref offset, ref array[index[0], index[1], index[2], i]);
			}
		}

		private static void ReadLastDimension5D(byte[] buffer, ref int offset, IFormatter<TItem> formatter, TItem[,,,,] array, int[] index, int max)
		{
			for (int i = 0; i < max; i++)
			{
				formatter.Deserialize(buffer, ref offset, ref array[index[0], index[1], index[2], index[3], i]);
			}
		}

		private static void ReadLastDimension6D(byte[] buffer, ref int offset, IFormatter<TItem> formatter, TItem[,,,,,] array, int[] index, int max)
		{
			for (int i = 0; i < max; i++)
			{
				formatter.Deserialize(buffer, ref offset, ref array[index[0], index[1], index[2], index[2], index[4], i]);
			}
		}

		public void Serialize(ref byte[] buffer, ref int offset, Array baseAr)
		{
			int rank = baseAr.Rank;
			SerializerBinary.WriteUInt32(ref buffer, ref offset, (uint)rank);
			for (int i = 0; i < rank; i++)
			{
				int length = baseAr.GetLength(i);
				SerializerBinary.WriteUInt32(ref buffer, ref offset, (uint)length);
			}
			IFormatter<TItem> itemFormatter = _itemFormatter;
			foreach (object item in baseAr)
			{
				itemFormatter.Serialize(ref buffer, ref offset, (TItem)item);
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Array baseAr)
		{
			int num = (int)SerializerBinary.ReadUInt32(buffer, ref offset);
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = (array[i] = (int)SerializerBinary.ReadUInt32(buffer, ref offset));
			}
			int num3 = array[0];
			for (int j = 1; j < num; j++)
			{
				num3 *= array[j];
			}
			if (num3 > _maxCount)
			{
				throw new InvalidOperationException($"The data describes an array with '{num3}' elements, which exceeds the allowed limit of '{_maxCount}'");
			}
			baseAr = Array.CreateInstance(typeof(TItem), array);
			int[] index = new int[num];
			ReadArrayEntry(buffer, ref offset, _itemFormatter, baseAr, index, array, 0);
		}

		private static void ReadArrayEntry(byte[] buffer, ref int offset, IFormatter<TItem> formatter, Array array, int[] index, int[] dimensionSizes, int depth)
		{
			int num = dimensionSizes[depth];
			if (depth == dimensionSizes.Length - 1)
			{
				switch (dimensionSizes.Length)
				{
				case 2:
					ReadLastDimension2D(buffer, ref offset, formatter, (TItem[,])array, index, num);
					break;
				case 3:
					ReadLastDimension3D(buffer, ref offset, formatter, (TItem[,,])array, index, num);
					break;
				case 4:
					ReadLastDimension4D(buffer, ref offset, formatter, (TItem[,,,])array, index, num);
					break;
				case 5:
					ReadLastDimension5D(buffer, ref offset, formatter, (TItem[,,,,])array, index, num);
					break;
				case 6:
					ReadLastDimension6D(buffer, ref offset, formatter, (TItem[,,,,,])array, index, num);
					break;
				default:
					throw new IndexOutOfRangeException("Array rank must be between 2 and 6");
				}
			}
			else
			{
				index[depth] = 0;
				for (int i = 0; i < num; i++)
				{
					ReadArrayEntry(buffer, ref offset, formatter, array, index, dimensionSizes, depth + 1);
					index[depth]++;
				}
			}
		}

		public void Serialize(ref byte[] buffer, ref int offset, TItem[,] ar)
		{
			Serialize(ref buffer, ref offset, (Array)ar);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref TItem[,] ar)
		{
			Array baseAr = ar;
			Deserialize(buffer, ref offset, ref baseAr);
			ar = (TItem[,])baseAr;
		}

		public void Serialize(ref byte[] buffer, ref int offset, TItem[,,] ar)
		{
			Serialize(ref buffer, ref offset, (Array)ar);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref TItem[,,] ar)
		{
			Array baseAr = ar;
			Deserialize(buffer, ref offset, ref baseAr);
			ar = (TItem[,,])baseAr;
		}

		public void Serialize(ref byte[] buffer, ref int offset, TItem[,,,] ar)
		{
			Serialize(ref buffer, ref offset, (Array)ar);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref TItem[,,,] ar)
		{
			Array baseAr = ar;
			Deserialize(buffer, ref offset, ref baseAr);
			ar = (TItem[,,,])baseAr;
		}

		public void Serialize(ref byte[] buffer, ref int offset, TItem[,,,,] ar)
		{
			Serialize(ref buffer, ref offset, (Array)ar);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref TItem[,,,,] ar)
		{
			Array baseAr = ar;
			Deserialize(buffer, ref offset, ref baseAr);
			ar = (TItem[,,,,])baseAr;
		}

		public void Serialize(ref byte[] buffer, ref int offset, TItem[,,,,,] ar)
		{
			Serialize(ref buffer, ref offset, (Array)ar);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref TItem[,,,,,] ar)
		{
			Array baseAr = ar;
			Deserialize(buffer, ref offset, ref baseAr);
			ar = (TItem[,,,,,])baseAr;
		}
	}
}

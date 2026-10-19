using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NAudio.Utils;

namespace NAudio.Wave
{
	[DefaultMember("Item")]
	public class CueList
	{
		private readonly List<Cue> cues = new List<Cue>();

		public int[] CuePositions
		{
			get
			{
				int[] array = new int[cues.get_Count()];
				for (int i = 0; i < cues.get_Count(); i++)
				{
					array[i] = cues.get_Item(i).Position;
				}
				return array;
			}
		}

		public string[] CueLabels
		{
			get
			{
				string[] array = new string[cues.get_Count()];
				for (int i = 0; i < cues.get_Count(); i++)
				{
					array[i] = cues.get_Item(i).Label;
				}
				return array;
			}
		}

		public int Count => cues.get_Count();

		public Cue this[int index] => cues.get_Item(index);

		public CueList()
		{
		}

		public void Add(Cue cue)
		{
			cues.Add(cue);
		}

		internal CueList(byte[] cueChunkData, byte[] listChunkData)
		{
			int num = BitConverter.ToInt32(cueChunkData, 0);
			Dictionary<int, int> val = new Dictionary<int, int>();
			int[] array = new int[num];
			int num2 = 0;
			int num3 = 4;
			while (cueChunkData.Length - num3 >= 24)
			{
				val.set_Item(BitConverter.ToInt32(cueChunkData, num3), num2);
				array[num2] = BitConverter.ToInt32(cueChunkData, num3 + 20);
				num3 += 24;
				num2++;
			}
			string[] array2 = new string[num];
			int num4 = 0;
			int num5 = ChunkIdentifier.ChunkIdentifierToInt32("labl");
			for (int i = 4; listChunkData.Length - i >= 16; i += num4 + num4 % 2 + 12)
			{
				if (BitConverter.ToInt32(listChunkData, i) == num5)
				{
					num4 = BitConverter.ToInt32(listChunkData, i + 4) - 4;
					int num6 = BitConverter.ToInt32(listChunkData, i + 8);
					num2 = val.get_Item(num6);
					array2[num2] = Encoding.get_Default().GetString(listChunkData, i + 12, num4 - 1);
				}
			}
			for (int j = 0; j < num; j++)
			{
				cues.Add(new Cue(array[j], array2[j]));
			}
		}

		internal byte[] GetRiffChunks()
		{
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Expected O, but got Unknown
			if (Count == 0)
			{
				return null;
			}
			int num = 12 + 24 * Count;
			int num2 = 12;
			for (int i = 0; i < Count; i++)
			{
				int num3 = this[i].Label.get_Length() + 1;
				num2 += num3 + num3 % 2 + 12;
			}
			byte[] array = new byte[num + num2];
			int num4 = ChunkIdentifier.ChunkIdentifierToInt32("cue ");
			int num5 = ChunkIdentifier.ChunkIdentifierToInt32("data");
			int num6 = ChunkIdentifier.ChunkIdentifierToInt32("LIST");
			int num7 = ChunkIdentifier.ChunkIdentifierToInt32("adtl");
			int num8 = ChunkIdentifier.ChunkIdentifierToInt32("labl");
			MemoryStream val = new MemoryStream(array);
			try
			{
				BinaryWriter val2 = new BinaryWriter((Stream)(object)val);
				try
				{
					val2.Write(num4);
					val2.Write(num - 8);
					val2.Write(Count);
					for (int j = 0; j < Count; j++)
					{
						int position = this[j].Position;
						val2.Write(j);
						val2.Write(position);
						val2.Write(num5);
						val2.Seek(8, (SeekOrigin)1);
						val2.Write(position);
					}
					val2.Write(num6);
					val2.Write(num2 - 8);
					val2.Write(num7);
					for (int k = 0; k < Count; k++)
					{
						val2.Write(num8);
						val2.Write(this[k].Label.get_Length() + 1 + 4);
						val2.Write(k);
						val2.Write(Encoding.get_Default().GetBytes(this[k].Label.ToCharArray()));
						if (this[k].Label.get_Length() % 2 == 0)
						{
							val2.Seek(2, (SeekOrigin)1);
						}
						else
						{
							val2.Seek(1, (SeekOrigin)1);
						}
					}
					return array;
				}
				finally
				{
					((global::System.IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((global::System.IDisposable)val)?.Dispose();
			}
		}

		internal static CueList FromChunks(WaveFileReader reader)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			CueList result = null;
			byte[] array = null;
			byte[] array2 = null;
			Enumerator<RiffChunk> enumerator = reader.ExtraChunks.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RiffChunk current = enumerator.get_Current();
					if (current.IdentifierAsString.ToLower() == "cue ")
					{
						array = reader.GetChunkData(current);
					}
					else if (current.IdentifierAsString.ToLower() == "list")
					{
						array2 = reader.GetChunkData(current);
					}
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator).Dispose();
			}
			if (array != null && array2 != null)
			{
				result = new CueList(array, array2);
			}
			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Utils;

namespace NAudio.Wave
{
	public class AiffFileReader : WaveStream
	{
		public struct AiffChunk
		{
			public string ChunkName;

			public uint ChunkLength;

			public uint ChunkStart;

			public AiffChunk(uint start, string name, uint length)
			{
				ChunkStart = start;
				ChunkName = name;
				ChunkLength = length + (uint)((length % 2u == 1) ? 1 : 0);
			}
		}

		private readonly WaveFormat waveFormat;

		private readonly bool ownInput;

		private readonly long dataPosition;

		private readonly int dataChunkLength;

		private readonly List<AiffChunk> chunks = new List<AiffChunk>();

		private Stream waveStream;

		private readonly object lockObject = new object();

		public override WaveFormat WaveFormat => waveFormat;

		public override long Length => dataChunkLength;

		public long SampleCount
		{
			get
			{
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				if (waveFormat.Encoding == WaveFormatEncoding.Pcm || waveFormat.Encoding == WaveFormatEncoding.Extensible || waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
				{
					return dataChunkLength / BlockAlign;
				}
				throw new FormatException("Sample count is calculated only for the standard encodings");
			}
		}

		public override long Position
		{
			get
			{
				return waveStream.get_Position() - dataPosition;
			}
			set
			{
				lock (lockObject)
				{
					value = Math.Min(value, ((Stream)this).get_Length());
					value -= value % waveFormat.BlockAlign;
					waveStream.set_Position(value + dataPosition);
				}
			}
		}

		public AiffFileReader(string aiffFile)
			: this((Stream)(object)File.OpenRead(aiffFile))
		{
			ownInput = true;
		}

		public AiffFileReader(Stream inputStream)
		{
			waveStream = inputStream;
			ReadAiffHeader(waveStream, out waveFormat, out dataPosition, out dataChunkLength, chunks);
			((Stream)this).set_Position(0L);
		}

		public static void ReadAiffHeader(Stream stream, out WaveFormat format, out long dataChunkPosition, out int dataChunkLength, List<AiffChunk> chunks)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			dataChunkPosition = -1L;
			format = null;
			BinaryReader val = new BinaryReader(stream);
			if (ReadChunkName(val) != "FORM")
			{
				throw new FormatException("Not an AIFF file - no FORM header.");
			}
			ConvertInt(val.ReadBytes(4));
			string text = ReadChunkName(val);
			if (text != "AIFC" && text != "AIFF")
			{
				throw new FormatException("Not an AIFF file - no AIFF/AIFC header.");
			}
			dataChunkLength = 0;
			while (val.get_BaseStream().get_Position() < val.get_BaseStream().get_Length())
			{
				AiffChunk aiffChunk = ReadChunkHeader(val);
				if (aiffChunk.ChunkName == "\0\0\0\0" || val.get_BaseStream().get_Position() + aiffChunk.ChunkLength > val.get_BaseStream().get_Length())
				{
					break;
				}
				if (aiffChunk.ChunkName == "COMM")
				{
					short channels = ConvertShort(val.ReadBytes(2));
					ConvertInt(val.ReadBytes(4));
					short bits = ConvertShort(val.ReadBytes(2));
					double num = IEEE.ConvertFromIeeeExtended(val.ReadBytes(10));
					format = new WaveFormat((int)num, bits, channels);
					if (aiffChunk.ChunkLength > 18 && text == "AIFC")
					{
						if (new string(val.ReadChars(4)).ToLower() != "none")
						{
							throw new FormatException("Compressed AIFC is not supported.");
						}
						val.ReadBytes((int)(aiffChunk.ChunkLength - 22));
					}
					else
					{
						val.ReadBytes((int)(aiffChunk.ChunkLength - 18));
					}
				}
				else if (aiffChunk.ChunkName == "SSND")
				{
					uint num2 = ConvertInt(val.ReadBytes(4));
					ConvertInt(val.ReadBytes(4));
					dataChunkPosition = aiffChunk.ChunkStart + 16 + num2;
					dataChunkLength = (int)(aiffChunk.ChunkLength - 8);
					Stream baseStream = val.get_BaseStream();
					baseStream.set_Position(baseStream.get_Position() + (aiffChunk.ChunkLength - 8));
				}
				else
				{
					chunks?.Add(aiffChunk);
					Stream baseStream2 = val.get_BaseStream();
					baseStream2.set_Position(baseStream2.get_Position() + aiffChunk.ChunkLength);
				}
			}
			if (format == null)
			{
				throw new FormatException("Invalid AIFF file - No COMM chunk found.");
			}
			if (dataChunkPosition == -1)
			{
				throw new FormatException("Invalid AIFF file - No SSND chunk found.");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && waveStream != null)
			{
				if (ownInput)
				{
					waveStream.Close();
				}
				waveStream = null;
			}
			((Stream)this).Dispose(disposing);
		}

		public override int Read(byte[] array, int offset, int count)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			if (count % waveFormat.BlockAlign != 0)
			{
				throw new ArgumentException($"Must read complete blocks: requested {count}, block align is {WaveFormat.BlockAlign}");
			}
			lock (lockObject)
			{
				if (((Stream)this).get_Position() + count > dataChunkLength)
				{
					count = dataChunkLength - (int)((Stream)this).get_Position();
				}
				byte[] array2 = new byte[count];
				int num = waveStream.Read(array2, offset, count);
				int num2 = WaveFormat.BitsPerSample / 8;
				for (int i = 0; i < num; i += num2)
				{
					if (WaveFormat.BitsPerSample == 8)
					{
						array[i] = array2[i];
						continue;
					}
					if (WaveFormat.BitsPerSample == 16)
					{
						array[i] = array2[i + 1];
						array[i + 1] = array2[i];
						continue;
					}
					if (WaveFormat.BitsPerSample == 24)
					{
						array[i] = array2[i + 2];
						array[i + 1] = array2[i + 1];
						array[i + 2] = array2[i];
						continue;
					}
					if (WaveFormat.BitsPerSample == 32)
					{
						array[i] = array2[i + 3];
						array[i + 1] = array2[i + 2];
						array[i + 2] = array2[i + 1];
						array[i + 3] = array2[i];
						continue;
					}
					throw new FormatException("Unsupported PCM format.");
				}
				return num;
			}
		}

		private static uint ConvertInt(byte[] buffer)
		{
			if (buffer.Length != 4)
			{
				throw new global::System.Exception("Incorrect length for long.");
			}
			return (uint)((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]);
		}

		private static short ConvertShort(byte[] buffer)
		{
			if (buffer.Length != 2)
			{
				throw new global::System.Exception("Incorrect length for int.");
			}
			return (short)((buffer[0] << 8) | buffer[1]);
		}

		private static AiffChunk ReadChunkHeader(BinaryReader br)
		{
			return new AiffChunk((uint)br.get_BaseStream().get_Position(), ReadChunkName(br), ConvertInt(br.ReadBytes(4)));
		}

		private static string ReadChunkName(BinaryReader br)
		{
			return new string(br.ReadChars(4));
		}
	}
}

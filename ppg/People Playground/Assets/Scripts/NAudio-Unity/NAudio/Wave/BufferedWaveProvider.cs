using System;
using System.Runtime.CompilerServices;
using NAudio.Utils;

namespace NAudio.Wave
{
	public class BufferedWaveProvider : IWaveProvider
	{
		private CircularBuffer circularBuffer;

		private readonly WaveFormat waveFormat;

		public bool ReadFully
		{
			[CompilerGenerated]
			get
			{
				return <ReadFully>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<ReadFully>k__BackingField = value;
			}
		}

		public int BufferLength
		{
			[CompilerGenerated]
			get
			{
				return <BufferLength>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<BufferLength>k__BackingField = value;
			}
		}

		public TimeSpan BufferDuration
		{
			get
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				return TimeSpan.FromSeconds((double)BufferLength / (double)WaveFormat.AverageBytesPerSecond);
			}
			set
			{
				BufferLength = (int)(((TimeSpan)(ref value)).get_TotalSeconds() * (double)WaveFormat.AverageBytesPerSecond);
			}
		}

		public bool DiscardOnBufferOverflow
		{
			[CompilerGenerated]
			get
			{
				return <DiscardOnBufferOverflow>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<DiscardOnBufferOverflow>k__BackingField = value;
			}
		}

		public int BufferedBytes
		{
			get
			{
				if (circularBuffer != null)
				{
					return circularBuffer.Count;
				}
				return 0;
			}
		}

		public TimeSpan BufferedDuration => TimeSpan.FromSeconds((double)BufferedBytes / (double)WaveFormat.AverageBytesPerSecond);

		public WaveFormat WaveFormat => waveFormat;

		public BufferedWaveProvider(WaveFormat waveFormat)
		{
			this.waveFormat = waveFormat;
			BufferLength = waveFormat.AverageBytesPerSecond * 5;
			ReadFully = true;
		}

		public void AddSamples(byte[] buffer, int offset, int count)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (circularBuffer == null)
			{
				circularBuffer = new CircularBuffer(BufferLength);
			}
			if (circularBuffer.Write(buffer, offset, count) < count && !DiscardOnBufferOverflow)
			{
				throw new InvalidOperationException("Buffer full");
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			if (circularBuffer != null)
			{
				num = circularBuffer.Read(buffer, offset, count);
			}
			if (ReadFully && num < count)
			{
				global::System.Array.Clear((global::System.Array)buffer, offset + num, count - num);
				num = count;
			}
			return num;
		}

		public void ClearBuffer()
		{
			if (circularBuffer != null)
			{
				circularBuffer.Reset();
			}
		}
	}
}

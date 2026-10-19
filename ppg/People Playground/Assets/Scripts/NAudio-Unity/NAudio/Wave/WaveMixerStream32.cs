using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace NAudio.Wave
{
	public class WaveMixerStream32 : WaveStream
	{
		private readonly List<WaveStream> inputStreams;

		private readonly object inputsLock;

		private WaveFormat waveFormat;

		private long length;

		private long position;

		private readonly int bytesPerSample;

		public int InputCount => inputStreams.get_Count();

		public bool AutoStop
		{
			[CompilerGenerated]
			get
			{
				return <AutoStop>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<AutoStop>k__BackingField = value;
			}
		}

		public override int BlockAlign => waveFormat.BlockAlign;

		public override long Length => length;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				lock (inputsLock)
				{
					value = Math.Min(value, ((Stream)this).get_Length());
					Enumerator<WaveStream> enumerator = inputStreams.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							WaveStream current = enumerator.get_Current();
							((Stream)current).set_Position(Math.Min(value, ((Stream)current).get_Length()));
						}
					}
					finally
					{
						((global::System.IDisposable)enumerator).Dispose();
					}
					position = value;
				}
			}
		}

		public override WaveFormat WaveFormat => waveFormat;

		public WaveMixerStream32()
		{
			AutoStop = true;
			waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
			bytesPerSample = 4;
			inputStreams = new List<WaveStream>();
			inputsLock = new object();
		}

		public WaveMixerStream32(global::System.Collections.Generic.IEnumerable<WaveStream> inputStreams, bool autoStop)
			: this()
		{
			AutoStop = autoStop;
			global::System.Collections.Generic.IEnumerator<WaveStream> enumerator = inputStreams.GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					WaveStream current = enumerator.get_Current();
					AddInputStream(current);
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
		}

		public void AddInputStream(WaveStream waveStream)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			if (waveStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Must be IEEE floating point", "waveStream");
			}
			if (waveStream.WaveFormat.BitsPerSample != 32)
			{
				throw new ArgumentException("Only 32 bit audio currently supported", "waveStream");
			}
			if (inputStreams.get_Count() == 0)
			{
				int sampleRate = waveStream.WaveFormat.SampleRate;
				int channels = waveStream.WaveFormat.Channels;
				waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
			}
			else if (!((object)waveStream.WaveFormat).Equals((object)waveFormat))
			{
				throw new ArgumentException("All incoming channels must have the same format", "waveStream");
			}
			lock (inputsLock)
			{
				inputStreams.Add(waveStream);
				length = Math.Max(length, ((Stream)waveStream).get_Length());
				((Stream)waveStream).set_Position(((Stream)this).get_Position());
			}
		}

		public void RemoveInputStream(WaveStream waveStream)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			lock (inputsLock)
			{
				if (!inputStreams.Remove(waveStream))
				{
					return;
				}
				long num = 0L;
				Enumerator<WaveStream> enumerator = inputStreams.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						WaveStream current = enumerator.get_Current();
						num = Math.Max(num, ((Stream)current).get_Length());
					}
				}
				finally
				{
					((global::System.IDisposable)enumerator).Dispose();
				}
				length = num;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (AutoStop && position + count > length)
			{
				count = (int)(length - position);
			}
			if (count % bytesPerSample != 0)
			{
				throw new ArgumentException("Must read an whole number of samples", "count");
			}
			global::System.Array.Clear((global::System.Array)buffer, offset, count);
			int num = 0;
			byte[] array = new byte[count];
			lock (inputsLock)
			{
				Enumerator<WaveStream> enumerator = inputStreams.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						WaveStream current = enumerator.get_Current();
						if (current.HasData(count))
						{
							int num2 = ((Stream)current).Read(array, 0, count);
							num = Math.Max(num, num2);
							if (num2 > 0)
							{
								Sum32BitAudio(buffer, offset, array, num2);
							}
						}
						else
						{
							num = Math.Max(num, count);
							((Stream)current).set_Position(((Stream)current).get_Position() + count);
						}
					}
				}
				finally
				{
					((global::System.IDisposable)enumerator).Dispose();
				}
			}
			position += count;
			return count;
		}

		private unsafe static void Sum32BitAudio(byte[] destBuffer, int offset, byte[] sourceBuffer, int bytesRead)
		{
			fixed (byte* ptr = &destBuffer[offset])
			{
				fixed (byte* ptr3 = &sourceBuffer[0])
				{
					float* ptr2 = (float*)ptr;
					float* ptr4 = (float*)ptr3;
					int num = bytesRead / 4;
					for (int i = 0; i < num; i++)
					{
						ptr2[i] += ptr4[i];
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (disposing)
			{
				lock (inputsLock)
				{
					Enumerator<WaveStream> enumerator = inputStreams.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							((Stream)enumerator.get_Current()).Dispose();
						}
					}
					finally
					{
						((global::System.IDisposable)enumerator).Dispose();
					}
				}
			}
			((Stream)this).Dispose(disposing);
		}
	}
}

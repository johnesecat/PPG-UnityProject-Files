using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NAudio.Utils;

namespace NAudio.Wave
{
	public class MultiplexingWaveProvider : IWaveProvider
	{
		private readonly global::System.Collections.Generic.IList<IWaveProvider> inputs;

		private readonly int outputChannelCount;

		private readonly int inputChannelCount;

		private readonly List<int> mappings;

		private readonly int bytesPerSample;

		private byte[] inputBuffer;

		public WaveFormat WaveFormat
		{
			[CompilerGenerated]
			get
			{
				return <WaveFormat>k__BackingField;
			}
		}

		public int InputChannelCount => inputChannelCount;

		public int OutputChannelCount => outputChannelCount;

		public MultiplexingWaveProvider(global::System.Collections.Generic.IEnumerable<IWaveProvider> inputs)
			: this(inputs, -1)
		{
		}

		public MultiplexingWaveProvider(global::System.Collections.Generic.IEnumerable<IWaveProvider> inputs, int numberOfOutputChannels)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			this.inputs = (global::System.Collections.Generic.IList<IWaveProvider>)new List<IWaveProvider>(inputs);
			outputChannelCount = ((numberOfOutputChannels == -1) ? Enumerable.Sum<IWaveProvider>((global::System.Collections.Generic.IEnumerable<IWaveProvider>)this.inputs, (Func<IWaveProvider, int>)((IWaveProvider i) => i.WaveFormat.Channels)) : numberOfOutputChannels);
			if (((global::System.Collections.Generic.ICollection<IWaveProvider>)this.inputs).get_Count() == 0)
			{
				throw new ArgumentException("You must provide at least one input");
			}
			if (outputChannelCount < 1)
			{
				throw new ArgumentException("You must provide at least one output");
			}
			global::System.Collections.Generic.IEnumerator<IWaveProvider> enumerator = ((global::System.Collections.Generic.IEnumerable<IWaveProvider>)this.inputs).GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					IWaveProvider current = enumerator.get_Current();
					if (WaveFormat == null)
					{
						if (current.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
						{
							<WaveFormat>k__BackingField = new WaveFormat(current.WaveFormat.SampleRate, current.WaveFormat.BitsPerSample, outputChannelCount);
						}
						else
						{
							if (current.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
							{
								throw new ArgumentException("Only PCM and 32 bit float are supported");
							}
							<WaveFormat>k__BackingField = WaveFormat.CreateIeeeFloatWaveFormat(current.WaveFormat.SampleRate, outputChannelCount);
						}
					}
					else
					{
						if (current.WaveFormat.BitsPerSample != WaveFormat.BitsPerSample)
						{
							throw new ArgumentException("All inputs must have the same bit depth");
						}
						if (current.WaveFormat.SampleRate != WaveFormat.SampleRate)
						{
							throw new ArgumentException("All inputs must have the same sample rate");
						}
					}
					inputChannelCount += current.WaveFormat.Channels;
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			bytesPerSample = WaveFormat.BitsPerSample / 8;
			mappings = new List<int>();
			for (int j = 0; j < outputChannelCount; j++)
			{
				mappings.Add(j % inputChannelCount);
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = bytesPerSample * outputChannelCount;
			int num2 = count / num;
			int num3 = 0;
			int num4 = 0;
			global::System.Collections.Generic.IEnumerator<IWaveProvider> enumerator = ((global::System.Collections.Generic.IEnumerable<IWaveProvider>)inputs).GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					IWaveProvider current = enumerator.get_Current();
					int num5 = bytesPerSample * current.WaveFormat.Channels;
					int num6 = num2 * num5;
					inputBuffer = BufferHelpers.Ensure(inputBuffer, num6);
					int num7 = current.Read(inputBuffer, 0, num6);
					num4 = Math.Max(num4, num7 / num5);
					for (int i = 0; i < current.WaveFormat.Channels; i++)
					{
						int num8 = num3 + i;
						for (int j = 0; j < outputChannelCount; j++)
						{
							if (mappings.get_Item(j) != num8)
							{
								continue;
							}
							int num9 = i * bytesPerSample;
							int num10 = offset + j * bytesPerSample;
							int k;
							for (k = 0; k < num2; k++)
							{
								if (num9 >= num7)
								{
									break;
								}
								global::System.Array.Copy((global::System.Array)inputBuffer, num9, (global::System.Array)buffer, num10, bytesPerSample);
								num10 += num;
								num9 += num5;
							}
							for (; k < num2; k++)
							{
								global::System.Array.Clear((global::System.Array)buffer, num10, bytesPerSample);
								num10 += num;
							}
						}
					}
					num3 += current.WaveFormat.Channels;
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			return num4 * num;
		}

		public void ConnectInputToOutput(int inputChannel, int outputChannel)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (inputChannel < 0 || inputChannel >= InputChannelCount)
			{
				throw new ArgumentException("Invalid input channel");
			}
			if (outputChannel < 0 || outputChannel >= OutputChannelCount)
			{
				throw new ArgumentException("Invalid output channel");
			}
			mappings.set_Item(outputChannel, inputChannel);
		}
	}
}

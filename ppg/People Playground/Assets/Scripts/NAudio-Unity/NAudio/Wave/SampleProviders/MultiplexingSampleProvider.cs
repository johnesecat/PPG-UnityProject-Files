using System;
using System.Collections;
using System.Collections.Generic;
using NAudio.Utils;

namespace NAudio.Wave.SampleProviders
{
	public class MultiplexingSampleProvider : ISampleProvider
	{
		private readonly global::System.Collections.Generic.IList<ISampleProvider> inputs;

		private readonly WaveFormat waveFormat;

		private readonly int outputChannelCount;

		private readonly int inputChannelCount;

		private readonly List<int> mappings;

		private float[] inputBuffer;

		public WaveFormat WaveFormat => waveFormat;

		public int InputChannelCount => inputChannelCount;

		public int OutputChannelCount => outputChannelCount;

		public MultiplexingSampleProvider(global::System.Collections.Generic.IEnumerable<ISampleProvider> inputs, int numberOfOutputChannels)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			this.inputs = (global::System.Collections.Generic.IList<ISampleProvider>)new List<ISampleProvider>(inputs);
			outputChannelCount = numberOfOutputChannels;
			if (((global::System.Collections.Generic.ICollection<ISampleProvider>)this.inputs).get_Count() == 0)
			{
				throw new ArgumentException("You must provide at least one input");
			}
			if (numberOfOutputChannels < 1)
			{
				throw new ArgumentException("You must provide at least one output");
			}
			global::System.Collections.Generic.IEnumerator<ISampleProvider> enumerator = ((global::System.Collections.Generic.IEnumerable<ISampleProvider>)this.inputs).GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					ISampleProvider current = enumerator.get_Current();
					if (waveFormat == null)
					{
						if (current.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
						{
							throw new ArgumentException("Only 32 bit float is supported");
						}
						waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(current.WaveFormat.SampleRate, numberOfOutputChannels);
					}
					else
					{
						if (current.WaveFormat.BitsPerSample != waveFormat.BitsPerSample)
						{
							throw new ArgumentException("All inputs must have the same bit depth");
						}
						if (current.WaveFormat.SampleRate != waveFormat.SampleRate)
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
			mappings = new List<int>();
			for (int i = 0; i < outputChannelCount; i++)
			{
				mappings.Add(i % inputChannelCount);
			}
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = count / outputChannelCount;
			int num2 = 0;
			int num3 = 0;
			global::System.Collections.Generic.IEnumerator<ISampleProvider> enumerator = ((global::System.Collections.Generic.IEnumerable<ISampleProvider>)inputs).GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					ISampleProvider current = enumerator.get_Current();
					int num4 = num * current.WaveFormat.Channels;
					inputBuffer = BufferHelpers.Ensure(inputBuffer, num4);
					int num5 = current.Read(inputBuffer, 0, num4);
					num3 = Math.Max(num3, num5 / current.WaveFormat.Channels);
					for (int i = 0; i < current.WaveFormat.Channels; i++)
					{
						int num6 = num2 + i;
						for (int j = 0; j < outputChannelCount; j++)
						{
							if (mappings.get_Item(j) != num6)
							{
								continue;
							}
							int num7 = i;
							int num8 = offset + j;
							int k;
							for (k = 0; k < num; k++)
							{
								if (num7 >= num5)
								{
									break;
								}
								buffer[num8] = inputBuffer[num7];
								num8 += outputChannelCount;
								num7 += current.WaveFormat.Channels;
							}
							for (; k < num; k++)
							{
								buffer[num8] = 0f;
								num8 += outputChannelCount;
							}
						}
					}
					num2 += current.WaveFormat.Channels;
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			return num3 * outputChannelCount;
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

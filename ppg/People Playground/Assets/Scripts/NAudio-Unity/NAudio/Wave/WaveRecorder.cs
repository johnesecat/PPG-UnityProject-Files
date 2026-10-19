using System;
using System.IO;

namespace NAudio.Wave
{
	public class WaveRecorder : IWaveProvider, global::System.IDisposable
	{
		private WaveFileWriter writer;

		private IWaveProvider source;

		public WaveFormat WaveFormat => source.WaveFormat;

		public WaveRecorder(IWaveProvider source, string destination)
		{
			this.source = source;
			writer = new WaveFileWriter(destination, source.WaveFormat);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = source.Read(buffer, offset, count);
			((Stream)writer).Write(buffer, offset, num);
			return num;
		}

		public void Dispose()
		{
			if (writer != null)
			{
				((Stream)writer).Dispose();
				writer = null;
			}
		}
	}
}

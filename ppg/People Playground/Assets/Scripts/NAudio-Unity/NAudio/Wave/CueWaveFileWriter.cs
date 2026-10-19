using System.IO;

namespace NAudio.Wave
{
	public class CueWaveFileWriter : WaveFileWriter
	{
		private CueList cues;

		public CueWaveFileWriter(string fileName, WaveFormat waveFormat)
			: base(fileName, waveFormat)
		{
		}

		public void AddCue(int position, string label)
		{
			if (cues == null)
			{
				cues = new CueList();
			}
			cues.Add(new Cue(position, label));
		}

		private void WriteCues(BinaryWriter w)
		{
			if (cues != null)
			{
				int num = cues.GetRiffChunks().Length;
				w.Seek(0, (SeekOrigin)2);
				if (w.get_BaseStream().get_Length() % 2 == 1)
				{
					w.Write((byte)0);
				}
				w.Write(cues.GetRiffChunks(), 0, num);
				w.Seek(4, (SeekOrigin)0);
				w.Write((int)(w.get_BaseStream().get_Length() - 8));
			}
		}

		protected override void UpdateHeader(BinaryWriter writer)
		{
			base.UpdateHeader(writer);
			WriteCues(writer);
		}
	}
}

using System;
using System.IO;

namespace NAudio.Midi
{
	internal class SmpteOffsetEvent : MetaEvent
	{
		private byte hours;

		private byte minutes;

		private byte seconds;

		private byte frames;

		private byte subFrames;

		public int Hours => hours;

		public int Minutes => minutes;

		public int Seconds => seconds;

		public int Frames => frames;

		public int SubFrames => subFrames;

		public SmpteOffsetEvent(byte hours, byte minutes, byte seconds, byte frames, byte subFrames)
		{
			this.hours = hours;
			this.minutes = minutes;
			this.seconds = seconds;
			this.frames = frames;
			this.subFrames = subFrames;
		}

		public SmpteOffsetEvent(BinaryReader br, int length)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (length != 5)
			{
				throw new FormatException($"Invalid SMPTE Offset length: Got {length}, expected 5");
			}
			hours = br.ReadByte();
			minutes = br.ReadByte();
			seconds = br.ReadByte();
			frames = br.ReadByte();
			subFrames = br.ReadByte();
		}

		public override MidiEvent Clone()
		{
			return (SmpteOffsetEvent)((object)this).MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format("{0} {1}:{2}:{3}:{4}:{5}", new object[6]
			{
				base.ToString(),
				hours,
				minutes,
				seconds,
				frames,
				subFrames
			});
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(hours);
			writer.Write(minutes);
			writer.Write(seconds);
			writer.Write(frames);
			writer.Write(subFrames);
		}
	}
}

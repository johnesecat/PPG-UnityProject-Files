using System;
using System.IO;

namespace NAudio.Midi
{
	public class TrackSequenceNumberEvent : MetaEvent
	{
		private ushort sequenceNumber;

		public TrackSequenceNumberEvent(ushort sequenceNumber)
		{
			this.sequenceNumber = sequenceNumber;
		}

		public TrackSequenceNumberEvent(BinaryReader br, int length)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if (length != 2)
			{
				throw new FormatException("Invalid sequence number length");
			}
			sequenceNumber = (ushort)((br.ReadByte() << 8) + br.ReadByte());
		}

		public override MidiEvent Clone()
		{
			return (TrackSequenceNumberEvent)((object)this).MemberwiseClone();
		}

		public override string ToString()
		{
			return $"{base.ToString()} {sequenceNumber}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write((byte)((uint)(sequenceNumber >> 8) & 0xFFu));
			writer.Write((byte)(sequenceNumber & 0xFFu));
		}
	}
}

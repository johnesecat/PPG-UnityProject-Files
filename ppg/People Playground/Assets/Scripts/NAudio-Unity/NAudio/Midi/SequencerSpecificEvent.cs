using System;
using System.IO;
using System.Text;

namespace NAudio.Midi
{
	public class SequencerSpecificEvent : MetaEvent
	{
		private byte[] data;

		public byte[] Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
				metaDataLength = data.Length;
			}
		}

		public SequencerSpecificEvent(BinaryReader br, int length)
		{
			data = br.ReadBytes(length);
		}

		public SequencerSpecificEvent(byte[] data, long absoluteTime)
			: base(MetaEventType.SequencerSpecific, data.Length, absoluteTime)
		{
			this.data = data;
		}

		public override MidiEvent Clone()
		{
			return new SequencerSpecificEvent((byte[])((global::System.Array)data).Clone(), base.AbsoluteTime);
		}

		public override string ToString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			StringBuilder val = new StringBuilder();
			val.Append(base.ToString());
			val.Append(" ");
			byte[] array = data;
			foreach (byte b in array)
			{
				val.AppendFormat("{0:X2} ", (object)b);
			}
			int i = val.get_Length();
			val.set_Length(i - 1);
			return ((object)val).ToString();
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(data);
		}
	}
}

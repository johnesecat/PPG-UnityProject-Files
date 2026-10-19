using System;
using System.IO;

namespace NAudio.Midi
{
	public class TimeSignatureEvent : MetaEvent
	{
		private byte numerator;

		private byte denominator;

		private byte ticksInMetronomeClick;

		private byte no32ndNotesInQuarterNote;

		public int Numerator => numerator;

		public int Denominator => denominator;

		public int TicksInMetronomeClick => ticksInMetronomeClick;

		public int No32ndNotesInQuarterNote => no32ndNotesInQuarterNote;

		public string TimeSignature
		{
			get
			{
				string text = $"Unknown ({denominator})";
				switch (denominator)
				{
				case 1:
					text = "2";
					break;
				case 2:
					text = "4";
					break;
				case 3:
					text = "8";
					break;
				case 4:
					text = "16";
					break;
				case 5:
					text = "32";
					break;
				}
				return $"{numerator}/{text}";
			}
		}

		public TimeSignatureEvent(BinaryReader br, int length)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (length != 4)
			{
				throw new FormatException($"Invalid time signature length: Got {length}, expected 4");
			}
			numerator = br.ReadByte();
			denominator = br.ReadByte();
			ticksInMetronomeClick = br.ReadByte();
			no32ndNotesInQuarterNote = br.ReadByte();
		}

		public TimeSignatureEvent(long absoluteTime, int numerator, int denominator, int ticksInMetronomeClick, int no32ndNotesInQuarterNote)
			: base(MetaEventType.TimeSignature, 4, absoluteTime)
		{
			this.numerator = (byte)numerator;
			this.denominator = (byte)denominator;
			this.ticksInMetronomeClick = (byte)ticksInMetronomeClick;
			this.no32ndNotesInQuarterNote = (byte)no32ndNotesInQuarterNote;
		}

		public override MidiEvent Clone()
		{
			return (TimeSignatureEvent)((object)this).MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format("{0} {1} TicksInClick:{2} 32ndsInQuarterNote:{3}", new object[4]
			{
				base.ToString(),
				TimeSignature,
				ticksInMetronomeClick,
				no32ndNotesInQuarterNote
			});
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(numerator);
			writer.Write(denominator);
			writer.Write(ticksInMetronomeClick);
			writer.Write(no32ndNotesInQuarterNote);
		}
	}
}

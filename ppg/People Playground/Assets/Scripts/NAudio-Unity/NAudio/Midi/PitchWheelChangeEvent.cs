using System;
using System.IO;

namespace NAudio.Midi
{
	public class PitchWheelChangeEvent : MidiEvent
	{
		private int pitch;

		public int Pitch
		{
			get
			{
				return pitch;
			}
			set
			{
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				if (value < 0 || value >= 16384)
				{
					throw new ArgumentOutOfRangeException("value", "Pitch value must be in the range 0 - 0x3FFF");
				}
				pitch = value;
			}
		}

		public PitchWheelChangeEvent(BinaryReader br)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			byte b = br.ReadByte();
			byte b2 = br.ReadByte();
			if ((b & 0x80u) != 0)
			{
				throw new FormatException("Invalid pitchwheelchange byte 1");
			}
			if ((b2 & 0x80u) != 0)
			{
				throw new FormatException("Invalid pitchwheelchange byte 2");
			}
			pitch = b + (b2 << 7);
		}

		public PitchWheelChangeEvent(long absoluteTime, int channel, int pitchWheel)
			: base(absoluteTime, channel, MidiCommandCode.PitchWheelChange)
		{
			Pitch = pitchWheel;
		}

		public override string ToString()
		{
			return $"{base.ToString()} Pitch {pitch} ({pitch - 8192})";
		}

		public override int GetAsShortMessage()
		{
			return base.GetAsShortMessage() + ((pitch & 0x7F) << 8) + (((pitch >> 7) & 0x7F) << 16);
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write((byte)((uint)pitch & 0x7Fu));
			writer.Write((byte)((uint)(pitch >> 7) & 0x7Fu));
		}
	}
}

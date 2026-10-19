using System;
using System.IO;

namespace NAudio.Midi
{
	public class NoteOnEvent : NoteEvent
	{
		private NoteEvent offEvent;

		public NoteEvent OffEvent
		{
			get
			{
				return offEvent;
			}
			set
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				if (!MidiEvent.IsNoteOff(value))
				{
					throw new ArgumentException("OffEvent must be a valid MIDI note off event");
				}
				if (value.NoteNumber != NoteNumber)
				{
					throw new ArgumentException("Note Off Event must be for the same note number");
				}
				if (value.Channel != Channel)
				{
					throw new ArgumentException("Note Off Event must be for the same channel");
				}
				offEvent = value;
			}
		}

		public override int NoteNumber
		{
			get
			{
				return base.NoteNumber;
			}
			set
			{
				base.NoteNumber = value;
				if (OffEvent != null)
				{
					OffEvent.NoteNumber = NoteNumber;
				}
			}
		}

		public override int Channel
		{
			get
			{
				return base.Channel;
			}
			set
			{
				base.Channel = value;
				if (OffEvent != null)
				{
					OffEvent.Channel = Channel;
				}
			}
		}

		public int NoteLength
		{
			get
			{
				return (int)(offEvent.AbsoluteTime - base.AbsoluteTime);
			}
			set
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				if (value < 0)
				{
					throw new ArgumentException("NoteLength must be 0 or greater");
				}
				offEvent.AbsoluteTime = base.AbsoluteTime + value;
			}
		}

		public NoteOnEvent(BinaryReader br)
			: base(br)
		{
		}

		public NoteOnEvent(long absoluteTime, int channel, int noteNumber, int velocity, int duration)
			: base(absoluteTime, channel, MidiCommandCode.NoteOn, noteNumber, velocity)
		{
			OffEvent = new NoteEvent(absoluteTime, channel, MidiCommandCode.NoteOff, noteNumber, 0);
			NoteLength = duration;
		}

		public override MidiEvent Clone()
		{
			return new NoteOnEvent(base.AbsoluteTime, Channel, NoteNumber, base.Velocity, NoteLength);
		}

		public override string ToString()
		{
			if (base.Velocity == 0 && OffEvent == null)
			{
				return $"{base.ToString()} (Note Off)";
			}
			return string.Format("{0} Len: {1}", (object)base.ToString(), (object)((OffEvent == null) ? "?" : NoteLength.ToString()));
		}
	}
}

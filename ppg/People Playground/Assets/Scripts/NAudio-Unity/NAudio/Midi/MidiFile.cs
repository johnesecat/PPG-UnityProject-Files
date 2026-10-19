using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NAudio.Utils;

namespace NAudio.Midi
{
	public class MidiFile
	{
		private readonly MidiEventCollection events;

		private readonly ushort fileFormat;

		private readonly ushort deltaTicksPerQuarterNote;

		private readonly bool strictChecking;

		public int FileFormat => fileFormat;

		public MidiEventCollection Events => events;

		public int Tracks => events.Tracks;

		public int DeltaTicksPerQuarterNote => deltaTicksPerQuarterNote;

		public MidiFile(string filename)
			: this(filename, strictChecking: true)
		{
		}

		public MidiFile(string filename, bool strictChecking)
			: this((Stream)(object)File.OpenRead(filename), strictChecking, ownInputStream: true)
		{
		}

		public MidiFile(Stream inputStream, bool strictChecking)
			: this(inputStream, strictChecking, ownInputStream: false)
		{
		}

		private MidiFile(Stream inputStream, bool strictChecking, bool ownInputStream)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			this.strictChecking = strictChecking;
			BinaryReader val = new BinaryReader(inputStream);
			try
			{
				if (Encoding.get_UTF8().GetString(val.ReadBytes(4)) != "MThd")
				{
					throw new FormatException("Not a MIDI file - header chunk missing");
				}
				uint num = SwapUInt32(val.ReadUInt32());
				if (num != 6)
				{
					throw new FormatException("Unexpected header chunk length");
				}
				fileFormat = SwapUInt16(val.ReadUInt16());
				int num2 = SwapUInt16(val.ReadUInt16());
				deltaTicksPerQuarterNote = SwapUInt16(val.ReadUInt16());
				events = new MidiEventCollection((fileFormat != 0) ? 1 : 0, deltaTicksPerQuarterNote);
				for (int i = 0; i < num2; i++)
				{
					events.AddTrack();
				}
				long num3 = 0L;
				for (int j = 0; j < num2; j++)
				{
					if (fileFormat == 1)
					{
						num3 = 0L;
					}
					if (Encoding.get_UTF8().GetString(val.ReadBytes(4)) != "MTrk")
					{
						throw new FormatException("Invalid chunk header");
					}
					num = SwapUInt32(val.ReadUInt32());
					long position = val.get_BaseStream().get_Position();
					MidiEvent midiEvent = null;
					List<NoteOnEvent> val2 = new List<NoteOnEvent>();
					while (val.get_BaseStream().get_Position() < position + num)
					{
						try
						{
							midiEvent = MidiEvent.ReadNextEvent(val, midiEvent);
						}
						catch (InvalidDataException)
						{
							if (strictChecking)
							{
								throw;
							}
							continue;
						}
						catch (FormatException)
						{
							if (strictChecking)
							{
								throw;
							}
							continue;
						}
						num3 = (midiEvent.AbsoluteTime = num3 + midiEvent.DeltaTime);
						((global::System.Collections.Generic.ICollection<MidiEvent>)events[j]).Add(midiEvent);
						if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
						{
							NoteEvent noteEvent = (NoteEvent)midiEvent;
							if (noteEvent.Velocity > 0)
							{
								val2.Add((NoteOnEvent)noteEvent);
							}
							else
							{
								FindNoteOn(noteEvent, val2);
							}
						}
						else if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
						{
							FindNoteOn((NoteEvent)midiEvent, val2);
						}
						else if (midiEvent.CommandCode == MidiCommandCode.MetaEvent && ((MetaEvent)midiEvent).MetaEventType == MetaEventType.EndTrack && strictChecking && val.get_BaseStream().get_Position() < position + num)
						{
							throw new FormatException($"End Track event was not the last MIDI event on track {j}");
						}
					}
					if (val2.get_Count() > 0 && strictChecking)
					{
						throw new FormatException($"Note ons without note offs {val2.get_Count()} (file format {fileFormat})");
					}
					if (val.get_BaseStream().get_Position() != position + num)
					{
						throw new FormatException($"Read too far {num}+{position}!={val.get_BaseStream().get_Position()}");
					}
				}
			}
			finally
			{
				if (ownInputStream)
				{
					val.Close();
				}
			}
		}

		private void FindNoteOn(NoteEvent offEvent, List<NoteOnEvent> outstandingNoteOns)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			Enumerator<NoteOnEvent> enumerator = outstandingNoteOns.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NoteOnEvent current = enumerator.get_Current();
					if (current.Channel == offEvent.Channel && current.NoteNumber == offEvent.NoteNumber)
					{
						current.OffEvent = offEvent;
						outstandingNoteOns.Remove(current);
						flag = true;
						break;
					}
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator).Dispose();
			}
			if (!flag && strictChecking)
			{
				throw new FormatException($"Got an off without an on {offEvent}");
			}
		}

		private static uint SwapUInt32(uint i)
		{
			return ((i & 0xFF000000u) >> 24) | ((i & 0xFF0000) >> 8) | ((i & 0xFF00) << 8) | ((i & 0xFF) << 24);
		}

		private static ushort SwapUInt16(ushort i)
		{
			return (ushort)(((i & 0xFF00) >> 8) | ((i & 0xFF) << 8));
		}

		public override string ToString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			StringBuilder val = new StringBuilder();
			val.AppendFormat("Format {0}, Tracks {1}, Delta Ticks Per Quarter Note {2}\r\n", (object)fileFormat, (object)Tracks, (object)deltaTicksPerQuarterNote);
			for (int i = 0; i < Tracks; i++)
			{
				global::System.Collections.Generic.IEnumerator<MidiEvent> enumerator = ((global::System.Collections.Generic.IEnumerable<MidiEvent>)events[i]).GetEnumerator();
				try
				{
					while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
					{
						MidiEvent current = enumerator.get_Current();
						val.AppendFormat("{0}\r\n", (object)current);
					}
				}
				finally
				{
					((global::System.IDisposable)enumerator)?.Dispose();
				}
			}
			return ((object)val).ToString();
		}

		public static void Export(string filename, MidiEventCollection events)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			if (events.MidiFileType == 0 && events.Tracks > 1)
			{
				throw new ArgumentException("Can't export more than one track to a type 0 file");
			}
			BinaryWriter val = new BinaryWriter((Stream)(object)File.Create(filename));
			try
			{
				val.Write(Encoding.get_UTF8().GetBytes("MThd"));
				val.Write(SwapUInt32(6u));
				val.Write(SwapUInt16((ushort)events.MidiFileType));
				val.Write(SwapUInt16((ushort)events.Tracks));
				val.Write(SwapUInt16((ushort)events.DeltaTicksPerQuarterNote));
				for (int i = 0; i < events.Tracks; i++)
				{
					global::System.Collections.Generic.IList<MidiEvent> list = events[i];
					val.Write(Encoding.get_UTF8().GetBytes("MTrk"));
					long position = val.get_BaseStream().get_Position();
					val.Write(SwapUInt32(0u));
					long absoluteTime = events.StartAbsoluteTime;
					MergeSort.Sort(list, new MidiEventComparer());
					((global::System.Collections.Generic.ICollection<MidiEvent>)list).get_Count();
					_ = 0;
					global::System.Collections.Generic.IEnumerator<MidiEvent> enumerator = ((global::System.Collections.Generic.IEnumerable<MidiEvent>)list).GetEnumerator();
					try
					{
						while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
						{
							enumerator.get_Current().Export(ref absoluteTime, val);
						}
					}
					finally
					{
						((global::System.IDisposable)enumerator)?.Dispose();
					}
					uint num = (uint)((int)(val.get_BaseStream().get_Position() - position) - 4);
					val.get_BaseStream().set_Position(position);
					val.Write(SwapUInt32(num));
					Stream baseStream = val.get_BaseStream();
					baseStream.set_Position(baseStream.get_Position() + num);
				}
			}
			finally
			{
				((global::System.IDisposable)val)?.Dispose();
			}
		}
	}
}

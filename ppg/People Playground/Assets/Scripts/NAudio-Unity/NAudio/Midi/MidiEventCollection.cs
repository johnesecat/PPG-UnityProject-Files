using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using NAudio.Utils;

namespace NAudio.Midi
{
	[DefaultMember("Item")]
	public class MidiEventCollection : global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.IList<MidiEvent>>, global::System.Collections.IEnumerable
	{
		private int midiFileType;

		private readonly List<global::System.Collections.Generic.IList<MidiEvent>> trackEvents;

		public int Tracks => trackEvents.get_Count();

		public long StartAbsoluteTime
		{
			[CompilerGenerated]
			get
			{
				return <StartAbsoluteTime>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<StartAbsoluteTime>k__BackingField = value;
			}
		}

		public int DeltaTicksPerQuarterNote
		{
			[CompilerGenerated]
			get
			{
				return <DeltaTicksPerQuarterNote>k__BackingField;
			}
		}

		public global::System.Collections.Generic.IList<MidiEvent> this[int trackNumber] => trackEvents.get_Item(trackNumber);

		public int MidiFileType
		{
			get
			{
				return midiFileType;
			}
			set
			{
				if (midiFileType != value)
				{
					midiFileType = value;
					if (value == 0)
					{
						FlattenToOneTrack();
					}
					else
					{
						ExplodeToManyTracks();
					}
				}
			}
		}

		public MidiEventCollection(int midiFileType, int deltaTicksPerQuarterNote)
		{
			this.midiFileType = midiFileType;
			<DeltaTicksPerQuarterNote>k__BackingField = deltaTicksPerQuarterNote;
			StartAbsoluteTime = 0L;
			trackEvents = new List<global::System.Collections.Generic.IList<MidiEvent>>();
		}

		public global::System.Collections.Generic.IList<MidiEvent> GetTrackEvents(int trackNumber)
		{
			return trackEvents.get_Item(trackNumber);
		}

		public global::System.Collections.Generic.IList<MidiEvent> AddTrack()
		{
			return AddTrack(null);
		}

		public global::System.Collections.Generic.IList<MidiEvent> AddTrack(global::System.Collections.Generic.IList<MidiEvent> initialEvents)
		{
			List<MidiEvent> val = new List<MidiEvent>();
			if (initialEvents != null)
			{
				val.AddRange((global::System.Collections.Generic.IEnumerable<MidiEvent>)initialEvents);
			}
			trackEvents.Add((global::System.Collections.Generic.IList<MidiEvent>)val);
			return (global::System.Collections.Generic.IList<MidiEvent>)val;
		}

		public void RemoveTrack(int track)
		{
			trackEvents.RemoveAt(track);
		}

		public void Clear()
		{
			trackEvents.Clear();
		}

		public void AddEvent(MidiEvent midiEvent, int originalTrack)
		{
			if (midiFileType == 0)
			{
				EnsureTracks(1);
				((global::System.Collections.Generic.ICollection<MidiEvent>)trackEvents.get_Item(0)).Add(midiEvent);
			}
			else if (originalTrack == 0)
			{
				switch (midiEvent.CommandCode)
				{
				case MidiCommandCode.NoteOff:
				case MidiCommandCode.NoteOn:
				case MidiCommandCode.KeyAfterTouch:
				case MidiCommandCode.ControlChange:
				case MidiCommandCode.PatchChange:
				case MidiCommandCode.ChannelAfterTouch:
				case MidiCommandCode.PitchWheelChange:
					EnsureTracks(midiEvent.Channel + 1);
					((global::System.Collections.Generic.ICollection<MidiEvent>)trackEvents.get_Item(midiEvent.Channel)).Add(midiEvent);
					break;
				default:
					EnsureTracks(1);
					((global::System.Collections.Generic.ICollection<MidiEvent>)trackEvents.get_Item(0)).Add(midiEvent);
					break;
				}
			}
			else
			{
				EnsureTracks(originalTrack + 1);
				((global::System.Collections.Generic.ICollection<MidiEvent>)trackEvents.get_Item(originalTrack)).Add(midiEvent);
			}
		}

		private void EnsureTracks(int count)
		{
			for (int i = trackEvents.get_Count(); i < count; i++)
			{
				trackEvents.Add((global::System.Collections.Generic.IList<MidiEvent>)new List<MidiEvent>());
			}
		}

		private void ExplodeToManyTracks()
		{
			global::System.Collections.Generic.IList<MidiEvent> list = trackEvents.get_Item(0);
			Clear();
			global::System.Collections.Generic.IEnumerator<MidiEvent> enumerator = ((global::System.Collections.Generic.IEnumerable<MidiEvent>)list).GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					MidiEvent current = enumerator.get_Current();
					AddEvent(current, 0);
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			PrepareForExport();
		}

		private void FlattenToOneTrack()
		{
			bool flag = false;
			for (int i = 1; i < trackEvents.get_Count(); i++)
			{
				global::System.Collections.Generic.IEnumerator<MidiEvent> enumerator = ((global::System.Collections.Generic.IEnumerable<MidiEvent>)trackEvents.get_Item(i)).GetEnumerator();
				try
				{
					while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
					{
						MidiEvent current = enumerator.get_Current();
						if (!MidiEvent.IsEndTrack(current))
						{
							((global::System.Collections.Generic.ICollection<MidiEvent>)trackEvents.get_Item(0)).Add(current);
							flag = true;
						}
					}
				}
				finally
				{
					((global::System.IDisposable)enumerator)?.Dispose();
				}
			}
			for (int num = trackEvents.get_Count() - 1; num > 0; num--)
			{
				RemoveTrack(num);
			}
			if (flag)
			{
				PrepareForExport();
			}
		}

		public void PrepareForExport()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			MidiEventComparer comparer = new MidiEventComparer();
			Enumerator<global::System.Collections.Generic.IList<MidiEvent>> enumerator = trackEvents.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					global::System.Collections.Generic.IList<MidiEvent> current = enumerator.get_Current();
					MergeSort.Sort(current, comparer);
					int num = 0;
					while (num < ((global::System.Collections.Generic.ICollection<MidiEvent>)current).get_Count() - 1)
					{
						if (MidiEvent.IsEndTrack(current.get_Item(num)))
						{
							current.RemoveAt(num);
						}
						else
						{
							num++;
						}
					}
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator).Dispose();
			}
			int num2 = 0;
			while (num2 < trackEvents.get_Count())
			{
				global::System.Collections.Generic.IList<MidiEvent> list = trackEvents.get_Item(num2);
				if (((global::System.Collections.Generic.ICollection<MidiEvent>)list).get_Count() == 0)
				{
					RemoveTrack(num2);
					continue;
				}
				if (((global::System.Collections.Generic.ICollection<MidiEvent>)list).get_Count() == 1 && MidiEvent.IsEndTrack(list.get_Item(0)))
				{
					RemoveTrack(num2);
					continue;
				}
				if (!MidiEvent.IsEndTrack(list.get_Item(((global::System.Collections.Generic.ICollection<MidiEvent>)list).get_Count() - 1)))
				{
					((global::System.Collections.Generic.ICollection<MidiEvent>)list).Add((MidiEvent)new MetaEvent(MetaEventType.EndTrack, 0, list.get_Item(((global::System.Collections.Generic.ICollection<MidiEvent>)list).get_Count() - 1).AbsoluteTime));
				}
				num2++;
			}
		}

		public global::System.Collections.Generic.IEnumerator<global::System.Collections.Generic.IList<MidiEvent>> GetEnumerator()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return (global::System.Collections.Generic.IEnumerator<global::System.Collections.Generic.IList<MidiEvent>>)(object)trackEvents.GetEnumerator();
		}

		global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return (global::System.Collections.IEnumerator)(object)trackEvents.GetEnumerator();
		}
	}
}

using System;
using System.Runtime.CompilerServices;

namespace NAudio.Midi
{
	public class MidiInMessageEventArgs : EventArgs
	{
		public int RawMessage
		{
			[CompilerGenerated]
			get
			{
				return <RawMessage>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<RawMessage>k__BackingField = value;
			}
		}

		public MidiEvent MidiEvent
		{
			[CompilerGenerated]
			get
			{
				return <MidiEvent>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<MidiEvent>k__BackingField = value;
			}
		}

		public int Timestamp
		{
			[CompilerGenerated]
			get
			{
				return <Timestamp>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<Timestamp>k__BackingField = value;
			}
		}

		public MidiInMessageEventArgs(int message, int timestamp)
			: this()
		{
			RawMessage = message;
			Timestamp = timestamp;
			try
			{
				MidiEvent = MidiEvent.FromRawMessage(message);
			}
			catch (global::System.Exception)
			{
			}
		}
	}
}

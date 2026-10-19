using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NAudio.Mixer
{
	public class MixerLine
	{
		private MixerInterop.MIXERLINE mixerLine;

		private global::System.IntPtr mixerHandle;

		private MixerFlags mixerHandleType;

		public string Name => mixerLine.szName;

		public string ShortName => mixerLine.szShortName;

		public int LineId => mixerLine.dwLineID;

		public MixerLineComponentType ComponentType => mixerLine.dwComponentType;

		public string TypeDescription
		{
			get
			{
				switch (mixerLine.dwComponentType)
				{
				case MixerLineComponentType.DestinationUndefined:
					return "Undefined Destination";
				case MixerLineComponentType.DestinationDigital:
					return "Digital Destination";
				case MixerLineComponentType.DestinationLine:
					return "Line Level Destination";
				case MixerLineComponentType.DestinationMonitor:
					return "Monitor Destination";
				case MixerLineComponentType.DestinationSpeakers:
					return "Speakers Destination";
				case MixerLineComponentType.DestinationHeadphones:
					return "Headphones Destination";
				case MixerLineComponentType.DestinationTelephone:
					return "Telephone Destination";
				case MixerLineComponentType.DestinationWaveIn:
					return "Wave Input Destination";
				case MixerLineComponentType.DestinationVoiceIn:
					return "Voice Recognition Destination";
				case MixerLineComponentType.SourceUndefined:
					return "Undefined Source";
				case MixerLineComponentType.SourceDigital:
					return "Digital Source";
				case MixerLineComponentType.SourceLine:
					return "Line Level Source";
				case MixerLineComponentType.SourceMicrophone:
					return "Microphone Source";
				case MixerLineComponentType.SourceSynthesizer:
					return "Synthesizer Source";
				case MixerLineComponentType.SourceCompactDisc:
					return "Compact Disk Source";
				case MixerLineComponentType.SourceTelephone:
					return "Telephone Source";
				case MixerLineComponentType.SourcePcSpeaker:
					return "PC Speaker Source";
				case MixerLineComponentType.SourceWaveOut:
					return "Wave Out Source";
				case MixerLineComponentType.SourceAuxiliary:
					return "Auxiliary Source";
				case MixerLineComponentType.SourceAnalog:
					return "Analog Source";
				default:
					return "Invalid Component Type";
				}
			}
		}

		public int Channels => mixerLine.cChannels;

		public int SourceCount => mixerLine.cConnections;

		public int ControlsCount => mixerLine.cControls;

		public bool IsActive => (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_ACTIVE) != 0;

		public bool IsDisconnected => (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_DISCONNECTED) != 0;

		public bool IsSource => (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_SOURCE) != 0;

		public global::System.Collections.Generic.IEnumerable<MixerControl> Controls => (global::System.Collections.Generic.IEnumerable<MixerControl>)MixerControl.GetMixerControls(mixerHandle, this, mixerHandleType);

		public global::System.Collections.Generic.IEnumerable<MixerLine> Sources
		{
			get
			{
				for (int source = 0; source < SourceCount; source++)
				{
					yield return GetSource(source);
				}
			}
		}

		public string TargetName => mixerLine.szPname;

		public MixerLine(global::System.IntPtr mixerHandle, int destinationIndex, MixerFlags mixerHandleType)
		{
			this.mixerHandle = mixerHandle;
			this.mixerHandleType = mixerHandleType;
			mixerLine = default(MixerInterop.MIXERLINE);
			mixerLine.cbStruct = Marshal.SizeOf((object)mixerLine);
			mixerLine.dwDestination = destinationIndex;
			MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType | MixerFlags.Mixer), "mixerGetLineInfo");
		}

		public MixerLine(global::System.IntPtr mixerHandle, int destinationIndex, int sourceIndex, MixerFlags mixerHandleType)
		{
			this.mixerHandle = mixerHandle;
			this.mixerHandleType = mixerHandleType;
			mixerLine = default(MixerInterop.MIXERLINE);
			mixerLine.cbStruct = Marshal.SizeOf((object)mixerLine);
			mixerLine.dwDestination = destinationIndex;
			mixerLine.dwSource = sourceIndex;
			MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType | MixerFlags.ListText), "mixerGetLineInfo");
		}

		public static int GetMixerIdForWaveIn(int waveInDevice)
		{
			int mixerID = -1;
			MmException.Try(MixerInterop.mixerGetID((global::System.IntPtr)waveInDevice, out mixerID, MixerFlags.WaveIn), "mixerGetID");
			return mixerID;
		}

		public MixerLine GetSource(int sourceIndex)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (sourceIndex < 0 || sourceIndex >= SourceCount)
			{
				throw new ArgumentOutOfRangeException("sourceIndex");
			}
			return new MixerLine(mixerHandle, mixerLine.dwDestination, sourceIndex, mixerHandleType);
		}

		public override string ToString()
		{
			return string.Format("{0} {1} ({2} controls, ID={3})", new object[4] { Name, TypeDescription, ControlsCount, mixerLine.dwLineID });
		}
	}
}

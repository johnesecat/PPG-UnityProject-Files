using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace NAudio.Midi
{
	public class RawMetaEvent : MetaEvent
	{
		public byte[] Data
		{
			[CompilerGenerated]
			get
			{
				return <Data>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Data>k__BackingField = value;
			}
		}

		public RawMetaEvent(MetaEventType metaEventType, long absoluteTime, byte[] data)
			: base(metaEventType, (data != null) ? data.Length : 0, absoluteTime)
		{
			Data = data;
		}

		public override MidiEvent Clone()
		{
			return new RawMetaEvent(base.MetaEventType, base.AbsoluteTime, (byte[])((global::System.Array)Data)?.Clone());
		}

		public override string ToString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			StringBuilder val = new StringBuilder().Append(base.ToString());
			byte[] data = Data;
			foreach (byte b in data)
			{
				val.AppendFormat(" {0:X2}", (object)b);
			}
			return ((object)val).ToString();
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			if (Data != null)
			{
				writer.Write(Data, 0, Data.Length);
			}
		}
	}
}

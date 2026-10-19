using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace NAudio.Wave
{
	public class RiffChunk
	{
		public int Identifier
		{
			[CompilerGenerated]
			get
			{
				return <Identifier>k__BackingField;
			}
		}

		public string IdentifierAsString => Encoding.get_UTF8().GetString(BitConverter.GetBytes(Identifier));

		public int Length
		{
			[CompilerGenerated]
			get
			{
				return <Length>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<Length>k__BackingField = value;
			}
		}

		public long StreamPosition
		{
			[CompilerGenerated]
			get
			{
				return <StreamPosition>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<StreamPosition>k__BackingField = value;
			}
		}

		public RiffChunk(int identifier, int length, long streamPosition)
		{
			<Identifier>k__BackingField = identifier;
			Length = length;
			StreamPosition = streamPosition;
		}
	}
}

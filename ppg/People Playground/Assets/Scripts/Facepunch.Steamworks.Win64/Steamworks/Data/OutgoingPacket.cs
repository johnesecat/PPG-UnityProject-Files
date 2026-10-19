using System.Runtime.CompilerServices;

namespace Steamworks.Data
{
	public struct OutgoingPacket
	{
		public uint Address
		{
			[IsReadOnly]
			get;
			internal set; }

		public ushort Port
		{
			[IsReadOnly]
			get;
			internal set; }

		public byte[] Data
		{
			[IsReadOnly]
			get;
			internal set; }

		public int Size
		{
			[IsReadOnly]
			get;
			internal set; }
	}
}

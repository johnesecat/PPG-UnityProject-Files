using System;
using System.Runtime.InteropServices;

namespace NAudio.MediaFoundation
{
	[StructLayout(0)]
	public class MFT_REGISTER_TYPE_INFO
	{
		public Guid guidMajorType;

		public Guid guidSubtype;
	}
}

using System;

namespace NAudio.MediaFoundation
{
	[Flags]
	public enum _MFT_PROCESS_OUTPUT_FLAGS
	{
		None,
		MFT_PROCESS_OUTPUT_DISCARD_WHEN_NO_BUFFER,
		MFT_PROCESS_OUTPUT_REGENERATE_LAST_OUTPUT
	}
}

using System;

namespace NAudio.Wave.Compression
{
	[Flags]
	internal enum AcmStreamOpenFlags
	{
		Query = 1,
		Async = 2,
		NonRealTime = 4,
		CallbackTypeMask = 458752,
		CallbackNull = 0,
		CallbackWindow = 0x10000,
		CallbackTask = 0x20000,
		CallbackFunction = 196608,
		CallbackThread = 0x20000,
		CallbackEvent = 327680
	}
}

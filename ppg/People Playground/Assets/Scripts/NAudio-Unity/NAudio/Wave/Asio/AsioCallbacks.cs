using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Asio
{
	[StructLayout(0, Pack = 4)]
	public struct AsioCallbacks
	{
		[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
		public delegate void AsioBufferSwitchCallBack(int doubleBufferIndex, bool directProcess);

		[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
		public delegate void AsioSampleRateDidChangeCallBack(double sRate);

		[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
		public delegate int AsioAsioMessageCallBack(AsioMessageSelector selector, int value, global::System.IntPtr message, global::System.IntPtr opt);

		[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
		public delegate global::System.IntPtr AsioBufferSwitchTimeInfoCallBack(global::System.IntPtr asioTimeParam, int doubleBufferIndex, bool directProcess);

		public AsioBufferSwitchCallBack pbufferSwitch;

		public AsioSampleRateDidChangeCallBack psampleRateDidChange;

		public AsioAsioMessageCallBack pasioMessage;

		public AsioBufferSwitchTimeInfoCallBack pbufferSwitchTimeInfo;
	}
}

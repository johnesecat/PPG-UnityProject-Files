using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	[DefaultMember("Item")]
	public class MMDeviceCollection : global::System.Collections.Generic.IEnumerable<MMDevice>, global::System.Collections.IEnumerable
	{
		private IMMDeviceCollection _MMDeviceCollection;

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(_MMDeviceCollection.GetCount(out var numDevices));
				return numDevices;
			}
		}

		public MMDevice this[int index]
		{
			get
			{
				_MMDeviceCollection.Item(index, out var device);
				return new MMDevice(device);
			}
		}

		internal MMDeviceCollection(IMMDeviceCollection parent)
		{
			_MMDeviceCollection = parent;
		}

		public global::System.Collections.Generic.IEnumerator<MMDevice> GetEnumerator()
		{
			for (int index = 0; index < Count; index++)
			{
				yield return this[index];
			}
		}

		global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
		{
			return (global::System.Collections.IEnumerator)GetEnumerator();
		}
	}
}

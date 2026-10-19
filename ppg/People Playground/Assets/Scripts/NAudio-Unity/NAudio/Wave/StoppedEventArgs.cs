using System;

namespace NAudio.Wave
{
	public class StoppedEventArgs : EventArgs
	{
		private readonly global::System.Exception exception;

		public global::System.Exception Exception => exception;

		public StoppedEventArgs(global::System.Exception exception = null)
			: this()
		{
			this.exception = exception;
		}
	}
}

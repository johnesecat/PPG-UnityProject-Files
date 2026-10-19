using System;
using System.Collections;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common
{
	public class TrackableCoroutine
	{
		private IEnumerator CoroutineEnumerator { get; set; }

		public bool IsInProgress { get; set; }

		public bool IsForceStopRequested { get; set; }

		public event EventHandler<EventArgs> BeforeYieldReturn;

		public event EventHandler<EventArgs> BeforeStart;

		public event EventHandler<EventArgs> Finished;

		public TrackableCoroutine Init(IEnumerator coroutineEnumerator)
		{
			CoroutineEnumerator = coroutineEnumerator;
			return this;
		}

		public void Start(Func<IEnumerator, Coroutine> startCoroutine)
		{
			IsInProgress = true;
			this.BeforeStart?.Invoke(this, EventArgs.Empty);
			startCoroutine(CoroutineEnumerator);
		}

		public void OnBeforeYieldReturn()
		{
			this.BeforeYieldReturn?.Invoke(this, EventArgs.Empty);
		}

		public void OnFinished()
		{
			IsInProgress = false;
			this.Finished?.Invoke(this, EventArgs.Empty);
		}

		public void ForceStop()
		{
			IsInProgress = false;
			IsForceStopRequested = true;
		}
	}
}

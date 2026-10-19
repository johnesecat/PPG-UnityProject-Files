using System;

namespace ImmersiveVrToolsCommon.Runtime.Utilities
{
	public class Throttler
	{
		private readonly float _everyNSeconds;

		private float _lastTimeCalled;

		public Throttler(float everyNSeconds)
		{
			_everyNSeconds = everyNSeconds;
		}

		public bool TryExecute(float currentTime, Action action)
		{
			if (currentTime > _lastTimeCalled + _everyNSeconds)
			{
				_lastTimeCalled = currentTime;
				action();
				return true;
			}
			return false;
		}

		public bool TryExecute<TResult>(float currentTime, Func<TResult> func, out TResult result)
		{
			if (currentTime > _lastTimeCalled + _everyNSeconds)
			{
				_lastTimeCalled = currentTime;
				result = func();
				return true;
			}
			result = default(TResult);
			return false;
		}
	}
}

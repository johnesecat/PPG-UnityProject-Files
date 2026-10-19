using System;
using System.Collections;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class ChangedSincePreviousFrameHandler<T> : ChangeSincePreviousFrameMonitor<T>
	{
		public delegate void ValueChanged(T oldValue, T newValue);

		private ValueChanged _onValueChanged;

		private readonly bool _triggerHandlerOnInitialUpdateCall;

		private bool _isFirstUpdateFinished;

		public ChangedSincePreviousFrameHandler(Func<T> getValue, MonoBehaviour coroutineRunner, ValueChanged onValueChanged, bool triggerHandlerOnInitialUpdateCall)
			: base(getValue, coroutineRunner, startCoroutineInCtor: false)
		{
			_onValueChanged = onValueChanged;
			_triggerHandlerOnInitialUpdateCall = triggerHandlerOnInitialUpdateCall;
			coroutineRunner.StartCoroutine(Update());
		}

		protected override IEnumerator Update()
		{
			IEnumerator updateEnumerator = base.Update();
			while (updateEnumerator.MoveNext())
			{
				if (!_isFirstUpdateFinished && _triggerHandlerOnInitialUpdateCall)
				{
					TriggerHandlerSafe();
				}
				_isFirstUpdateFinished = true;
				if (base.IsValueUpdatedSinceLastUpdateCall)
				{
					TriggerHandlerSafe();
				}
				yield return updateEnumerator.Current;
			}
		}

		private void TriggerHandlerSafe()
		{
			try
			{
				_onValueChanged?.Invoke(_previousValue, base.CurrentValue);
			}
			catch (Exception message)
			{
				UnityEngine.Debug.LogError(message);
			}
		}
	}
}

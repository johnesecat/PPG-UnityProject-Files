using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImmersiveVRTools.Runtime.Common
{
	public class UnityMainThreadDispatcher : SingletonBase<UnityMainThreadDispatcher>
	{
		private static readonly Queue<Action> _executionQueue = new Queue<Action>();

		public void Update()
		{
			lock (_executionQueue)
			{
				while (_executionQueue.Count > 0)
				{
					_executionQueue.Dequeue()();
				}
			}
		}

		public void Enqueue(IEnumerator action)
		{
			lock (_executionQueue)
			{
				_executionQueue.Enqueue(delegate
				{
					StartCoroutine(action);
				});
			}
		}

		public void Enqueue(Action action)
		{
			Enqueue(ActionWrapper(action));
		}

		public Task EnqueueAsync(Action action)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Enqueue(ActionWrapper(WrappedAction));
			return tcs.Task;
			void WrappedAction()
			{
				try
				{
					action();
					tcs.TrySetResult(result: true);
				}
				catch (Exception exception)
				{
					tcs.TrySetException(exception);
				}
			}
		}

		private IEnumerator ActionWrapper(Action a)
		{
			a();
			yield return null;
		}
	}
}

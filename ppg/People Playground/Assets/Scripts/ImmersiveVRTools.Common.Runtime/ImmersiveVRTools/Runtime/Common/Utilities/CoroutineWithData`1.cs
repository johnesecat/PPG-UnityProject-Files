using System.Collections;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class CoroutineWithData<TResult>
	{
		private IEnumerator _target;

		public Coroutine Coroutine { get; private set; }

		public TResult Result { get; private set; }

		public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
		{
			_target = target;
			Coroutine = owner.StartCoroutine(Run());
		}

		private IEnumerator Run()
		{
			while (_target.MoveNext())
			{
				object current;
				if ((current = _target.Current) is TResult)
				{
					TResult val2 = (Result = (TResult)current);
				}
				yield return _target.Current;
			}
		}
	}
}
